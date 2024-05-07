// jdyun 24/05/06(월) - 어린이날 대체 휴일
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using P4VHelper.Base;
using P4VHelper.Base.Extension;
using P4VHelper.Engine.Cache;
using P4VHelper.Engine.Collection;
using P4VHelper.Engine.Model;
using P4VHelper.Engine.Param;
using P4VHelper.ViewModel;
using Perforce.P4;

namespace P4VHelper.Model
{
    public enum EndScrollState
    {
        None,
        Step1,
        Step2,
    }

    public class SearchResult<T> : Bindable where T : ISegmentElement
    {
        private readonly SortedDictionary<int, SegmentResult> bufferedList_;    // 패럴 쓰레드에서 접근하는 리스트
        private readonly WpfObservableRangeCollection<T> viewList_;             // UI 쓰레드 리스트
        private int cursor_;                                                    // 현재 프로세싱중인 리스트 ID
        private int cursorLast_;                                                // 프로세싱 된 리스트 ID
        private DateTime loadingScrollTime_;                                    // 로딩중 ViewMoreItems를 호출한 시각
        private readonly TimeSpan loadingScrollSyncDelay_;                      // 로딩중 스크롤 ViewMoreItems를 실행할 주기
        private int _scrollableItemCount;                                       // 스크롤 가능한 남은 아이템 수 (멀티쓰레드에서 접근하지만 한번에 접근하는 쓰레드는 1개이므므로 아토믹 처리안함)
        private SearchParam? param_;

        public MainViewModel ViewModel { get; }
        public WpfObservableRangeCollection<T> ViewList { get; }
        public int ScrollableItemCount => _scrollableItemCount;

        public SearchResult(MainViewModel _viewModel)
        {
            ViewModel = _viewModel;
            ViewList = new WpfObservableRangeCollection<T>();

            bufferedList_ = new SortedDictionary<int, SegmentResult>();
            viewList_ = ViewList;
            cursor_ = 1;
            cursorLast_ = 0;
            loadingScrollSyncDelay_ = TimeSpan.FromMilliseconds(300);
        }

        public void Add(List<T> _items)
        {
            lock (this)
            {
                int id = ++cursorLast_;
                var segResult = new SegmentResult()
                {
                    Id = id,
                    Result = _items,
                    ReadingOffset = 0,
                };

                bufferedList_.Add(id, segResult);

                DateTime now = DateTime.Now;
                if (loadingScrollTime_ + loadingScrollSyncDelay_ <= now)
                {
                    ViewMoreItems(ViewModel.Config.ScrollLimit);
                    loadingScrollTime_ = now;
                }

                // 스크롤 가능한 남은 아이템 수 업데이트
                UpdateScrollableItemCount();
            }
        }

        private static Stopwatch sw = new();
        public void Reset(SearchParam _param)
        {
            viewList_.Clear();

            lock (this)
            {
                bufferedList_.Clear();

                param_ = _param;
                loadingScrollTime_ = new DateTime(0);
                cursor_ = 1;
                cursorLast_ = 0;
            }
        }

        /// <summary>
        /// 인자로 전달받은 갯수만큼 버퍼링된 아이템을 UI 목록에 반영하도록 한다.
        /// 로딩중 200ms 마다 호출되거나 사용자가 데이터그리드를 스크롤하면서 호출됨.
        /// </summary>
        public int ViewMoreItems(int _scrollLimit, bool _async = true)
        {
            int leftReadCount = _scrollLimit;
            List<T> readElements = new List<T>(_scrollLimit);

            lock (this)
            {
                for (; cursor_ <= cursorLast_; )
                {
                    if (param_.Notifier.IsInterruptRequested)
                        break;

                    Debug.Assert(bufferedList_.ContainsKey(cursor_));    // 당연히 존재해야한다.
                    SegmentResult segResult = bufferedList_[cursor_];
                    Debug.Assert(segResult.Result.Count > 0);

                    int readCount = readElements.AddRange(segResult.Result, segResult.ReadingOffset, leftReadCount);
                    leftReadCount -= readCount;
                    segResult.ReadingOffset += readCount;

                    Debug.Assert(leftReadCount >= 0);
                    if (segResult.ReadableCount == 0)
                        cursor_++;

                    if (leftReadCount <= 0)
                        break;
                }

                if (_async)
                {
                    // double check (BeginInvoke(비동기)로 안하면 한번만 체크해도댐)
                    // 만약 double check를 안하고 비동기로 처리해버리면 background task가 interrupt가 되고나서
                    // 아이템이 추가되는 괴이한 현상이 발생할 수 있음
                    if (!param_.Notifier.IsInterruptRequested)
                    {
                        ViewModel.View.Dispatcher.BeginInvoke(() =>
                        {
                            if (param_.Notifier.IsInterruptRequested)
                                return;

                            viewList_.AddRange(readElements);
                            OnPropertyChanged(nameof(ScrollableItemCount));
                        });
                    }
                }
                else
                {
                    if (param_.Notifier.IsInterruptRequested)
                        return _scrollLimit - leftReadCount;

                    ViewModel.View.Dispatcher.Invoke(() =>
                    {
                        viewList_.AddRange(readElements);
                        OnPropertyChanged(nameof(ScrollableItemCount));
                    });
                }
            }

            return _scrollLimit - leftReadCount;
        }

        public void UpdateScrollableItemCount()
        {
            if (bufferedList_.ContainsKey(cursor_) == false)
            {
                _scrollableItemCount = 0;
                return;
            }

            int itemCount = 0;
            int cursor = cursor_;

            for (; cursor <= cursorLast_; ++cursor)
            {
                SegmentResult segResult = bufferedList_[cursor];
                Debug.Assert(segResult.Count > 0);
                itemCount += segResult.ReadableCount;
            }

            _scrollableItemCount = itemCount;
        }

        public class SegmentResult
        {
            public int Id;                  // 몇번째로 Add 되었는지 (1이 제일 처음 읽음)
            public int ReadingOffset;       // 어디서부터 읽을 차례인지(인덱스)
            public List<T> Result;

            public int ReadableCount => Result.Count - ReadingOffset;
            public int Count => Result.Count;
        }
    }
}

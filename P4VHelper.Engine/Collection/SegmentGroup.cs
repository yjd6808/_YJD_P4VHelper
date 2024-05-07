using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4VHelper.Base.Extension;
using P4VHelper.Engine.Cache;
using P4VHelper.Engine.Model;
using P4VHelper.Engine.Param;

using NativeChangelist = Perforce.P4.Changelist;

namespace P4VHelper.Engine.Collection
{
    public class SegmentGroup
    {
        private readonly List<Segment> elements_;
        private readonly TimeCache timeCache_;

        public SegmentType Type => Config.Type;
        public P4VConfig.SegmentGroup Config { get; }
        public string DirPath => $"segments/{Type.ToString().ToLower()}/{Config.Alias}";
        public string FilePath => $"segments/{Type.ToString().ToLower()}.ini";
        public int Id { get; }
        public SegmentIo Io { get; }
        public int Count => elements_.Count;

        public SegmentGroup(P4VConfig.SegmentGroup _config)
        {
            Config = _config;
            Id = _config.Id;
            Io = SegmentIo.Create(_config.Type, this);

            elements_ = new List<Segment>(64);
            timeCache_ = new TimeCache();
        }

        public void Ready(int _count)
        {
            // 이제 사용안함.
            int readyCount = _count - elements_.Count;    // 추가해줘야하는 세그먼트 수
            int segCount = elements_.Count;               // 기존 세그먼트 수

            for (int i = 0; i < readyCount; ++i)
            {
                int segId = i + segCount;
                var seg = new Segment(segId, this);
                elements_.Add(seg);
            }
        }

        public void Clear()
        {
            foreach (Segment element in elements_)
            {
                element.Clear();
            }
        }

        // 최신 체인지리스트
        public async Task<NativeChangelist> UpdateAndGetLastChangeList()
        {
            NativeChangelist changelist;
            if (timeCache_.Elapsed(Id))
            {
                changelist = await API.P4.GetLastChangelistAsync(Config.Path);
                timeCache_.Set(Id, changelist);
            }
            else
            {
                changelist = timeCache_.Get<NativeChangelist>(Id);
            }

            return changelist;
        }

        public async void Load(LoadParam _param)
        {
            // TODO: 다른 체인지리스트 타입 로딩
            if (_param.Type != SegmentType.Changelist)
            {
                return;
            }

            // TODO: 패럴 로드
            NativeChangelist lastChangelist = await UpdateAndGetLastChangeList();

            int totalRevCount = lastChangelist.Id;
            int totalSegmentCount = (totalRevCount - 1) / Config.SegmentSize + 1;

            Ready(totalSegmentCount);

            if (totalRevCount <= 0)
            {
                _param.Notifier.Progress();
                return;
            }

            int loadCount = _param.LoadCount;
            int availableLoadCount = loadCount > totalSegmentCount ? totalSegmentCount : totalSegmentCount - loadCount;

            int startSegmentId = totalSegmentCount - 1;
            int endSegmentid = loadCount > totalSegmentCount ? 0 : availableLoadCount;

            int leftSaveCount = _param.Save ? availableLoadCount : 0;
            int leftCacheCount = loadCount < Config.CachedSegmentCount ? loadCount : Config.CachedSegmentCount;

            int curCachedSegmentCount = 0;

            _param.Notifier.Start(leftSaveCount + leftCacheCount);

            for (int segId = startSegmentId; segId >= endSegmentid; --segId)
            {
                Segment? seg = elements_[segId];

                if (seg == null)
                    throw new Exception($"{segId} 세그먼트가 존재하지 않습니다");

                seg.Ready(Config.SegmentSize);
                try
                {
                    if (curCachedSegmentCount < Config.CachedSegmentCount)
                    {
                        seg.Load(_param.LoadArgs);
                        _param.Notifier.Progress();
                        curCachedSegmentCount++;
                    }

                    if (_param.Save)
                    {
                        seg.Save(_param.SaveArgs);
                        _param.Notifier.Progress();
                    }
                }
                catch (Exception e)
                {
                    _param.NotifyException(seg, e);
                }
            }
        }

        public void Search(SearchParam _param)
        {
            // TODO: 패럴 서치
            _param.Start(elements_.Count);

            foreach (Segment seg in elements_.ReverseEx())
            {
                bool limitOver = seg.Search(_param);

                if (limitOver)
                    break;
            }
        }

        public Segment At(int _id)
        {
            return elements_[_id];
        }

        public async Task<bool> IsInDisk()
        {
            NativeChangelist lastChangelist = await UpdateAndGetLastChangeList();

            int totalRevCount = lastChangelist.Id;
            int totalSegmentCount = (totalRevCount - 1) / Config.SegmentSize + 1;

            Ready(totalSegmentCount);

            for (int segId = totalSegmentCount - 1; segId >= 0; --segId)
            {
                Segment? seg = elements_[segId];

                if (seg == null)
                    throw new Exception($"{segId} 세그먼트가 존재하지 않습니다");

                seg.Ready(Config.SegmentSize);
                if (seg.State == SegmentState.None)
                {
                    return false;
                }
            }

            return true;
        }
    }
}

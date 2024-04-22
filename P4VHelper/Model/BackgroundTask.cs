// jdyun 24/04/10(수)
// 컨텐츠 공용 백그라운드 테스크
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Threading;
using Accessibility;
using P4VHelper.Base;
using P4VHelper.Base.Extension;
using P4VHelper.Base.Notifier;
using P4VHelper.Model.TaskList;
using P4VHelper.View;

namespace P4VHelper.Model
{
    public enum BackgroundTaskState
    {
        None,
        Waiting,
        Paused,
        Running,
        InterruptRequested, // Running 상태인데 Interrupt를 요청받은 상황
        Interrupted,
        Finished
    }

    public abstract class BackgroundTask : Bindable, IProgressListener
    {
        /// <summary>
        /// 자세히 보기가 눌려져있는지
        /// </summary>
        protected bool _viewDetail;

        /// <summary>
        /// 무조건 백그라운드 스레드에서만 write를 수행
        /// </summary>
        protected BackgroundTaskState _state;

        /// <summary>
        /// UI 쓰레드 디스패쳐
        /// </summary>
        protected Dispatcher _dispatcher;

        /// <summary>
        /// 작업 이름
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// 자세히 보기가 가능한 작업
        /// </summary>
        public abstract bool HasDetailView { get; }

        /// <summary>
        /// 진행도(0 ~ 100%) 알림기
        /// </summary>
        public ProgressNotifer Notifier { get; protected set; }

        /// <summary>
        /// 실행중인 상태인가
        /// </summary>
        public bool IsRunning => _state == BackgroundTaskState.Running;

        /// <summary>
        /// 중단이 요청된 상태인가
        /// </summary>
        public bool IsInterruptRequested => _state == BackgroundTaskState.InterruptRequested;

        /// <summary>
        /// 상태표시줄에 표시되는 녀석인가?
        /// </summary>
        public bool IsTargeted => Mgr.TargetedTask == this;

        /// <summary>
        /// UI 싱크 알림을 줘야하는 상태인가?
        /// 상패표시줄에 타게팅 되었거나, 작업표시줄에 자세히보기를 누른 경우
        /// </summary>
        public bool IsNotificationRequired => IsTargeted || Mgr.ViewDetail;

        /// <summary>
        /// 2번째 프로그래스 유닛이 있는 경우
        /// </summary>
        public bool HasSecondProgressUnit => Notifier.Count > 1;

        public BackgroundTaskState State => _state;
        public bool ViewDetail
        {
            get => _viewDetail;
            set
            {
                _viewDetail = value;
                _OnViewDetailChanged(value);
            }
        }

        public BackgroundTaskMgr Mgr { get; }
        public BackgroundTask(BackgroundTaskMgr mgr)
        {
            Mgr = mgr;

            _state = BackgroundTaskState.None;
            _viewDetail = false;
            _dispatcher = mgr.Dispatcher;
        }

        public abstract void Execute();

        public void _OnInterruptRequested()
        {
            _state = BackgroundTaskState.InterruptRequested;
            OnInterruptRequested();

            _dispatcher.BeginInvoke(() =>
            {
                if (IsNotificationRequired)
                {
                    OnPropertyChanged(nameof(State));
                    OnPropertyChanged(nameof(IsRunning));
                }

                OnInterruptRequestedDispatched();
            });
        }

        public void _OnBegin()
        {
            _state = BackgroundTaskState.Running;
            OnBegin();

            _dispatcher.BeginInvoke(() =>
            {
                if (IsNotificationRequired)
                {
                    OnPropertyChanged(nameof(State));
                    OnPropertyChanged(nameof(IsRunning));
                }

                OnBeginDispatched();
            });

            Execute();

            if (IsInterruptRequested)
                _OnInterrupted();
        }

        public void _OnInterrupted()
        {
            _state = BackgroundTaskState.Interrupted;

            OnInterrupted();
            _dispatcher.BeginInvoke(() =>
            {
                if (IsNotificationRequired)
                {
                    OnPropertyChanged(nameof(State));
                    OnPropertyChanged(nameof(IsRunning));
                }

                OnInterruptedDispatched();
            });
        }

        public void _OnWaiting()
        {
            _state = BackgroundTaskState.Waiting;

            OnWaiting();
            _dispatcher.BeginInvoke(() =>
            {
                if (IsNotificationRequired)
                {
                    OnPropertyChanged(nameof(State));
                    OnPropertyChanged(nameof(IsRunning));
                }

                OnWaitingDispatched();
            });
        }

        public void _OnTargeted()
        {
            OnTargeted();
            _dispatcher.BeginInvoke(() =>
            {
                if (IsNotificationRequired)
                {
                    OnPropertyChanged(nameof(State));
                    OnPropertyChanged(nameof(IsRunning));
                    OnPropertyChanged(nameof(Name));
                    Notifier.First.NotifyProperty("Percent");
                    Notifier.First.NotifyProperty("ProgressText");

                    if (HasSecondProgressUnit)
                    {
                        Notifier.Second.NotifyProperty("Percent");
                        Notifier.Second.NotifyProperty("ProgressText");
                    }
                }

                OnTargetedDispatched();
            });
        }

        public void _OnEnd()
        {
            _state = BackgroundTaskState.Finished;

            OnEnd();
            _dispatcher.BeginInvoke(() =>
            {
                if (IsNotificationRequired)
                {
                    OnPropertyChanged(nameof(State));
                    OnPropertyChanged(nameof(IsRunning));
                }

                OnEndDispatched();
            });
        }

        public void _OnPaused()
        {
            throw new NotImplementedException();
        }

        public void _OnViewDetailChanged(bool changed)
        {
            OnViewDetailChanged(changed);
            _dispatcher.BeginInvoke(() =>
            {
                OnViewDetailChangedDispatched(changed);
            });
        }

        public void _OnReported(int cur)
        {
            OnReported(cur);
            _dispatcher.BeginInvoke(() =>
            {
                if (IsNotificationRequired)
                {
                    Notifier.First.NotifyProperty("Percent");
                    Notifier.First.NotifyProperty("ProgressText");

                    if (HasSecondProgressUnit)
                    {
                        Notifier.Second.NotifyProperty("Percent");
                        Notifier.Second.NotifyProperty("ProgressText");
                    }
                }
                OnReportedDispatched(cur);
            });
        }

        // 핸들러로 빼야하나 고민중인데 일단 이렇게해보고..
        protected virtual void OnBegin() {}
        protected virtual void OnBeginDispatched() {}
        protected virtual void OnWaiting() { }
        protected virtual void OnWaitingDispatched() { }
        protected virtual void OnInterruptRequested() { }
        protected virtual void OnInterruptRequestedDispatched() { }
        protected virtual void OnInterrupted() { }
        protected virtual void OnInterruptedDispatched() { }
        protected virtual void OnTargeted() { }
        protected virtual void OnTargetedDispatched() { }
        protected virtual void OnEnd() {}
        protected virtual void OnEndDispatched() {}
        protected virtual void OnViewDetailChanged(bool changed) {}
        protected virtual void OnViewDetailChangedDispatched(bool changed) { }
        protected virtual void OnReported(int cur) { }
        protected virtual void OnReportedDispatched(int cur) { }
    }
}

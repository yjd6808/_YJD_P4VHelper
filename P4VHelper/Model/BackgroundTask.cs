// jdyun 24/04/10(수)
// 컨텐츠 공용 백그라운드 테스크
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows.Automation;
using Accessibility;
using P4VHelper.Base;
using P4VHelper.Base.Extension;
using P4VHelper.Base.Util;
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
        protected bool _viewDetail;
        protected BackgroundTaskState _state = BackgroundTaskState.None;      // 무조건 백그라운드 스레드에서 write가 수행됨

        public abstract string Name { get; }                                // 작업 이름
        public abstract bool HasDetailView { get; }                         // 자세히 보기가 가능한 테스크인가?
        public ProgressNotifer Notifier { get; protected set; }             // Progress 알림기
        public bool IsRunning => _state == BackgroundTaskState.Running;     // 현재 실행중인가?
        public bool IsTargeted => Mgr.TargetedTask == this;                 // 상태표시줄에 표시되는 녀석인가?
        public bool IsInterruptRequested => _state == BackgroundTaskState.InterruptRequested;

        public string ProgressText => $"{Notifier.Cur} / {Notifier.Max} ({Notifier.Percent.ToString("0.00")}%)";
        public double Percent => Notifier.Percent;
        

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
        }

        public abstract void Execute();

        public void _OnInterruptRequested()
        {
            _state = BackgroundTaskState.InterruptRequested;

            OnInterruptRequested();
            Mgr.Dispatcher.BeginInvoke(() =>
            {
                OnPropertyChanged(nameof(State));
                OnPropertyChanged(nameof(IsRunning));

                OnInterruptRequestedDispatched();
            });
        }

        public void _OnBegin()
        {
            _state = BackgroundTaskState.Running;

            OnBegin();
            Mgr.Dispatcher.BeginInvoke(() =>
            {
                OnPropertyChanged(nameof(State));
                OnPropertyChanged(nameof(IsRunning));

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
            Mgr.Dispatcher.BeginInvoke(() =>
            {
                OnPropertyChanged(nameof(State));
                OnPropertyChanged(nameof(IsRunning));

                OnInterruptedDispatched();
            });
        }

        public void _OnWaiting()
        {
            _state = BackgroundTaskState.Waiting;

            OnWaiting();
            Mgr.Dispatcher.BeginInvoke(() =>
            {
                OnPropertyChanged(nameof(State));
                OnPropertyChanged(nameof(IsRunning));

                OnWaitingDispatched();
            });
        }

        public void _OnTargeted()
        {
            OnTargeted();
            Mgr.Dispatcher.BeginInvoke(() =>
            {
                OnPropertyChanged(nameof(State));
                OnPropertyChanged(nameof(IsRunning));
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(Percent));
                OnPropertyChanged(nameof(ProgressText));

                OnTargetedDispatched();
            });
        }

        public void _OnEnd()
        {
            _state = BackgroundTaskState.Finished;

            OnEnd();
            Mgr.Dispatcher.BeginInvoke(() =>
            {
                OnPropertyChanged(nameof(State));
                OnPropertyChanged(nameof(IsRunning));

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
            Mgr.Dispatcher.BeginInvoke(() =>
            {
                OnViewDetailChangedDispatched(changed);
            });
        }

        public void _OnReported(int cur)
        {
            OnReported(cur);
            Mgr.Dispatcher.BeginInvoke(() =>
            {
                OnPropertyChanged(nameof(Percent));
                OnPropertyChanged(nameof(ProgressText));

                OnReportedDispatched(cur);
            });
        }

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

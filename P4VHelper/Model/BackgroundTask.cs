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
        Finished
    }

    public abstract class BackgroundTask : Bindable, IProgressListener
    {
        public static BackgroundTask Default = new Default(null);

        private bool _viewDetail;
        private BackgroundTaskState _state = BackgroundTaskState.None;
        private Action _action;

        public abstract int TotalSubtaskCount { get; }
        public abstract string Name { get; }
        public abstract bool HasDetailView { get; }
        public abstract ProgressNotifer Notifier { get; }

        public string Cur => Notifier.Cur.ToString();
        public string Max => Notifier.Max.ToString();
        public string Percent => Notifier.Percent.ToString("0.00");

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

        public abstract void Do();

        public void Execute()
        {
            int max = Notifier.Max;

            for (int i = 0; i < max; ++i)
            {
                Do();
                Notifier.Progress();
            }
        }

        public void _OnBegin()
        {
            _state = BackgroundTaskState.Running;

            OnBegin();
            Mgr.Dispatcher.BeginInvoke(() =>
            {
                OnPropertyChanged(nameof(State));
                OnBeginDispatched();
            });
        }

        public void _OnWaiting()
        {
            _state = BackgroundTaskState.Waiting;

            OnWaiting();
            Mgr.Dispatcher.BeginInvoke(() =>
            {
                OnPropertyChanged(nameof(State));
                OnWaitingDispatched();
            });
        }

        public void _OnTargeted()
        {
            OnTargeted();
            Mgr.Dispatcher.BeginInvoke(() =>
            {
                OnPropertyChanged(nameof(State));
                OnPropertyChanged(nameof(Cur));
                OnPropertyChanged(nameof(Max));
                OnPropertyChanged(nameof(Percent));

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
                OnReportedDispatched(cur);
            });
        }

        protected virtual void OnBegin() {}
        protected virtual void OnBeginDispatched() {}
        protected virtual void OnWaiting() { }
        protected virtual void OnWaitingDispatched() { }
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

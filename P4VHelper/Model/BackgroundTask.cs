// jdyun 24/04/10(수)
// 컨텐츠 공용 백그라운드 테스크
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Threading;
using Accessibility;
using P4VHelper.Base;
using P4VHelper.Base.Extension;
using P4VHelper.Base.Notifier;
using P4VHelper.Model.TaskList;
using P4VHelper.View;
using P4VHelper.ViewModel;

namespace P4VHelper.Model
{
    public enum BackgroundTaskState
    {
        None,
        Waiting,
        Paused,
        Running,
        Error,              // Exception이 발생한 경우(오류)
        InterruptRequested, // Running 상태인데 Interrupt를 요청받은 상황
        Interrupted,
        Finished
    }

    public abstract class BackgroundTask : Bindable, IProgressListener
    {
        /// <summary>
        /// 자세히 보기가 눌려져있는지
        /// </summary>
        protected bool viewDetail_;

        /// <summary>
        /// 무조건 백그라운드 스레드에서만 write를 수행
        /// </summary>
        protected BackgroundTaskState state_;

        /// <summary>
        /// 태스크 타입 이름
        /// </summary>
        public string TypeName => GetType().Name;

        /// <summary>
        /// 태스크 작업 분류
        /// </summary>
        public virtual string ClassId => TypeName;

        /// <summary>
        /// 작업 설명
        /// </summary>
        public abstract string Description { get; }

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
        public bool IsRunning => state_ == BackgroundTaskState.Running;

        /// <summary>
        /// 오류가 발생했는가?
        /// </summary>
        public bool IsError => state_ == BackgroundTaskState.Error;

        /// <summary>
        /// 중단이 요청된 상태인가
        /// </summary>
        public bool IsInterruptRequested => state_ == BackgroundTaskState.InterruptRequested;

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

        public BackgroundTaskState State => state_;
        public bool ViewDetail
        {
            get => viewDetail_;
            set
            {
                viewDetail_ = value;
                _OnViewDetailChanged(value);
            }
        }

        public BackgroundTaskMgr Mgr => BackgroundTaskMgr.Instance;
        public Dispatcher Dispatcher => Mgr.Dispatcher;

        public BackgroundTask()
        {
            state_ = BackgroundTaskState.None;
            viewDetail_ = false;
        }

        public Exception? __Execute()
        {

            try
            {
                Execute();
                return null;
            }
            catch (ProgressNotifer.InterruptException e)
            {
                state_ = BackgroundTaskState.InterruptRequested;
                return null;
            }
#if !DEBUG
            catch (Exception e)
            {
                state_ = BackgroundTaskState.Error;
                return e;
            }
#endif
        }

        public abstract void Execute();

        public void _OnError(Exception _e)
        {
            state_ = BackgroundTaskState.Error;
            OnError(_e);

            Dispatcher.BeginInvoke(() =>
            {
                if (IsNotificationRequired)
                {
                    OnPropertyChanged(nameof(State));
                    OnPropertyChanged(nameof(IsRunning));
                }

                OnErrorDispatched(_e);
            });
        }

        public void _OnInterruptRequested()
        {
            state_ = BackgroundTaskState.InterruptRequested;
            Notifier.IsInterruptRequested = true;

            OnInterruptRequested();

            Dispatcher.BeginInvoke(() =>
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
            state_ = BackgroundTaskState.Running;
            OnBegin();

            Dispatcher.BeginInvoke(() =>
            {
                if (IsNotificationRequired)
                {
                    OnPropertyChanged(nameof(State));
                    OnPropertyChanged(nameof(IsRunning));
                }

                OnBeginDispatched();
            });

            Exception? e = __Execute();

            if (IsError)
                _OnError(e);
            else if (IsInterruptRequested)
                _OnInterrupted();
        }

        public void _OnInterrupted()
        {
            state_ = BackgroundTaskState.Interrupted;

            OnInterrupted();
            Dispatcher.BeginInvoke(() =>
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
            state_ = BackgroundTaskState.Waiting;

            OnWaiting();
            Dispatcher.BeginInvoke(() =>
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
            Dispatcher.BeginInvoke(() =>
            {
                if (IsNotificationRequired)
                {
                    OnPropertyChanged(nameof(State));
                    OnPropertyChanged(nameof(IsRunning));
                    OnPropertyChanged(nameof(Description));
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
            if (IsRunning)
            {
                state_ = BackgroundTaskState.Finished;
            }

            OnEnd();
            Dispatcher.BeginInvoke(() =>
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

        public void _OnViewDetailChanged(bool _changed)
        {
            OnViewDetailChanged(_changed);
            Dispatcher.BeginInvoke(() =>
            {
                OnViewDetailChangedDispatched(_changed);
            });
        }

        public void _OnReported(int _cur)
        {
            OnReported(_cur);
            Dispatcher.BeginInvoke(() =>
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
                OnReportedDispatched(_cur);
            });
        }

        // 핸들러로 빼야하나 고민중인데 일단 이렇게해보고..
        protected virtual void OnBegin() {}
        protected virtual void OnBeginDispatched() {}
        protected virtual void OnWaiting() { }
        protected virtual void OnWaitingDispatched() { }
        protected virtual void OnError(Exception _e) { }

        protected virtual void OnErrorDispatched(Exception _e)
        {
            Mgr.ViewModel.Logger?.WriteDebug($"{_e} 오류 발생");
        }
        protected virtual void OnInterruptRequested() { }
        protected virtual void OnInterruptRequestedDispatched() { }
        protected virtual void OnInterrupted() { }
        protected virtual void OnInterruptedDispatched() { }
        protected virtual void OnTargeted() { }
        protected virtual void OnTargetedDispatched() { }
        protected virtual void OnEnd() {}

        protected virtual void OnEndDispatched()
        {
            if (state_ == BackgroundTaskState.Finished)
            {
                Mgr.ViewModel.Logger?.WriteDebug($"{Description} 작업 완료");
            }
        }
        protected virtual void OnViewDetailChanged(bool _changed) {}
        protected virtual void OnViewDetailChangedDispatched(bool _changed) { }
        protected virtual void OnReported(int _cur) { }
        protected virtual void OnReportedDispatched(int _cur) { }
    }
}

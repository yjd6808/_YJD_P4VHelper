// jdyun 24/04/10(수)
// 컨텐츠 공용 백그라운드 테스크
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accessibility;
using P4VHelper.Base;
using P4VHelper.Base.Extension;

namespace P4VHelper.Model
{
    public class BackgroundTask : Bindable
    {
        public static BackgroundTask Default = new (null, "작업 없음", 0, () => {});

        private int _completedSubtaskCount;
        private Action _action;

        public int CompletedSubtaskCount => InterlockedEx.Get(ref _completedSubtaskCount);
        public int TotalSubtaskCount { get; }
        public bool Completed => CompletedSubtaskCount >= TotalSubtaskCount;
        public float CompletedPercent => (float)CompletedSubtaskCount / TotalSubtaskCount * 100.0f;
        public string Name { get; }
        public Action Action => _action;
        public BackgroundTaskMgr Mgr { get; }

        public BackgroundTask(BackgroundTaskMgr mgr, string name, int totalSubtaskCount, Action action)
        {
            _completedSubtaskCount = 0;
            _action = action;

            TotalSubtaskCount = totalSubtaskCount;
            Name = name;
            Mgr = mgr;
        }

        public void Execute()
        {
            _action();
        }

        public virtual void OnBegin()
        {

        }

        public virtual void OnEnd()
        {

        }
    }
}

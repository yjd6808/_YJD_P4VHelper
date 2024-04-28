// jdyun 24/04/13(토)
// 간단한 조건 변수를 구현해보아요
// @참고: https://stackoverflow.com/questions/15657637/condition-variables-c-net
// @참고: https://learn.microsoft.com/en-us/dotnet/api/system.threading.monitor.wait?view=net-8.0
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace P4VHelper.Base
{
    public enum CvResult
    {
        Timeout,    // 타이암웃
        None,       // 타임아웃 이전에 반환
    }

    public class Cv
    {
        public CvResult Wait(object _locker, Func<bool> _predicate)
        {
            Debug.Assert(Monitor.IsEntered(_locker));
            return Wait(_locker, _predicate, Timeout.InfiniteTimeSpan);
        }

        public CvResult Wait(object _locker, Func<bool> _predicate, int _timeout)
        {
            Debug.Assert(Monitor.IsEntered(_locker));
            return Wait(_locker, _predicate, new TimeSpan(0, 0, 0, 0, _timeout));
        }

        public CvResult Wait(object _locker, Func<bool> _predicate, TimeSpan _timeout)
        {
            Debug.Assert(Monitor.IsEntered(_locker)); 
            CvResult result = CvResult.None;
            while (!_predicate())
            {
                bool isLockAcquiredBeforeTimeout = Monitor.Wait(_locker, _timeout);
                if (!isLockAcquiredBeforeTimeout)
                {
                    result = CvResult.Timeout;
                    break; // 타임아웃인 경우는 조건 무시해야겠지?
                }
            }
            return result;
        }

        public void NotifyOne(object _locker)
        {
            Debug.Assert(Monitor.IsEntered(_locker));
            Monitor.Pulse(_locker);
        }

        public void NotifyAll(object _locker)
        {
            Debug.Assert(Monitor.IsEntered(_locker));
            Monitor.PulseAll(_locker);
        }
    }
}

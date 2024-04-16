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
    public enum CVResult
    {
        Timeout,    // 타이암웃
        None,       // 타임아웃 이전에 반환
    }

    public class CV
    {
        public CVResult Wait(object locker, Func<bool> predicate)
        {
            Debug.Assert(Monitor.IsEntered(locker));
            return Wait(locker, predicate, Timeout.InfiniteTimeSpan);
        }

        public CVResult Wait(object locker, Func<bool> predicate, int timeout)
        {
            Debug.Assert(Monitor.IsEntered(locker));
            return Wait(locker, predicate, new TimeSpan(0, 0, 0, 0, timeout));
        }

        public CVResult Wait(object locker, Func<bool> predicate, TimeSpan timeout)
        {
            Debug.Assert(Monitor.IsEntered(locker)); 
            CVResult result = CVResult.None;
            while (!predicate())
            {
                bool isLockAcquiredBeforeTimeout = Monitor.Wait(locker, timeout);
                if (!isLockAcquiredBeforeTimeout)
                {
                    result = CVResult.Timeout;
                }
            }
            return result;
        }

        public void NotifyOne(object locker)
        {
            Debug.Assert(Monitor.IsEntered(locker));
            Monitor.Pulse(locker);
        }

        public void NotifyAll(object locker)
        {
            Debug.Assert(Monitor.IsEntered(locker));
            Monitor.PulseAll(locker);
        }
    }
}

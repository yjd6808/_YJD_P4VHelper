using P4VHelper.Base.Extension;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace P4VHelper.Base.Notifier
{
    public abstract class ProgressUnit : Bindable
    {
        public static readonly ProgressUnit Default = Factory.CreateEach(null, 0);

        public enum Type
        {
            Each,           // 각 작업마다 알림
            Percent,        // 퍼센트 단위마다 알림
        }

        protected ProgressNotifer? _notifier;
        protected int _slot;                        // 노티파이어의 몇번째 슬롯(인덱스)의 유닛인지
        protected int _notified;                    // 마지막 알림처리된 인덱스
        protected int _cur;                         // 수행해야할 인덱스
        protected int _max;                         // 마지막 인덱스
        protected Stopwatch? _stopwatch;             // 인터벌이 있는 경우 시간계산 용도
        protected TimeSpan _reportInterval;         // 인터벌이 지난 경우 알림
        protected TimeSpan _lastReportedElapsed;    // 작업시작 마지막 작업을 수행했을 때의 경과 시작(ReportInterval을 사용하는 경우만)

        public int Notified
        {
            get => _notified;
            set => _notified = value;
        }

        public int Cur
        {
            get => _cur;
            set => _cur = value;
        }

        public int Max
        {
            get => _max;
            set => _max = value;
        }

        public TimeSpan ReportInterval
        {
            get => _reportInterval;
            set => _reportInterval = value;
        }

        public double Percent => _max > 0 ? (double)_cur / _max * 100.0f : 0.0f;
        public string ProgressText => $"{_cur} / {_max} ({Percent.ToString("0.00")}%)";

        public abstract void Progress();
        public abstract void OnStarted();

        public void Start(int max)
        {
            _max = max;

            if (_stopwatch is not null)
                _stopwatch.Start();

            OnStarted();
        }

        public static class Factory
        {
            public static ProgressUnit CreateEach(ProgressNotifer notifier, int slot)
            {
                var each = new _Each();
                each._notifier = notifier;
                each._slot = slot;
                return each;
            }

            public static ProgressUnit CreateEach(ProgressNotifer notifier, int slot, TimeSpan reportInterval)
            {
                var each = new _Each();
                each._notifier = notifier;
                each._slot = slot;
                each._reportInterval = reportInterval;
                each._stopwatch = new Stopwatch();
                return each;
            }

            public static ProgressUnit CreatePercent(ProgressNotifer notifier, int slot, float percent)
            {
                var per = new _Percent(percent);
                per._notifier = notifier;
                per._slot = slot;
                return per;
            }

            public static ProgressUnit CreatePercent(ProgressNotifer notifier, int slot, float percent, TimeSpan reportInterval)
            {
                var per = new _Percent(percent);
                per._notifier = notifier;
                per._slot = slot;
                per._reportInterval = reportInterval;
                per._stopwatch = new Stopwatch();
                return per;
            }
        }

        public class _Each : ProgressUnit
        {
            public override void OnStarted()
            {
            }

            public override void Progress()
            {
                Debug.Assert(_max > 0, "작업량(max)가 설정되지 않았습니다.");
                Debug.Assert(_cur < _max, "Progress를 정해진 작업량(max)보다 더 많이 실행하였습니다");
                _cur++;

                if (_reportInterval == TimeoutEx.InfiniteSpan)
                {
                    _notifier?.Report(_slot);
                }
                else
                {
                    Debug.Assert(_stopwatch is not null);
                    TimeSpan elapsed = _stopwatch.Elapsed;

                    if (elapsed >= _lastReportedElapsed + _reportInterval)
                    {
                        _notifier?.Report(_slot);
                        _lastReportedElapsed = elapsed;
                    }
                }
            }
        }

        public class _Percent : ProgressUnit
        {
            private float _percent;
            private float _quantity;
            private float _nextQuantity;	// 알람을 줄 다음 스탭 갯수

            public _Percent(float percent)
            {
                _percent = percent;
            }

            public override void OnStarted()
            {
                Debug.Assert(_percent >= 0.1f, "알림 퍼센트가 0.1%보다 작으면 안됩니다.");
                _quantity = _max * _percent * 0.01f;
                _nextQuantity = _quantity;
            }

            private void CalcNextQuantity()
            {
                if (_cur <= _nextQuantity)
                    return;

                _notifier?.Report(_slot);
                _nextQuantity += _quantity;
            }

            public override void Progress()
            {
                Debug.Assert(_max > 0, "작업량(max)가 설정되지 않았습니다.");
                Debug.Assert(_cur < _max, "Progress를 정해진 작업량(max)보다 더 많이 실행하였습니다");
                _cur++;

                if (_cur == _max)
                {
                    _notifier?.Report(_slot);
                    return;
                }

                if (_reportInterval == TimeoutEx.InfiniteSpan)
                {
                    CalcNextQuantity();
                }
                else
                {
                    Debug.Assert(_stopwatch is not null);
                    TimeSpan elapsed = _stopwatch.Elapsed;

                    if (elapsed >= _lastReportedElapsed + _reportInterval)
                    {
                        _notifier?.Report(_slot);
                        _lastReportedElapsed = elapsed;
                        return;
                    }

                    CalcNextQuantity();
                }
            }
        }
    }
}

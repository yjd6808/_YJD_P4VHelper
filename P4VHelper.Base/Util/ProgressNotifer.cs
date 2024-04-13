// jdyun 24/04/13(토)
// BackgroundTask 알림 방식을 다양화하기 위해서 만듬
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using P4VHelper.Base.Extension;

namespace P4VHelper.Base.Util
{
    public abstract class ProgressNotifer
    {
        public enum Type
        {
            Each,           // 각 작업마다 알림
            Percent,        // 퍼센트 단위마다 알림
        }

        private IProgressListener _listener;
        protected int _cur;
        protected int _max;
        protected Stopwatch? _stopwatch;
        protected TimeSpan _reportInterval;    // 인터벌이 지난 경우 알림
        protected TimeSpan _lastProgressed;    // 작업시작 마지막 작업을 수행했을 때의 경과 시작(ReportInterval을 사용하는 경우만)

        public int Cur => _cur;
        public int Max => _max;
        public float Percent => (float)_cur / _max * 100.0f;
        public TimeSpan ReportInterval => _reportInterval;

        public ProgressNotifer(IProgressListener listener, int max)
        {
            _listener = listener;
            _cur = 0;
            _max = max;
            _reportInterval = TimeoutEx.InfiniteSpan;
            _stopwatch = null;
        }

        public ProgressNotifer(IProgressListener listener, int max, TimeSpan reportInterval)
        {
            _listener = listener;
            _cur = 0;
            _max = max;
            _reportInterval = reportInterval;
            _stopwatch = new Stopwatch();
        }

        public abstract void Progress();

        public void Start()
        {
            if (_stopwatch is not null)
                _stopwatch.Start();
        }

        public void Report()
        {
            Debug.Assert(_cur <= _max);
            _listener._OnReported(_cur);

            // if (_cur == _max)
            // _listener._OnFinished();
        }
    }

    public class EachProgressNotifier : ProgressNotifer
    {
        public EachProgressNotifier(IProgressListener listener, int max) 
            : base(listener, max)  {}
        public EachProgressNotifier(IProgressListener listener, int max, TimeSpan reportInterval) 
            : base(listener, max, reportInterval)  {}

        public override void Progress()
        {
            Debug.Assert(_cur < _max);
            _cur++;

            if (_reportInterval == TimeoutEx.InfiniteSpan)
            {
                Report();
            }
            else
            {
                Debug.Assert(_stopwatch is not null);
                TimeSpan elapsed = _stopwatch.Elapsed;

                if (elapsed >= _lastProgressed + _reportInterval)
                    Report();

                _lastProgressed = elapsed;
            }
        }
    }

    public class PercentProgressNotifier : ProgressNotifer
    {
        private float _percent;
        private float _quantity;
        private float _nextQuantity;	// 알람을 줄 다음 스탭 갯수

        public PercentProgressNotifier(IProgressListener listener, int max, float percent)
            : base(listener, max)
        {
            _percent = percent;

            Init();
        }

        public PercentProgressNotifier(IProgressListener listener, int max, float percent, TimeSpan reportInterval)
            : base(listener, max, reportInterval)
        {
            _percent = percent;

            Init();
        }

        private void Init()
        {
            // ex) 30개 -> 1% -> 0.3개마다 알림
            Debug.Assert(_percent >= 0.1f, "알림 퍼센트가 0.1%보다 작으면 안됩니다.");
            _quantity = _max * _percent * 0.01f;
            _nextQuantity = _quantity;
        }

        private void ProcessLogic()
        {
            if (_cur <= _nextQuantity)
                return;

            Report();
            _nextQuantity += _quantity;
        }

        public override void Progress()
        {
            Debug.Assert(_cur < _max);
            _cur++;

            if (_cur == _max)
            {
                Report();
                return;
            }

            if (_reportInterval == TimeoutEx.InfiniteSpan)
            {
                ProcessLogic();
            }
            else
            {
                Debug.Assert(_stopwatch is not null);
                TimeSpan elapsed = _stopwatch.Elapsed;

                if (elapsed >= _lastProgressed + _reportInterval)
                {
                    Report();
                    return;
                }

                ProcessLogic();
                _lastProgressed = elapsed;
            }
        }
    }
}

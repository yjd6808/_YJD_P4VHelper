// jdyun 24/04/13(토)
// BackgroundTask 알림 방식을 다양화하기 위해서 만듬
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.CompilerServices;
using P4VHelper.Base.Extension;

namespace P4VHelper.Base.Notifier
{
    public class ProgressNotifer : Bindable
    {
        private IProgressListener _listener;            // 리포트를 받은 대상
        private List<ProgressUnit> _units;              // 프로그래스 단위

        public int Count => _units.Count;

        public ProgressUnit First
        {
            get
            {
                if (Count >= 1)
                    return _units[0];

                return ProgressUnit.Default;
            }
        }

        public ProgressUnit Second
        {
            get
            {
                if (Count >= 2)
                    return _units[1];

                return ProgressUnit.Default;
            }
        }

        public ProgressUnit Third
        {
            get
            {
                if (Count >= 3)
                    return _units[2];

                return ProgressUnit.Default;
            }
        }

        public ProgressUnit this[int slot]
        {
            get
            {
                if (slot < 0 || slot >= _units.Count)
                    return ProgressUnit.Default;

                return _units[slot];
            }
        }

        public ProgressNotifer(IProgressListener listener)
        {
            _listener = listener;
            _units = new List<ProgressUnit>();
        }

        public void AddEach(int reportElapsedMs = 200)
        {
            ProgressUnit unit = ProgressUnit.Factory.CreateEach(
                this,
                _units.Count,
                TimeSpan.FromMilliseconds(reportElapsedMs)
            );

            _units.Add(unit);
        }

        public void AddPercentUnit(float percent = 2.0f, int reportElapsedMs = 200)
        {
            ProgressUnit unit = ProgressUnit.Factory.CreatePercent(
                this,
                _units.Count,
                percent,
                TimeSpan.FromMicroseconds(reportElapsedMs)
            );

            _units.Add(unit);
        }

        public void Start(params int[] maxs)
        {
            if (maxs.Length != _units.Count)
                throw new ArgumentException("파라미터수와 유닛 수가 틀립니다.");

            for (int i = 0; i < maxs.Length; ++i)
                _units[i].Start(maxs[i]);
        }

        public void Progress(int slot)
        {
            _units[slot].Progress();
        }

        public void Report(int slot)
        {
            ProgressUnit unit = _units[slot];
            Debug.Assert(unit.Cur <= unit.Max);
            _listener._OnReported(slot);
            unit.Notified = unit.Cur;

            // if (_cur == _max)
            // _listener._OnFinished();
        }
    }
}

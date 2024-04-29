﻿// jdyun 24/04/13(토)
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
        private readonly IProgressListener listener_;            // 리포트를 받은 대상
        private readonly List<ProgressUnit> units_;              // 프로그래스 단위
        private int isInterruptRequested_;

        public int Count => units_.Count;
        public bool IsInterruptRequested
        {
            get => InterlockedEx.Bool.Get(ref isInterruptRequested_);
            set => InterlockedEx.Bool.Set(ref isInterruptRequested_, value);
        }

        public ProgressUnit First
        {
            get
            {
                if (Count >= 1)
                    return units_[0];

                return ProgressUnit.s_Default;
            }
        }

        public ProgressUnit Second
        {
            get
            {
                if (Count >= 2)
                    return units_[1];

                return ProgressUnit.s_Default;
            }
        }

        public ProgressUnit Third
        {
            get
            {
                if (Count >= 3)
                    return units_[2];

                return ProgressUnit.s_Default;
            }
        }

        public ProgressUnit this[int _slot]
        {
            get
            {
                if (_slot < 0 || _slot >= units_.Count)
                    return ProgressUnit.s_Default;

                return units_[_slot];
            }
        }

        public ProgressNotifer(IProgressListener _listener)
        {
            listener_ = _listener;
            units_ = new List<ProgressUnit>();
        }

        public void AddEach(int _reportElapsedMs = 50)
        {
            ProgressUnit unit = ProgressUnit.Factory.CreateEach(
                this,
                units_.Count,
                TimeSpan.FromMilliseconds(_reportElapsedMs)
            );

            units_.Add(unit);
        }

        public void AddPercentUnit(float _percent = 2.0f, int _reportElapsedMs = 50)
        {
            ProgressUnit unit = ProgressUnit.Factory.CreatePercent(
                this,
                units_.Count,
                _percent,
                TimeSpan.FromMicroseconds(_reportElapsedMs)
            );

            units_.Add(unit);
        }

        public void Start(params int[] _maxs)
        {
            if (IsInterruptRequested)
                throw new InterruptException();

            if (_maxs.Length != units_.Count)
                throw new ArgumentException("파라미터수와 유닛 수가 틀립니다.");

            for (int i = 0; i < _maxs.Length; ++i)
                units_[i].Start(_maxs[i]);
        }

        public void Progress(int _slot = 0, int _count = 1)
        {
            if (IsInterruptRequested)
                throw new InterruptException();

            units_[_slot].Progress(_count);
        }

        public void Report(int _slot)
        {
            ProgressUnit unit = units_[_slot];
            Debug.Assert(unit.Cur <= unit.Max);
            listener_._OnReported(_slot);
            unit.Notified = unit.Cur;

            // if (_cur == _max)
            // _listener._OnFinished();
        }

        public class InterruptException : Exception
        {
        }
    }
}

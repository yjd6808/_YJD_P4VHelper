﻿using P4VHelper.Base.Extension;
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
        public static readonly ProgressUnit s_Default = Factory.CreateEach(null, 0);

        public enum Type
        {
            Each,           // 각 작업마다 알림
            Percent,        // 퍼센트 단위마다 알림
        }

        protected ProgressNotifer? notifier_;
        protected int slot_;                        // 노티파이어의 몇번째 슬롯(인덱스)의 유닛인지
        protected int notified_;                    // 마지막 알림처리된 인덱스
        protected int cur_;                         // 수행해야할 인덱스
        protected int max_;                         // 마지막 인덱스
        protected Stopwatch? stopwatch_;             // 인터벌이 있는 경우 시간계산 용도
        protected TimeSpan reportInterval_;         // 인터벌이 지난 경우 알림
        protected TimeSpan lastReportedElapsed_;    // 작업시작 마지막 작업을 수행했을 때의 경과 시작(ReportInterval을 사용하는 경우만)

        public int Notified
        {
            get => notified_;
            set => notified_= value;
        }

        public int Cur
        {
            get => cur_;
            set => cur_ = value;
        }

        public int Max
        {
            get => max_;
            set => max_ = value;
        }

        public TimeSpan ReportInterval
        {
            get => reportInterval_;
            set => reportInterval_ = value;
        }

        public double Percent => max_ > 0 ? (double)cur_ / max_ * 100.0f : 0.0f;
        public string ProgressText => $"{cur_} / {max_} ({Percent.ToString("0.00")}%)";

        public abstract void Progress(int _count);
        public abstract void OnStarted();

        public void Start(int _max)
        {
            max_ = _max;

            if (stopwatch_ is not null)
                stopwatch_.Start();

            OnStarted();
        }

        public static class Factory
        {
            public static ProgressUnit CreateEach(ProgressNotifer _notifier, int _slot)
            {
                var each = new Each();
                each.notifier_ = _notifier;
                each.slot_ = _slot;
                return each;
            }

            public static ProgressUnit CreateEach(ProgressNotifer _notifier, int _slot, TimeSpan _reportInterval)
            {
                var each = new Each();
                each.notifier_ = _notifier;
                each.slot_ = _slot;
                each.reportInterval_ = _reportInterval;
                each.stopwatch_ = new Stopwatch();
                return each;
            }

            public static ProgressUnit CreatePercent(ProgressNotifer _notifier, int _slot, float _percent)
            {
                var per = new _Percent(_percent);
                per.notifier_ = _notifier;
                per.slot_ = _slot;
                return per;
            }

            public static ProgressUnit CreatePercent(ProgressNotifer _notifier, int _slot, float _percent, TimeSpan _reportInterval)
            {
                var per = new _Percent(_percent);
                per.notifier_ = _notifier;
                per.slot_ = _slot;
                per.reportInterval_ = _reportInterval;
                per.stopwatch_ = new Stopwatch();
                return per;
            }
        }

        public class Each : ProgressUnit
        {
            public override void OnStarted()
            {
            }

            public override void Progress(int _count)
            {
                Debug.Assert(max_ > 0, "작업량(max)가 설정되지 않았습니다.");
                Debug.Assert(cur_ < max_, "Progress를 정해진 작업량(max)보다 더 많이 실행하였습니다");
                cur_ += _count;

                if (reportInterval_ == TimeoutEx.s_InfiniteSpan)
                {
                    notifier_?.Report(slot_);
                }
                else
                {
                    Debug.Assert(stopwatch_ is not null);
                    TimeSpan elapsed = stopwatch_.Elapsed;

                    if (elapsed >= lastReportedElapsed_ + reportInterval_)
                    {
                        notifier_?.Report(slot_);
                        lastReportedElapsed_ = elapsed;
                    }
                }
            }
        }

        public class _Percent : ProgressUnit
        {
            private readonly float percent_;
            private float quantity_;
            private float nextQuantity_;	// 알람을 줄 다음 스탭 갯수

            public _Percent(float _percent)
            {
                percent_ = _percent;
            }

            public override void OnStarted()
            {
                Debug.Assert(percent_ >= 0.1f, "알림 퍼센트가 0.1%보다 작으면 안됩니다.");
                quantity_ = max_ * percent_ * 0.01f;
                nextQuantity_ = quantity_;
            }

            private void CalcNextQuantity()
            {
                if (cur_ <= nextQuantity_)
                    return;

                notifier_?.Report(slot_);
                nextQuantity_ += quantity_;
            }

            public override void Progress(int _count)
            {
                Debug.Assert(max_ > 0, "작업량(max)가 설정되지 않았습니다.");
                Debug.Assert(cur_< max_, "Progress를 정해진 작업량(max)보다 더 많이 실행하였습니다");
                cur_ += _count;

                if (cur_ == max_)
                {
                    notifier_?.Report(slot_);
                    return;
                }

                if (reportInterval_ == TimeoutEx.s_InfiniteSpan)
                {
                    CalcNextQuantity();
                }
                else
                {
                    Debug.Assert(stopwatch_ is not null);
                    TimeSpan elapsed = stopwatch_.Elapsed;

                    if (elapsed >= lastReportedElapsed_ + reportInterval_)
                    {
                        notifier_?.Report(slot_);
                        lastReportedElapsed_ = elapsed;
                        return;
                    }

                    CalcNextQuantity();
                }
            }
        }
    }
}

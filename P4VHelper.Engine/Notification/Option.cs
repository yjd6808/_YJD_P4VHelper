// jdyun 24/04/21(일)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4VHelper.Engine.Notification
{
    public struct Option
    {
        public static readonly Option Default = new ();

        public Option()
        {
            Unit = Unit.Segment;
            Delay = Timeout.Infinite;
        }

        /// <summary>
        /// 알림 단위
        /// </summary>
        public Unit Unit { get; set; }

        /// <summary>
        /// 알림 주기 (이 수치를 넘어서면 1개라도 찾은게 있다면 알림)
        /// </summary>
        public int Delay { get; set; }
    }
}

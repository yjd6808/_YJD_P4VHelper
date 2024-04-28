// jdyun 24/04/13(토)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4VHelper.Base.Notifier
{
    public interface IProgressListener
    {
        /// <summary>
        /// Notifier에 의해 slot 프로그래스 유닛으로부터 완료보고를 받음
        /// Notifier[slot].Cur 멤버에 접근하여 Report된 인덱스 정보를 얻으면 된다.
        /// </summary>
        void _OnReported(int _slot);

        /// <summary>
        /// Notifier에 의해 작업 완료가 보고됨
        /// </summary>
        // void _OnFinished();
    }
}

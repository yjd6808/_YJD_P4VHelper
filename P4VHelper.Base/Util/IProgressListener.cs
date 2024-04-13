// jdyun 24/04/13(토)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4VHelper.Base.Util
{
    public interface IProgressListener
    {
        /// <summary>
        /// Notifier에 의해 현재 작업 진행상태가 보고됨
        /// </summary>
        /// <param name="cur">완료한 작업 인덱스</param>
        void _OnReported(int cur);

        /// <summary>
        /// Notifier에 의해 작업 완료가 보고됨
        /// </summary>
        // void _OnFinished();
    }
}

using P4VHelper.Engine.Collection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4VHelper.Base.Notifier;

namespace P4VHelper.Engine.Param
{
    public class LoadParam
    {
        public SegmentType Type;
        public string Alias;
        public ProgressNotifer? Notifier;
        public int LoadCount = Int32.MaxValue;
        public bool Save = false;
        public LoadArgs? LoadArgs = null;
        public SaveArgs? SaveArgs = null;

        public void Validate()
        {
            if (Type == SegmentType.Max)
                throw new Exception("SeachParam.Input 설정안됨");

            if (string.IsNullOrEmpty(Alias))
                throw new Exception("SeachParam.Alias 설정안됨");
        }

        public void NotifyException(Segment _seg, Exception _e)
        {
            Handler?.Invoke(_seg, _e);
        }

        public delegate void ExceptionHandler(Segment _seg, Exception _e);
        public event ExceptionHandler Handler;
    }
}

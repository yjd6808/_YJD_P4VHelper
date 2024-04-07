// jdyun 24/04/06(토)
using System.Diagnostics;

namespace P4VHelper.Base.Logger
{
    public abstract class Logger
    {
        private Logger? _next;
        private int _id = 0;

        public abstract void Write(LogLevel level, string msg);

        protected void WriteChaining(LogLevel level, string msg)
        {
            Logger? cur = this;

            while (cur != null)
            {
                cur.Write(level, msg);
                cur = cur._next;
            }
        }

        public void Add(Logger logger)
        {
            if (logger._id == _id)
                throw new Exception("루트와 ID가 같습니다.");

            if (_next == null)
            {
                _next = logger;
                return;
            }

            Logger? cur = this;
            Logger? prev = null;

            while (cur != null)
            {
                if (cur._id == logger._id)
                    throw new Exception("동일한 ID의 로거가 있습니다.");

                prev = cur;
                cur = cur._next;
            }

            if (prev != null)
                prev._next = logger;
        }

        public void Remove(int id)
        {
            // 루트는 삭제 불가능
            if (_id == id)
                throw new Exception("루트 로거는 삭제할 수 없습니다.");

            Logger? cur = this;
            Logger? prev = null;

            while (cur != null)
            {
                if (cur._id != id)
                {
                    prev = cur;
                    cur = cur._next;
                    continue;
                }

                // 루트가 곧 더미노드 역할을 하므로 절대로 prev가 null일 수 없다.
                Debug.Assert(prev != null);
                prev._next = cur._next;
                break;
            }
        }
    }
}

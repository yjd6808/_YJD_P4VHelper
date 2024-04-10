// jdyun 24/04/06(토)
using System.Diagnostics;

namespace P4VHelper.Base.Logger
{
    public abstract class Logger
    {
        private Logger? _next;
        private int _id;

        public Logger(int id)
        {
            _id = id;
        }

        public abstract void Write(LogLevel level, string msg);

        public void Write(int id, LogLevel level, string msg)
        {
            Logger? cur = this;

            while (cur != null)
            {
                if (cur._id == id)
                {
                    cur.Write(level, msg);
                    break;
                }

                cur = cur._next;
            }
        }

        public void WriteDebug(string msg) => Write(LogLevel.Debug, msg);
        public void WriteInfo(string msg) => Write(LogLevel.Info, msg);
        public void WriteError(string msg) => Write(LogLevel.Error, msg);
        public void WriteNormal(string msg) => Write(LogLevel.Normal, msg);
        public void WriteWarn(string msg) => Write(LogLevel.Warn, msg);

        public void WriteDebug(int id, string msg) => Write(id, LogLevel.Debug, msg);
        public void WriteInfo(int id, string msg) => Write(id, LogLevel.Info, msg);
        public void WriteError(int id, string msg) => Write(id, LogLevel.Error, msg);
        public void WriteNormal(int id, string msg) => Write(id, LogLevel.Normal, msg);
        public void WriteWarn(int id, string msg) => Write(id, LogLevel.Warn, msg);


        protected void WriteChaining(LogLevel level, string msg)
        {
            Logger? cur = _next;

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

// jdyun 24/04/06(토)
using System.Diagnostics;

namespace P4VHelper.Base.Logger
{
    public abstract class Logger
    {
        private Logger? next_;
        private readonly int id_;

        public Logger(int _id)
        {
            id_ = _id;
        }

        public abstract void Write(LogLevel _level, string _msg);

        public void Write(int _id, LogLevel _level, string _msg)
        {
            Logger? cur = this;

            while (cur != null)
            {
                if (cur.id_ == _id)
                {
                    cur.Write(_level, _msg);
                    break;
                }

                cur = cur.next_;
            }
        }

        public void WriteDebug(string _msg) => Write(LogLevel.Debug, _msg);
        public void WriteInfo(string _msg) => Write(LogLevel.Info, _msg);
        public void WriteError(string _msg) => Write(LogLevel.Error, _msg);
        public void WriteNormal(string _msg) => Write(LogLevel.Normal, _msg);
        public void WriteWarn(string _msg) => Write(LogLevel.Warn, _msg);

        public void WriteDebug(int _id, string _msg) => Write(_id, LogLevel.Debug, _msg);
        public void WriteInfo(int _id, string _msg) => Write(_id, LogLevel.Info, _msg);
        public void WriteError(int _id, string _msg) => Write(_id, LogLevel.Error, _msg);
        public void WriteNormal(int _id, string _msg) => Write(_id, LogLevel.Normal, _msg);
        public void WriteWarn(int _id, string _msg) => Write(_id, LogLevel.Warn, _msg);


        protected void WriteChaining(LogLevel _level, string _msg)
        {
            Logger? cur = next_;

            while (cur != null)
            {
                cur.Write(_level, _msg);
                cur = cur.next_;
            }
        }

        public void Add(Logger _logger)
        {
            if (_logger.id_ == id_)
                throw new Exception("루트와 ID가 같습니다.");

            if (next_ == null)
            {
                next_ = _logger;
                return;
            }

            Logger? cur = this;
            Logger? prev = null;

            while (cur != null)
            {
                if (cur.id_ == _logger.id_)
                    throw new Exception("동일한 ID의 로거가 있습니다.");

                prev = cur;
                cur = cur.next_;
            }

            if (prev != null)
                prev.next_ = _logger;
        }

        public void Remove(int _id)
        {
            // 루트는 삭제 불가능
            if (id_ == _id)
                throw new Exception("루트 로거는 삭제할 수 없습니다.");

            Logger? cur = this;
            Logger? prev = null;

            while (cur != null)
            {
                if (cur.id_ != _id)
                {
                    prev = cur;
                    cur = cur.next_;
                    continue;
                }

                // 루트가 곧 더미노드 역할을 하므로 절대로 prev가 null일 수 없다.
                Debug.Assert(prev != null);
                prev.next_ = cur.next_;
                break;
            }
        }
    }
}

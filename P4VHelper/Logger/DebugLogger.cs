using System.Diagnostics;
using P4VHelper.Base.Logger;

namespace P4VHelper.Logger
{
    internal class DebugLogger : Base.Logger.Logger
    {
        public const int ID = 1;

        public DebugLogger() : base(ID)
        {
        }

        public override void Write(LogLevel level, string msg)
        {
            Debug.WriteLine($"[{level}][{DateTime.Now:T}] {msg}");
        }
    }
}

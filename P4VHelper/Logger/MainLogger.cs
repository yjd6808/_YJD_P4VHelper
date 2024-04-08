using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4VHelper.Base.Logger;
using P4VHelper.Customize.Control;

namespace P4VHelper.Logger
{
    public class MainLogger : Base.Logger.Logger
    {
        public const int ID = 0;

        private LogListBox _logListBox;

        
        public MainLogger(LogListBox logListBox) : base(ID)
        {
            _logListBox = logListBox;
        }

        public override void Write(LogLevel level, string msg)
        {
            _logListBox.AddLog($"[{level}][{DateTime.Now:T}] {msg}");
            WriteChaining(level, msg);
        }
    }
}

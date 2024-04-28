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

        private readonly LogListBox logListBox_;

        
        public MainLogger(LogListBox _logListBox) : base(ID)
        {
            logListBox_ = _logListBox;
        }

        public override void Write(LogLevel _level, string _msg)
        {
            logListBox_.AddLog($"[{_level}][{DateTime.Now:T}] {_msg}");
            WriteChaining(_level, _msg);
        }
    }
}

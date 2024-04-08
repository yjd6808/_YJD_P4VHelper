// jdyun 24/04/08(월)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4VHelper.Resource
{
    public enum IconType
    {
        History,
    }

    internal class R
    {
        public const string RES_PATH = "pack://application:,,,/P4VHelper;component/Resource/";
        public const string ICON_PATH = RES_PATH + "Icons/";

        public const string ICON_HISTORY_KEY = ICON_PATH + "history.png";

        private static string GetIconKey(IconType type)
        {
            switch (type)
            {
                case IconType.History: return ICON_HISTORY_KEY;
                default: throw new ArgumentException("몽미");
            }
        }
    }
}

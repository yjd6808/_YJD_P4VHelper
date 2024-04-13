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
        Minimize,
        Maximize,
        Pin,
        Unpin,
        Close,
    }

    internal class R
    {
        public const string RES_PATH = "pack://application:,,,/P4VHelper;component/Resource/";
        public const string ICON_PATH = RES_PATH + "Icons/";

        public const string ICON_HISTORY_KEY = ICON_PATH + "history.png";
        public const string ICON_PIN_KEY = ICON_PATH + "common_pin.ico";
        public const string ICON_UNPIN_KEY = ICON_PATH + "common_unpin.ico";
        public const string ICON_MINIMIZE_KEY = ICON_PATH + "win_minimize.ico";
        public const string ICON_MAXIMIZE_KEY = ICON_PATH + "win_maximize.ico";
        public const string ICON_CLOSE_KEY = ICON_PATH + "win_close.ico";

        private static string GetIconKey(IconType type)
        {
            switch (type)
            {
                case IconType.History: return ICON_HISTORY_KEY;
                case IconType.Pin: return ICON_PIN_KEY;
                case IconType.Unpin: return ICON_UNPIN_KEY;
                case IconType.Minimize: return ICON_MINIMIZE_KEY;
                case IconType.Maximize: return ICON_MAXIMIZE_KEY;
                case IconType.Close: return ICON_CLOSE_KEY;
                default: throw new ArgumentException("몽미");
            }
        }
    }
}

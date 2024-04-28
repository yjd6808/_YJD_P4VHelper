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
        ArrowBottom,
        ArrowBottomLeft,
        ArrowBottomRight,
        ArrowLeft,
        ArrowRight,
        ArrowTop,
        ArrowTopLeft,
        ArrowTopRight,
        Album,
        Anchor,
        Backup,
        Checked,
        Collapse,
        Delete,
        Down,
        Empty,
        Grid,
        Info,
        Link,
        Music,
        NotUsable,
        Position,
        Rect,
        Refresh,
        Reset,
        Select,
        Spark,
        Transparent,
        Unclip,
        Unlock,
        Up,
        Run,
        Progress,
    }

    internal class R
    {
        public const string RES_PATH = "pack://application:,,,/P4VHelper;component/Resource/";
        public const string ICON_PATH = RES_PATH + "Icons/";

        public const string ICON_HISTORY_KEY = "history.png";
        public const string ICON_MINIMIZE_KEY = "win_minimize.ico";
        public const string ICON_MAXIMIZE_KEY = "win_maximize.ico";
        public const string ICON_CLOSE_KEY = "win_close.ico";
        public const string ICON_PIN_KEY = "common_pin.ico";
        public const string ICON_UNPIN_KEY = "common_unpin.ico";
        public const string ICON_UP_KEY = "common_up.ico";
        public const string ICON_ALBUM_KEY = "common_album.ico";
        public const string ICON_ANCHOR_KEY = "common_anchor.ico";
        public const string ICON_BACKUP_KEY = "common_backup.ico";
        public const string ICON_CHECKED_KEY = "common_checked.ico";
        public const string ICON_COLLAPSE_KEY = "common_collapse.ico";
        public const string ICON_DELETE_KEY = "common_delete.ico";
        public const string ICON_DOWN_KEY = "common_down.ico";
        public const string ICON_EMPTY_KEY = "common_empty.ico";
        public const string ICON_GRID_KEY = "common_grid.ico";
        public const string ICON_INFO_KEY = "common_info.ico";
        public const string ICON_LINK_KEY = "common_link.ico";
        public const string ICON_MUSIC_KEY = "common_music.ico";
        public const string ICON_NOT_USABLE_KEY = "common_not_usable.ico";
        public const string ICON_POSITION_KEY = "common_position.ico";
        public const string ICON_RECT_KEY = "common_rect.ico";
        public const string ICON_REFRESH_KEY = "common_refresh.ico";
        public const string ICON_RESET_KEY = "common_reset.ico";
        public const string ICON_SELECT_KEY = "common_select.ico";
        public const string ICON_SPARK_KEY = "common_spark.ico";
        public const string ICON_TRANSPARENT_KEY = "common_transparent.ico";
        public const string ICON_UNCLIP_KEY = "common_unclip.ico";
        public const string ICON_UNLOCK_KEY = "common_unlock.ico";
        public const string ICON_RUN_KEY = "common_run.ico";
        public const string ICON_PROGRESS_KEY = "common_progress.ico";

        private static string GetIconKey(IconType _type)
        {
            switch (_type)
            {
                case IconType.History: return ICON_HISTORY_KEY;
                case IconType.Pin: return ICON_PIN_KEY;
                case IconType.Unpin: return ICON_UNPIN_KEY;
                case IconType.Minimize: return ICON_MINIMIZE_KEY;
                case IconType.Maximize: return ICON_MAXIMIZE_KEY;
                case IconType.Close: return ICON_CLOSE_KEY;
                case IconType.Up: return ICON_UP_KEY;
                case IconType.Album: return ICON_ALBUM_KEY;
                case IconType.Anchor: return ICON_ANCHOR_KEY;
                case IconType.Backup: return ICON_BACKUP_KEY;
                case IconType.Checked: return ICON_CHECKED_KEY;
                case IconType.Collapse: return ICON_COLLAPSE_KEY;
                case IconType.Delete: return ICON_DELETE_KEY;
                case IconType.Down: return ICON_DOWN_KEY;
                case IconType.Empty: return ICON_EMPTY_KEY;
                case IconType.Grid: return ICON_GRID_KEY;
                case IconType.Info: return ICON_INFO_KEY;
                case IconType.Link: return ICON_LINK_KEY;
                case IconType.Music: return ICON_MUSIC_KEY;
                case IconType.NotUsable: return ICON_NOT_USABLE_KEY;
                case IconType.Position: return ICON_POSITION_KEY;
                case IconType.Rect: return ICON_RECT_KEY;
                case IconType.Refresh: return ICON_REFRESH_KEY;
                case IconType.Reset: return ICON_RESET_KEY;
                case IconType.Select: return ICON_SELECT_KEY;
                case IconType.Spark: return ICON_SPARK_KEY;
                case IconType.Transparent: return ICON_TRANSPARENT_KEY;
                case IconType.Unclip: return ICON_UNCLIP_KEY;
                case IconType.Unlock: return ICON_UNLOCK_KEY;
                case IconType.Run: return ICON_RUN_KEY;
                case IconType.Progress: return ICON_PROGRESS_KEY;
                default: throw new ArgumentException("Unknown icon type");
            }
        }
    }
}

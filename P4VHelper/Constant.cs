using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using P4VHelper.Customize.Converter;

namespace P4VHelper
{
    public class Constant
    {
        public const int ZERO = 0;

        // MainView에서 사용
        public const string TITLE = "P4VHelper";
        public const double MAIN_GRID_HEIGHT_MULTIPLIER = 1.4;    // 확장 상태바를 포함한 MainGrid의 높이 MainPanelGrid는 윈도우 높이 배수 (800 x 1.4)
        public const double STATUS_BAR_HEIGHT = 20.0;
        public const double STATUS_BAR_EXPANDED_DIVISION = 2.5;   // 확장 상태바의 높이는 MainView 높이의 1/2.5이다.
        public static readonly Thickness STATUS_BAR_HEIGHT_THICKNESS = new (0, 0, 0, STATUS_BAR_HEIGHT);
        public const string STATUS_BAR_RUNNING_THREAD_COUNT_STRING_FMT = "작업 실행중인 쓰레드 수 : {0}";     // 안씀
        public const string STATUS_BAR_WATING_TASK_COUNT = "실행 대기중인 작업 수 : {1}";                    // 안씀
    }
}

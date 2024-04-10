// jdyun 24/04/08(월)
using P4VHelper.Resource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace P4VHelper.Customize.Control
{
    public partial class LogListBox : System.Windows.Controls.UserControl
    {
        public ObservableCollection<string> Logs { get; } = new();

        #region Properties
        public int MaxItemCount
        {
            get => (int)GetValue(MaxItemCountProperty);
            set => SetValue(MaxItemCountProperty, value);
        }

        public static DependencyProperty MaxItemCountProperty = DependencyProperty.Register(
            nameof(MaxItemCount),
            typeof(int),
            typeof(LogListBox),
            new PropertyMetadata(1000)
        );
        #endregion


        public LogListBox()
        {
            InitializeComponent();
        }

        public void AddLog(string log)
        {
            if (Logs.Count > MaxItemCount)
                Logs.RemoveAt(0);

            Logs.Add(log);

            if (Logs.Count > 0)
                _lbLogList.ScrollIntoView(Logs.Last());
        }

        public void Clear()
        {
            Logs.Clear();
        }
    }
}

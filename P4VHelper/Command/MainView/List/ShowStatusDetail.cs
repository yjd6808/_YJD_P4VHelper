using P4VHelper.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using P4VHelper.Customize.Control;

namespace P4VHelper.Command.MainView.List
{
    public class ShowStatusDetail : MainCommand<ImageToggleButton>
    {
        public ShowStatusDetail(MainViewModel _viewModel) : base(_viewModel, "상태바 상세보기 클릭")
        {
        }

        public override void Execute(ImageToggleButton _button)
        {
            ViewModel.TaskMgr.ViewDetail = _button.Toggled;
        }
    }
}

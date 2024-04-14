using P4VHelper.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4VHelper.Command.MainView.List
{
    public class ShowStatusDetail : MainCommand
    {
        public ShowStatusDetail(MainViewModel viewModel) : base(viewModel, "상태바 상세보기 클릭")
        {
        }

        public override void Execute()
        {
            if (ViewModel.TaskMgr.ViewdDetail)
                ViewModel.TaskMgr.ViewdDetail = false;
            else
                ViewModel.TaskMgr.ViewdDetail = true;
        }
    }
}

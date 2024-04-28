using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4VHelper.Base.Logger;
using P4VHelper.ViewModel;

namespace P4VHelper.Command.MainView.List
{
    public class Test : MainCommand
    {
        public Test(MainViewModel _viewModel) : base(_viewModel, "테스트 커맨드")
        {
        }
        public override void Execute()
        {
            ViewModel.Logger.WriteNormal("안녕하세요");
        }
    }
}

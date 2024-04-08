using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4VHelper.Base.Logger;
using P4VHelper.ViewModel;

namespace P4VHelper.Command.MainView.List
{
    internal class Test : MainCommand
    {
        public Test(MainViewModel viewModel) : base(viewModel, "테스트 커맨드")
        {
        }
        public override void Execute()
        {
            ViewModel.Logger.WriteNormal("안녕하세요");
        }
    }
}

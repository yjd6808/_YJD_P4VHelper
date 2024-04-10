// jdyun 24/04/10(수)
using P4VHelper.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4VHelper.Model.Main
{
    public class Changelist : Bindable
    {
        private long _revision;
        public long Revision
        {
            get => _revision;
            set
            {
                _revision = value;
                OnPropertyChanged();
            }
        }

        private string _userName = string.Empty;
        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value; 
                OnPropertyChanged();
            }
        }

        private string _descryption = string.Empty;
        public string Descryption
        {
            get => _descryption;
            set
            {
                _descryption = value;
                OnPropertyChanged();
            }
        }
    }
}

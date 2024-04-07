// jdyun 24/04/06(토)
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace P4VHelper.Base
{
    public class Bindable : INotifyPropertyChanged
    {
        [Browsable(false)]
        public bool IsNotifyEnabled { get; set; } = true;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            if (!IsNotifyEnabled)
                return;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void NotifyProperty(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;

            if (IsNotifyEnabled)
                OnPropertyChanged(propertyName);

            return true;
        }
    }
}

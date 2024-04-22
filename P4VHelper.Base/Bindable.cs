// jdyun 24/04/06(토)
/*
 * [주의사항]
 * OnPropertyChanged(nameof(Notifier.First.Percent))는 현재 개체에 대해서 Notifier.Second.Percent라는 프로퍼티명을 찾을려고 시도하기 때문에 동작하지 않는다.
 * this -> Notifier -> Second -> Percent
 *
 * this가 아닌 Second 개체에서 Percent 프로퍼티를 찾아서 실행하도록 해야한다.
 * 따라서 Notifier.Second.NotifyProperty("Percent")와 같이 실행시켜줘야한다.
 * 헷갈릴 수 있으므로 주의해야함.
 */


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

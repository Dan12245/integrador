using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Consumo_Reducido_de_Agua_ahora_si_definitivo.Core
{
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;


        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(sender:this, e:new PropertyChangedEventArgs(propertyName));
        }
    }
}

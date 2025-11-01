using System;
using System.Globalization;
using System.Windows.Data;

namespace Consumo_Reducido_de_Agua_ahora_si_definitivo.ViewModels
{
    public class PeriodToCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString().Equals(parameter?.ToString(), StringComparison.OrdinalIgnoreCase) == true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return parameter?.ToString();
            return Binding.DoNothing;
        }
    }
}
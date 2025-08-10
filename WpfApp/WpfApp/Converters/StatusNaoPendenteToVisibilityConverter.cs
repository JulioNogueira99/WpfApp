using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using WpfApp.Enums;

namespace WpfApp.Converters
{
    public class StatusNaoPendenteToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is StatusPedido status)
                return status != StatusPedido.Pendente ? Visibility.Visible : Visibility.Collapsed;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}

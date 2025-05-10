using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace BibliotecaProyectoIntegrado.Converters
{
    public class GestionarInventarioTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = value?.ToString()?.ToLowerInvariant();

            if (string.IsNullOrEmpty(status))
                return "Gestionar";

            if (status.Contains("prestado"))
                return "Devolver";
            else
                return "Prestar";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}

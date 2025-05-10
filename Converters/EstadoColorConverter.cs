using System.Globalization;
using Microsoft.Maui.Controls;

namespace BibliotecaProyectoIntegrado.Converters;

public class EstadoColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString()?.ToLower() switch
        {
            "disponible" => "#7EBB94",
            "prestado" => Colors.Red,
            _ => Colors.Black
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

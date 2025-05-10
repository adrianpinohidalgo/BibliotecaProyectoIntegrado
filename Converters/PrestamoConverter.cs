using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace BibliotecaProyectoIntegrado.Converters
{
    // PrestamoConverter.cs
    public class PrestamoConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => value is bool boolValue && boolValue ? "Devolver libro" : "Pedir préstamo";

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
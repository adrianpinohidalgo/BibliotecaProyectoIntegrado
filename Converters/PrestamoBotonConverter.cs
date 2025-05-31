using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using BibliotecaProyectoIntegrado.Models;

namespace BibliotecaProyectoIntegrado.Converters
{
    public class PrestamoBotonConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2 || values[0] is not bool prestadoPorUsuario || values[1] is not bool prestadoPorOtro)
                return null;

            PrestamoBotonEstado resultado;

            if (prestadoPorOtro)
            {
                resultado = new PrestamoBotonEstado
                {
                    Texto = "No disponible",
                    Fondo = Colors.Gray,
                    Habilitado = false
                };
            }
            else if (prestadoPorUsuario)
            {
                resultado = new PrestamoBotonEstado
                {
                    Texto = "Devolver libro",
                    Fondo = Colors.Black,
                    Habilitado = true
                };
            }
            else
            {
                resultado = new PrestamoBotonEstado
                {
                    Texto = "Pedir préstamo",
                    Fondo = Color.FromArgb("#7EBB94"),
                    Habilitado = true
                };
            }

            return parameter switch
            {
                "color" => resultado.Fondo,
                "enabled" => resultado.Habilitado,
                _ => resultado.Texto
            };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}

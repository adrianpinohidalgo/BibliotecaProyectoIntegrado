using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BibliotecaProyectoIntegrado.Models
{
    public class LibroConEstado : INotifyPropertyChanged
    {
        public Libro Libro { get; set; }

        private bool _estaPrestadoPorUsuarioActual;
        public bool EstaPrestadoPorUsuarioActual
        {
            get => _estaPrestadoPorUsuarioActual;
            set
            {
                if (_estaPrestadoPorUsuarioActual != value)
                {
                    _estaPrestadoPorUsuarioActual = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _estaPrestadoPorOtroUsuario;
        public bool EstaPrestadoPorOtroUsuario
        {
            get => _estaPrestadoPorOtroUsuario;
            set
            {
                if (_estaPrestadoPorOtroUsuario != value)
                {
                    _estaPrestadoPorOtroUsuario = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }



}

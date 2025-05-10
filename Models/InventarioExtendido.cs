using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BibliotecaProyectoIntegrado.Models
{
    public class InventarioExtendido : INotifyPropertyChanged
    {
        public int InventarioId { get; set; }

        public Libro Libro { get; set; } = null!;

        private string _status = "";
        public string Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

using BibliotecaProyectoIntegrado.Models;
using BibliotecaProyectoIntegrado.Services;
using BibliotecaProyectoIntegrado.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace BibliotecaProyectoIntegrado.ViewModels
{
    public class PrestamosViewModel : BaseViewModel
    {
        public ObservableCollection<PrestamoExtendido> Prestamos { get; set; } = new();

        public ICommand DevolverCommand { get; }

        private bool _esAdmin;
        public bool EsAdmin
        {
            get => _esAdmin;
            set
            {
                _esAdmin = value;
                OnPropertyChanged();
            }
        }

        public PrestamosViewModel()
        {
            DevolverCommand = new Command<PrestamoExtendido>(async (prestamo) => await DevolverPrestamoAsync(prestamo));
            WeakReferenceMessenger.Default.Register<LibroActualizadoMessage>(this, (r, m) => LoadPrestamos());
            LoadPrestamos();
        }


        private async void LoadPrestamos()
        {
            int usuarioId = Preferences.Get("UsuarioId", 0);
            var usuarios = await DatabaseService.GetUsuariosAsync();
            var usuario = usuarios.FirstOrDefault(u => u.Id == usuarioId);
            EsAdmin = usuario?.NumeroSocio == "U001";

            List<PrestamoExtendido> prestamosActivos;

            if (EsAdmin)
            {
                prestamosActivos = await DatabaseService.GetTodosLosPrestamosExtendidosAsync();
            }
            else
            {
                prestamosActivos = await DatabaseService.GetPrestamosExtendidosDelUsuarioAsync(usuarioId);
            }

            Prestamos = new ObservableCollection<PrestamoExtendido>(prestamosActivos);
            OnPropertyChanged(nameof(Prestamos));
        }


        private async Task DevolverPrestamoAsync(PrestamoExtendido prestamoExtendido)
        {
            // Crea un objeto Prestamo "normal"
            var prestamo = new Prestamo
            {
                Id = prestamoExtendido.Id,
                LibroId = prestamoExtendido.LibroId,
                UsuarioId = prestamoExtendido.UsuarioId,
                FechaPrestamo = prestamoExtendido.FechaPrestamo,
                FechaDevolucion = DateTime.Now
            };

            await DatabaseService.UpdatePrestamoAsync(prestamo);

            var inventario = await DatabaseService.GetInventarioPorLibroAsync(prestamo.LibroId);
            if (inventario != null)
            {
                inventario.Status = "Disponible";
                await DatabaseService.UpdateInventarioAsync(inventario);
            }

            await Application.Current.MainPage.DisplayAlert("Devolución", $"Has devuelto '{prestamoExtendido.Libro?.Titulo}'.", "OK");

            WeakReferenceMessenger.Default.Send(new LibroActualizadoMessage(prestamoExtendido.Libro));

            LoadPrestamos();
        }

    }
}


// ✅ Asegúrate de poner esta clase **fuera** del ViewModel, pero en el mismo archivo o en otro si prefieres
public class PrestamoExtendido : Prestamo
{
    public Libro? Libro { get; set; }
    public Usuario? Usuario { get; set; }
    public DateTime FechaVencimiento => FechaPrestamo.AddDays(14);

}

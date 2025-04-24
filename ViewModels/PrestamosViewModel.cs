using BibliotecaProyectoIntegrado.Models;
using BibliotecaProyectoIntegrado.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace BibliotecaProyectoIntegrado.ViewModels;

public class PrestamosViewModel : BaseViewModel
{
    public ObservableCollection<PrestamoExtendido> PrestamosActivos { get; set; } = new();
    public ObservableCollection<PrestamoExtendido> PrestamosVencidos { get; set; } = new();

    public ObservableCollection<Libro> LibrosDisponibles { get; set; } = new();
    public ObservableCollection<Usuario> Usuarios { get; set; } = new();

    private Libro _libroSeleccionado;
    public Libro LibroSeleccionado
    {
        get => _libroSeleccionado;
        set => SetProperty(ref _libroSeleccionado, value);
    }

    private Usuario _usuarioSeleccionado;
    public Usuario UsuarioSeleccionado
    {
        get => _usuarioSeleccionado;
        set => SetProperty(ref _usuarioSeleccionado, value);
    }


    public ICommand FinalizarPrestamoCommand { get; }
    public ICommand RenovarPrestamoCommand { get; }
    public ICommand RegistrarPrestamoCommand { get; }

    public PrestamosViewModel()
    {
        FinalizarPrestamoCommand = new Command<PrestamoExtendido>(async (prestamo) =>
        {
            prestamo.FechaDevolucion = DateTime.Now;
            await DatabaseService.UpdatePrestamoAsync(prestamo);
            await CargarPrestamosAsync();
        });

        RenovarPrestamoCommand = new Command<PrestamoExtendido>(async (prestamo) =>
        {
            prestamo.FechaPrestamo = DateTime.Now;
            await DatabaseService.UpdatePrestamoAsync(prestamo);
            await CargarPrestamosAsync();
        });

        RegistrarPrestamoCommand = new Command(async () => await RegistrarPrestamoAsync());

        Task.Run(CargarPrestamosAsync);
        Task.Run(CargarDatosAsync);
    }

    public async Task CargarPrestamosAsync()
    {
        var prestamos = await DatabaseService.GetPrestamosExtendidosAsync();
        var ahora = DateTime.Now;

        var activos = prestamos.Where(p => p.FechaDevolucion == null && p.FechaPrestamo.AddDays(14) >= ahora);
        var vencidos = prestamos.Where(p => p.FechaDevolucion == null && p.FechaPrestamo.AddDays(14) < ahora);

        MainThread.BeginInvokeOnMainThread(() =>
        {
            PrestamosActivos.Clear();
            PrestamosVencidos.Clear();

            foreach (var p in activos)
                PrestamosActivos.Add(p);

            foreach (var p in vencidos)
                PrestamosVencidos.Add(p);
        });
    }

    public async Task CargarDatosAsync()
    {
        var libros = await DatabaseService.GetLibrosDisponiblesAsync(); // Ahora lo creamos
        var usuarios = await DatabaseService.GetUsuariosAsync();

        MainThread.BeginInvokeOnMainThread(() =>
        {
            LibrosDisponibles.Clear();
            foreach (var libro in libros)
                LibrosDisponibles.Add(libro);

            Usuarios.Clear();
            foreach (var usuario in usuarios)
                Usuarios.Add(usuario);
        });
    }


    public async Task RegistrarPrestamoAsync()
    {
        try
        {
            var libros = await DatabaseService.GetLibrosDisponiblesAsync();

            if (libros.Count == 0)
            {
                await Application.Current.MainPage.DisplayAlert("Aviso", "No hay libros disponibles.", "OK");
                return;
            }

            // Mostrar los títulos de los libros disponibles
            var titulos = libros.Select(l => l.Titulo).ToArray();
            string tituloSeleccionado = await Application.Current.MainPage.DisplayActionSheet(
                "Selecciona un libro", "Cancelar", null, titulos);

            if (string.IsNullOrEmpty(tituloSeleccionado) || tituloSeleccionado == "Cancelar")
                return;

            var libroSeleccionado = libros.FirstOrDefault(l => l.Titulo == tituloSeleccionado);
            if (libroSeleccionado == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No se encontró el libro seleccionado.", "OK");
                return;
            }

            int usuarioIdFijo = 1; // ← o el usuario logueado
            await DatabaseService.RegistrarPrestamoAsync(libroSeleccionado.Id, usuarioIdFijo);
            await CargarPrestamosAsync();

            var fechaDevolucion = DateTime.Now.AddDays(14).ToString("dd/MM/yyyy");

            await Application.Current.MainPage.DisplayAlert(
                "Éxito",
                $"Préstamo de '{libroSeleccionado.Titulo}' registrado.\nFecha de devolución: {fechaDevolucion}",
                "OK");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
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

using System.Collections.ObjectModel;
using System.Windows.Input;
using BibliotecaProyectoIntegrado.Services;
using BibliotecaProyectoIntegrado.Models;
using CommunityToolkit.Mvvm.Messaging;
using Plugin.LocalNotification;

namespace BibliotecaProyectoIntegrado.ViewModels;

public class MainPageViewModel : BaseViewModel
{
    public ObservableCollection<Libro> AllBooks { get; set; } = new();
    public ObservableCollection<LibroConEstado> FilteredBooks
    {
        get => _filteredBooks;
        set
        {
            _filteredBooks = value;
            OnPropertyChanged();
        }
    }
    private ObservableCollection<LibroConEstado> _filteredBooks = new();


    public int AvailableBooks { get; set; }
    public int ActiveLoans { get; set; }

    public ICommand FilterCommand { get; }

    public ICommand PedirPrestamoCommand { get; }

    public MainPageViewModel()
    {
        FilterCommand = new Command<string>(FilterBooks);
        PedirPrestamoCommand = new Command<Libro>(async (libro) => await AlternarPrestamoAsync(libro));
        WeakReferenceMessenger.Default.Register<LibroActualizadoMessage>(this, (r, m) => RecargarLibros());
        LoadDataAsync();
    }

 

    private async Task RealizarPrestamoAsync(Libro libro)
    {
        try
        {
            int usuarioId = Preferences.Get("UsuarioId", 0);
            if (usuarioId == 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Usuario no identificado.", "OK");
                return;
            }

            await DatabaseService.RegistrarPrestamoAsync(libro.Id, usuarioId);
            await Application.Current.MainPage.DisplayAlert("Éxito", $"Has solicitado el libro '{libro.Titulo}'.", "OK");

            // Recargar libros para reflejar cambios en el inventario
            await Task.Delay(300); // opcional: pequeña espera para asegurar actualización
            RecargarLibros();
            WeakReferenceMessenger.Default.Send(new LibroActualizadoMessage(libro));
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
    }



    private async void LoadDataAsync()
    {
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "biblioteca.db");
        await DatabaseService.InitAsync(dbPath);

        var books = await DatabaseService.GetBooksAsync();
        AllBooks = new ObservableCollection<Libro>(books);
        await ApplyFiltersAsync(); // ✅ actualiza correctamente FilteredBooks

        AvailableBooks = await DatabaseService.GetAvailableBooksCountAsync();
        ActiveLoans = await DatabaseService.GetActiveLoansCountAsync();

        // ✅ Recupera el nombre del usuario
        var userId = Preferences.Get("UsuarioId", 0);
        var usuario = (await DatabaseService.GetUsuariosAsync()).FirstOrDefault(u => u.Id == userId);
        NombreUsuario = usuario?.Nombre ?? "Usuario";
        EmailUsuario = usuario?.Email ?? "Email";

        OnPropertyChanged(nameof(AvailableBooks));
        OnPropertyChanged(nameof(ActiveLoans));
    }


    private string _selectedGenre = "Todos";
    public string SelectedGenre
    {
        get => _selectedGenre;
        set
        {
            _selectedGenre = value;
            OnPropertyChanged();
        }
    }

    private string _nombreUsuario;
    public string NombreUsuario
    {
        get => _nombreUsuario;
        set
        {
            _nombreUsuario = value;
            OnPropertyChanged();
        }
    }

    private string _emailUsuario;
    public string EmailUsuario
    {
        get => _emailUsuario;
        set
        {
            _emailUsuario = value;
            OnPropertyChanged();
        }
    }

    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }

    private void FilterBooks(string genre)
    {
        SelectedGenre = genre;
        ApplyFilters();
    }

    //private async void ApplyFilters()
    //{
    //    var usuarioId = Preferences.Get("UsuarioId", 0);
    //    var prestamos = await DatabaseService.GetPrestamosDelUsuarioAsync(usuarioId);

    //    var prestadosIds = prestamos.Select(p => p.LibroId).ToHashSet();

    //    var filtrados = AllBooks
    //        .Where(b =>
    //            (SelectedGenre == "All" || b.Genero.Equals(SelectedGenre, StringComparison.OrdinalIgnoreCase)) &&
    //            (string.IsNullOrWhiteSpace(SearchText) ||
    //             (!string.IsNullOrEmpty(b.Titulo) && b.Titulo.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) ||
    //             (!string.IsNullOrEmpty(b.Autor) && b.Autor.Contains(SearchText, StringComparison.OrdinalIgnoreCase)))
    //        )
    //        .Select(libro => new LibroConEstado
    //        {
    //            Libro = libro,
    //            EstaPrestado = prestadosIds.Contains(libro.Id)
    //        });

    //    FilteredBooks = new ObservableCollection<LibroConEstado>(filtrados);
    //}

    private async void ApplyFilters()
    {
        var usuarioId = Preferences.Get("UsuarioId", 0);

        // Obtener préstamos del usuario actual
        var prestamosUsuarioActual = await DatabaseService.GetPrestamosDelUsuarioAsync(usuarioId);
        var prestadosUsuarioActualIds = prestamosUsuarioActual
            .Where(p => p.FechaDevolucion == null) // Solo activos
            .Select(p => p.LibroId)
            .ToHashSet();

        var filtrados = new List<LibroConEstado>();

        var librosParaFiltrar = AllBooks
            .Where(b =>
                (SelectedGenre == "Todos" || b.Genero.Equals(SelectedGenre, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrWhiteSpace(SearchText) ||
                 (!string.IsNullOrEmpty(b.Titulo) && b.Titulo.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) ||
                 (!string.IsNullOrEmpty(b.Autor) && b.Autor.Contains(SearchText, StringComparison.OrdinalIgnoreCase)))
            );

        foreach (var libro in librosParaFiltrar)
        {
            bool estaPrestadoPorUsuarioActual = prestadosUsuarioActualIds.Contains(libro.Id);
            bool hayEjemplaresPrestados = await DatabaseService.HayEjemplaresPrestadosAsync(libro.Id);
            bool estaPrestadoPorOtroUsuario = hayEjemplaresPrestados && !estaPrestadoPorUsuarioActual;

            filtrados.Add(new LibroConEstado
            {
                Libro = libro,
                EstaPrestadoPorUsuarioActual = estaPrestadoPorUsuarioActual,
                EstaPrestadoPorOtroUsuario = estaPrestadoPorOtroUsuario
            });
        }

        FilteredBooks = new ObservableCollection<LibroConEstado>(filtrados);
    }

    private async Task AlternarPrestamoAsync(Libro libro)
    {
        try
        {
            int usuarioId = Preferences.Get("UsuarioId", 0);
            if (usuarioId == 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Usuario no identificado.", "OK");
                return;
            }

            // ✅ Verificar el estado actual del libro
            var libroConEstado = FilteredBooks.FirstOrDefault(l => l.Libro.Id == libro.Id);
            if (libroConEstado != null && libroConEstado.EstaPrestadoPorOtroUsuario)
            {
                await Application.Current.MainPage.DisplayAlert("No disponible",
                    $"El libro '{libro.Titulo}' está prestado por otro usuario.", "OK");
                return;
            }

            var prestamos = await DatabaseService.GetPrestamosDelUsuarioAsync(usuarioId);
            var prestamoExistente = prestamos.FirstOrDefault(p => p.LibroId == libro.Id && p.FechaDevolucion == null);

            if (prestamoExistente != null)
            {
                // Devolver libro (solo si lo tiene el usuario actual)
                prestamoExistente.FechaDevolucion = DateTime.Now;
                await DatabaseService.UpdatePrestamoAsync(prestamoExistente);

                var inventario = await DatabaseService.GetInventarioPorLibroAsync(libro.Id);
                if (inventario != null)
                {
                    inventario.Status = "Disponible";
                    await DatabaseService.UpdateInventarioAsync(inventario);
                }

                await Application.Current.MainPage.DisplayAlert("Devolución", $"Has devuelto '{libro.Titulo}'.", "OK");
            }
            else
            {
                // Verificar disponibilidad antes de prestar
                bool hayEjemplaresPrestados = await DatabaseService.HayEjemplaresPrestadosAsync(libro.Id);
                if (hayEjemplaresPrestados)
                {
                    await Application.Current.MainPage.DisplayAlert("No disponible",
                        $"El libro '{libro.Titulo}' no está disponible en este momento.", "OK");
                    return;
                }

                // Registrar préstamo
                var (exito, libroPrestado, usuario) = await DatabaseService.RegistrarPrestamoYObtenerDatosAsync(libro.Id, usuarioId);

                if (exito && libroPrestado != null && usuario != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Éxito", $"Has solicitado '{libro.Titulo}'.", "OK");

                    // Notificación
                    var request = new NotificationRequest
                    {
                        NotificationId = (int)DateTime.Now.Ticks % int.MaxValue,
                        Title = "¡Préstamo Registrado!",
                        Description = $"Hola {usuario.Nombre}, has tomado prestado '{libro.Titulo}'. Devuelve antes del {DateTime.Now.AddDays(14):dd/MM/yyyy}",
                        Schedule = new NotificationRequestSchedule
                        {
                            NotifyTime = DateTime.Now.AddSeconds(1)
                        }
                    };

                    try
                    {
                        await LocalNotificationCenter.Current.Show(request);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error notificando: {ex.Message}");
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "No se pudo registrar el préstamo.", "OK");
                }
            }

            await Task.Delay(300); // Espera opcional para asegurar la escritura en BD
            RecargarLibros();
            WeakReferenceMessenger.Default.Send(new LibroActualizadoMessage(libro));
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async void RecargarLibros()
    {
        var books = await DatabaseService.GetBooksAsync();
        AllBooks = new ObservableCollection<Libro>(books);

        // Asegura que la vista esté actualizada con datos frescos
        await Task.Delay(200); // Pequeño delay para dar tiempo al DB commit si es necesario
        await ApplyFiltersAsync();
    }

    private async Task ApplyFiltersAsync()
    {
        var usuarioId = Preferences.Get("UsuarioId", 0);

        var prestamosUsuarioActual = await DatabaseService.GetPrestamosDelUsuarioAsync(usuarioId);
        var prestadosUsuarioActualIds = prestamosUsuarioActual
            .Where(p => p.FechaDevolucion == null)
            .Select(p => p.LibroId)
            .ToHashSet();

        var filtrados = new List<LibroConEstado>();

        var librosParaFiltrar = AllBooks
            .Where(b =>
                (SelectedGenre == "All" || b.Genero.Equals(SelectedGenre, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrWhiteSpace(SearchText) ||
                 (!string.IsNullOrEmpty(b.Titulo) && b.Titulo.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) ||
                 (!string.IsNullOrEmpty(b.Autor) && b.Autor.Contains(SearchText, StringComparison.OrdinalIgnoreCase)))
            );

        foreach (var libro in librosParaFiltrar)
        {
            bool estaPrestadoPorUsuarioActual = prestadosUsuarioActualIds.Contains(libro.Id);
            bool hayEjemplaresPrestados = await DatabaseService.HayEjemplaresPrestadosAsync(libro.Id);
            bool estaPrestadoPorOtroUsuario = hayEjemplaresPrestados && !estaPrestadoPorUsuarioActual;

            filtrados.Add(new LibroConEstado
            {
                Libro = libro,
                EstaPrestadoPorUsuarioActual = estaPrestadoPorUsuarioActual,
                EstaPrestadoPorOtroUsuario = estaPrestadoPorOtroUsuario
            });
        }

        FilteredBooks = new ObservableCollection<LibroConEstado>(filtrados);

        System.Diagnostics.Debug.WriteLine($"Usuario actual: {usuarioId}");
        System.Diagnostics.Debug.WriteLine($"Préstamos activos del usuario: {prestadosUsuarioActualIds.Count}");

    }







}

using System.Collections.ObjectModel;
using System.Windows.Input;
using BibliotecaProyectoIntegrado.Services;
using BibliotecaProyectoIntegrado.Models;
using CommunityToolkit.Mvvm.Messaging;

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

            var prestamos = await DatabaseService.GetPrestamosDelUsuarioAsync(usuarioId);
            var prestamoExistente = prestamos.FirstOrDefault(p => p.LibroId == libro.Id && p.FechaDevolucion == null);

            if (prestamoExistente != null)
            {
                // Devolver libro
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
                // Pedir préstamo
                await DatabaseService.RegistrarPrestamoAsync(libro.Id, usuarioId);
                await Application.Current.MainPage.DisplayAlert("Éxito", $"Has solicitado '{libro.Titulo}'.", "OK");
            }

            RecargarLibros();
            WeakReferenceMessenger.Default.Send(new LibroActualizadoMessage(libro));
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
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
        ApplyFilters(); // ✅ actualiza correctamente FilteredBooks

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


    private string _selectedGenre = "All";
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

    private async void ApplyFilters()
    {
        var usuarioId = Preferences.Get("UsuarioId", 0);
        var prestamos = await DatabaseService.GetPrestamosDelUsuarioAsync(usuarioId);

        var prestadosIds = prestamos.Select(p => p.LibroId).ToHashSet();

        var filtrados = AllBooks
            .Where(b =>
                (SelectedGenre == "All" || b.Genero.Equals(SelectedGenre, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrWhiteSpace(SearchText) ||
                 (!string.IsNullOrEmpty(b.Titulo) && b.Titulo.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) ||
                 (!string.IsNullOrEmpty(b.Autor) && b.Autor.Contains(SearchText, StringComparison.OrdinalIgnoreCase)))
            )
            .Select(libro => new LibroConEstado
            {
                Libro = libro,
                EstaPrestado = prestadosIds.Contains(libro.Id)
            });

        FilteredBooks = new ObservableCollection<LibroConEstado>(filtrados);
    }


    private async void RecargarLibros()
    {
        var books = await DatabaseService.GetBooksAsync();
        AllBooks = new ObservableCollection<Libro>(books);
        ApplyFilters();
    }




}

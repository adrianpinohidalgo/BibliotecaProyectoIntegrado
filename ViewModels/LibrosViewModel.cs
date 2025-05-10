using System.Collections.ObjectModel;
using System.Windows.Input;
using BibliotecaProyectoIntegrado.Models;
using BibliotecaProyectoIntegrado.Services;
using BibliotecaProyectoIntegrado.Views;
using CommunityToolkit.Mvvm.Messaging;

namespace BibliotecaProyectoIntegrado.ViewModels
{
    public class LibrosViewModel : BaseViewModel
    {
        public ObservableCollection<Libro> Libros { get; set; } = new();
        public ICommand AgregarLibroCommand { get; }
        public ICommand EditarLibroCommand { get; }

        public bool EsAdmin { get; set; }

        public LibrosViewModel()
        {
            _ = LoadLibrosAsync();
            AgregarLibroCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync(nameof(LibroFormPage));

            }); 
            EditarLibroCommand = new Command(async () => await EditarLibroAsync());
            WeakReferenceMessenger.Default.Register<LibroActualizadoMessage>(this, async (r, m) =>
            {
                await LoadLibrosAsync();
            });

        }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (_selectedIndex != value)
                {
                    _selectedIndex = value;
                    OnPropertyChanged();
                }
            }
        }


        private async Task LoadLibrosAsync() // Change return type from void to Task
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "biblioteca.db");
            await DatabaseService.InitAsync(dbPath);
            var libros = await DatabaseService.GetBooksAsync();
            Libros = new ObservableCollection<Libro>(libros);
            OnPropertyChanged(nameof(Libros));
            var userId = Preferences.Get("UsuarioId", 0);
            var usuario = (await DatabaseService.GetUsuariosAsync()).FirstOrDefault(u => u.Id == userId);
            EsAdmin = usuario?.NumeroSocio == "U001";
            OnPropertyChanged(nameof(EsAdmin));
        }

        private async Task AgregarLibroAsync()
        {
            string titulo = await Application.Current.MainPage.DisplayPromptAsync("Nuevo libro", "Introduce el título:");
            if (string.IsNullOrWhiteSpace(titulo)) return;

            string autor = await Application.Current.MainPage.DisplayPromptAsync("Nuevo libro", "Introduce el autor:");
            if (string.IsNullOrWhiteSpace(autor)) return;

            string genero = await Application.Current.MainPage.DisplayPromptAsync("Nuevo libro", "Introduce el género:");
            string isbn = await Application.Current.MainPage.DisplayPromptAsync("Nuevo libro", "Introduce el ISBN:");
            string anioStr = await Application.Current.MainPage.DisplayPromptAsync("Nuevo libro", "Introduce el año:");
            int.TryParse(anioStr, out int anio);

            var nuevoLibro = new Libro
            {
                Titulo = titulo,
                Autor = autor,
                Genero = genero,
                ISBN = isbn,
                Anio = anio
            };

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "biblioteca.db");
            await DatabaseService.InitAsync(dbPath);
            await DatabaseService.InsertBooksAsync(nuevoLibro);

            await LoadLibrosAsync();
        }

        private async Task EditarLibroAsync()
        {
            if (SelectedIndex < 0 || SelectedIndex >= Libros.Count)
                return;

            var libroSeleccionado = Libros[SelectedIndex];

            var navParams = new Dictionary<string, object>
    {
        { "Libro", libroSeleccionado }
    };

            await Shell.Current.GoToAsync(nameof(LibroFormPage), navParams);
        }


    }

}

using System.Collections.ObjectModel;
using System.Windows.Input;
using BibliotecaProyectoIntegrado.Models;
using BibliotecaProyectoIntegrado.Services;

namespace BibliotecaProyectoIntegrado.ViewModels
{
    public class LibrosViewModel : BaseViewModel
    {
        public ObservableCollection<Libro> Libros { get; set; } = new();
        public ICommand AgregarLibroCommand { get; }
        public ICommand EditarLibroCommand { get; }


        public LibrosViewModel()
        {
            _ = LoadLibrosAsync();
            AgregarLibroCommand = new Command(async () => await AgregarLibroAsync());
            EditarLibroCommand = new Command(async () => await EditarLibroAsync());
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

            string nuevoTitulo = await Application.Current.MainPage.DisplayPromptAsync("Editar libro", "Nuevo título:", initialValue: libroSeleccionado.Titulo);
            if (string.IsNullOrWhiteSpace(nuevoTitulo)) return;

            string nuevoAutor = await Application.Current.MainPage.DisplayPromptAsync("Editar libro", "Nuevo autor:", initialValue: libroSeleccionado.Autor);
            if (string.IsNullOrWhiteSpace(nuevoAutor)) return;

            string nuevoGenero = await Application.Current.MainPage.DisplayPromptAsync("Editar libro", "Nuevo género:", initialValue: libroSeleccionado.Genero);
            string nuevoISBN = await Application.Current.MainPage.DisplayPromptAsync("Editar libro", "Nuevo ISBN:", initialValue: libroSeleccionado.ISBN);
            string nuevoAnioStr = await Application.Current.MainPage.DisplayPromptAsync("Editar libro", "Nuevo año:", initialValue: libroSeleccionado.Anio.ToString());
            int.TryParse(nuevoAnioStr, out int nuevoAnio);

            libroSeleccionado.Titulo = nuevoTitulo;
            libroSeleccionado.Autor = nuevoAutor;
            libroSeleccionado.Genero = nuevoGenero;
            libroSeleccionado.ISBN = nuevoISBN;
            libroSeleccionado.Anio = nuevoAnio;

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "biblioteca.db");
            await DatabaseService.InitAsync(dbPath);
            await DatabaseService.UpdateBooksAsync(libroSeleccionado);

            await LoadLibrosAsync();
        }
    }

}

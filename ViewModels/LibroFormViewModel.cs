using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BibliotecaProyectoIntegrado.Models;
using BibliotecaProyectoIntegrado.Services;
using CommunityToolkit.Mvvm.Messaging;

namespace BibliotecaProyectoIntegrado.ViewModels;

public partial class LibroFormViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private Libro libroEditando = new();

    [RelayCommand]
    private async Task GuardarAsync()
    {
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "biblioteca.db");
        await DatabaseService.InitAsync(dbPath);

        if (LibroEditando.Id == 0)
            await DatabaseService.InsertBooksAsync(LibroEditando);
        else
            await DatabaseService.UpdateBooksAsync(LibroEditando);

        // Notificar a otras vistas
        WeakReferenceMessenger.Default.Send(new LibroActualizadoMessage(LibroEditando));

        await Shell.Current.GoToAsync("..");
    }

    // ⬇️ Esto permite recibir el libro desde la navegación
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Libro", out var libro) && libro is Libro libroParam)
        {
            LibroEditando = new Libro
            {
                Id = libroParam.Id,
                Titulo = libroParam.Titulo,
                Autor = libroParam.Autor,
                Genero = libroParam.Genero,
                ISBN = libroParam.ISBN,
                Anio = libroParam.Anio,
                Descripcion = libroParam.Descripcion,
                Imagen = libroParam.Imagen
            };
        }
    }
}

using System.Collections.ObjectModel;
using BibliotecaProyectoIntegrado.Models;
using BibliotecaProyectoIntegrado.Services;

namespace BibliotecaProyectoIntegrado.ViewModels
{
    public class InventarioViewModel : BaseViewModel
    {
        public ObservableCollection<InventarioExtendido> InventarioLista { get; set; } = new();

        public InventarioViewModel()
        {
            CargarInventario();
        }

        private async void CargarInventario()
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "biblioteca.db");
            await DatabaseService.InitAsync(dbPath);

            var datos = await DatabaseService.GetInventarioExtendidoAsync();
            InventarioLista = new ObservableCollection<InventarioExtendido>(datos);
            OnPropertyChanged(nameof(InventarioLista));
        }
    }
}

public class InventarioExtendido
{
    public int InventarioId { get; set; }
    public Libro? Libro { get; set; }
    public string? Status { get; set; }
}


using BibliotecaProyectoIntegrado.Models;
using BibliotecaProyectoIntegrado.Services;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace BibliotecaProyectoIntegrado.ViewModels;

public class InventarioViewModel : BaseViewModel
{
    public ObservableCollection<InventarioExtendido> Inventario { get; set; } = new();

    public ICommand CambiarEstadoCommand { get; }
    public ICommand ImprimirPdfCommand { get; }


    public InventarioViewModel()
    {
        WeakReferenceMessenger.Default.Register<LibroActualizadoMessage>(this, async (r, m) =>
        {
            await ActualizarInventarioAsync(m.Value);
        });

        CambiarEstadoCommand = new Command<int>(async (inventarioId) =>
        {
            try
            {
                var item = Inventario.FirstOrDefault(i => i.InventarioId == inventarioId);
                if (item != null)
                {
                    var inventarioReal = await DatabaseService.GetInventarioByIdAsync(inventarioId);
                    if (inventarioReal != null)
                    {
                        inventarioReal.Status = inventarioReal.Status == "Disponible" ? "Prestado" : "Disponible";
                        await DatabaseService.UpdateInventarioAsync(inventarioReal);

                        bool algunoPrestado = await DatabaseService.HayEjemplaresPrestadosAsync(item.Libro.Id);
                        item.Status = algunoPrestado ? "Prestado" : "Disponible";

                        // ✅ Notificamos a otras páginas
                        WeakReferenceMessenger.Default.Send(new LibroActualizadoMessage(item.Libro));

                        OnPropertyChanged(nameof(Inventario));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al cambiar estado: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo actualizar el inventario: {ex.Message}", "OK");
            }
        });


        ImprimirPdfCommand = new Command(async () => await ImprimirPdfAsync());


        LoadInventario();
    }

    private async Task ActualizarInventarioAsync(Libro libroActualizado)
    {
        var item = Inventario.FirstOrDefault(i => i.Libro.Id == libroActualizado.Id);
        if (item != null)
        {
            bool algunoPrestado = await DatabaseService.HayEjemplaresPrestadosAsync(libroActualizado.Id);
            item.Status = algunoPrestado ? "Prestado" : "Disponible";
            OnPropertyChanged(nameof(Inventario));
        }
    }


    private async Task ImprimirPdfAsync()
    {
        var lista = await DatabaseService.GetInventarioExtendidoAsync();
        await PdfService.ExportarInventarioAsync(lista);
    }

    private async void LoadInventario()
    {
        var lista = await DatabaseService.GetInventarioExtendidoAsync();

        // Limpia y agrega nuevos elementos en lugar de reasignar la colección
        Inventario.Clear();
        foreach (var item in lista)
            Inventario.Add(item);
    }
}
using SQLite;

namespace BibliotecaProyectoIntegrado.Models;
public class Inventario
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int LibroId { get; set; }
    public string? Status { get; set; }
    public int Stock { get; set; }
    public int StockPrestado { get; set; }
    public int StockDisponible => Stock - StockPrestado;
}

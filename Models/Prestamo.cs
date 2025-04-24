using SQLite;

namespace BibliotecaProyectoIntegrado.Models;
public class Prestamo
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int LibroId { get; set; }
    public int UsuarioId { get; set; }
    public DateTime FechaPrestamo { get; set; }
    public DateTime? FechaDevolucion { get; set; }
}

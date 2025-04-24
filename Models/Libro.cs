using SQLite;

namespace BibliotecaProyectoIntegrado.Models;
public class Libro
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string? Titulo { get; set; }
    public string? Autor { get; set; }
    public string? Genero { get; set; }
    public string? ISBN { get; set; }
    public int Anio { get; set; }
    public string? Imagen { get; set; }
    public string? Descripcion { get; set; }

}

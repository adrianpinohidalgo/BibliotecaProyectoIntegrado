using SQLite;

namespace BibliotecaProyectoIntegrado.Models
{
    [Table("Inventario")]
    public class Inventario
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int LibroId { get; set; }

        public string Status { get; set; } = "Disponible";
    }
}
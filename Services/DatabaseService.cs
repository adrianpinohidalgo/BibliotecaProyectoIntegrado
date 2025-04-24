using SQLite;
using BibliotecaProyectoIntegrado.Models;
using BibliotecaProyectoIntegrado.ViewModels;

namespace BibliotecaProyectoIntegrado.Services
{
    public class DatabaseService
    {
        private static SQLiteAsyncConnection _database;

        public static async Task InitAsync(string dbPath)
        {
            if (_database is not null)
                return;

            _database = new SQLiteAsyncConnection(dbPath);
            await _database.CreateTableAsync<Libro>();
            await _database.CreateTableAsync<Usuario>();
            await _database.CreateTableAsync<Prestamo>();
            await _database.CreateTableAsync<Inventario>();

            bool isFirstRun = Preferences.Get("IsDbInitialized", false) == false;
            if (isFirstRun)
            {
                var books = new List<Libro>
                {
                    new() { Titulo = "The Hobbit", Autor = "J.R.R. Tolkien", Genero = "Fantasy", ISBN = "1234567890", Anio = 1937, Descripcion = "Un viaje inesperado." },
                    new() { Titulo = "1984", Autor = "George Orwell", Genero = "Fiction", ISBN = "1234567891", Anio = 1949, Descripcion = "Distopía totalitaria." },
                    new() { Titulo = "To Kill a Mockingbird", Autor = "Harper Lee", Genero = "Fiction", ISBN = "1234567892", Anio = 1960, Descripcion = "Justicia y racismo." },
                    new() { Titulo = "Dune", Autor = "Frank Herbert", Genero = "Science Fiction", ISBN = "1234567893", Anio = 1965, Descripcion = "Planeta desértico y poder." },
                    new() { Titulo = "Pride and Prejudice", Autor = "Jane Austen", Genero = "Romance", ISBN = "1234567894", Anio = 1813, Descripcion = "Orgullo, prejuicio y amor." },
                    new() { Titulo = "The Great Gatsby", Autor = "F. Scott Fitzgerald", Genero = "Fiction", ISBN = "1234567895", Anio = 1925, Descripcion = "El sueño americano." },
                    new() { Titulo = "Harry Potter y la Piedra Filosofal", Autor = "J.K. Rowling", Genero = "Fantasy", ISBN = "1234567896", Anio = 1997, Descripcion = "Magia en Hogwarts." },
                    new() { Titulo = "El nombre del viento", Autor = "Patrick Rothfuss", Genero = "Fantasy", ISBN = "1234567897", Anio = 2007, Descripcion = "La historia de Kvothe." },
                    new() { Titulo = "Fahrenheit 451", Autor = "Ray Bradbury", Genero = "Science Fiction", ISBN = "1234567898", Anio = 1953, Descripcion = "Libros prohibidos." },
                    new() { Titulo = "Crónica de una muerte anunciada", Autor = "Gabriel García Márquez", Genero = "Fiction", ISBN = "1234567899", Anio = 1981, Descripcion = "Tragedia anunciada." },
                    new() { Titulo = "Don Quijote de la Mancha", Autor = "Miguel de Cervantes", Genero = "Classic", ISBN = "1234567810", Anio = 1605, Descripcion = "Caballería y locura." },
                    new() { Titulo = "La sombra del viento", Autor = "Carlos Ruiz Zafón", Genero = "Mystery", ISBN = "1234567811", Anio = 2001, Descripcion = "Libros y secretos." },
                    new() { Titulo = "It", Autor = "Stephen King", Genero = "Horror", ISBN = "1234567812", Anio = 1986, Descripcion = "El terror en Derry." },
                    new() { Titulo = "Cien años de soledad", Autor = "Gabriel García Márquez", Genero = "Fiction", ISBN = "1234567813", Anio = 1967, Descripcion = "La saga de los Buendía." },
                    new() { Titulo = "Ready Player One", Autor = "Ernest Cline", Genero = "Science Fiction", ISBN = "1234567814", Anio = 2011, Descripcion = "Realidad virtual." },
                    new() { Titulo = "El psicoanalista", Autor = "John Katzenbach", Genero = "Thriller", ISBN = "1234567815", Anio = 2002, Descripcion = "Juego psicológico mortal." },
                    new() { Titulo = "El Código Da Vinci", Autor = "Dan Brown", Genero = "Thriller", ISBN = "1234567816", Anio = 2003, Descripcion = "Misterios religiosos." },
                    new() { Titulo = "Drácula", Autor = "Bram Stoker", Genero = "Horror", ISBN = "1234567817", Anio = 1897, Descripcion = "El vampiro original." },
                    new() { Titulo = "El señor de las moscas", Autor = "William Golding", Genero = "Fiction", ISBN = "1234567818", Anio = 1954, Descripcion = "Supervivencia infantil." },
                    new() { Titulo = "Los juegos del hambre", Autor = "Suzanne Collins", Genero = "Dystopia", ISBN = "1234567819", Anio = 2008, Descripcion = "Lucha por sobrevivir." }
                };

                await _database.InsertAllAsync(books);

                var random = new Random();

                foreach (var book in books)
                {
                    int copies = random.Next(1, 4); // entre 1 y 3 copias
                    for (int i = 0; i < copies; i++)
                    {
                        await _database.InsertAsync(new Inventario
                        {
                            LibroId = book.Id,
                            Status = "Disponible"
                        });
                    }
                }

            
                await _database.InsertAsync(new Usuario { Nombre = "Adrian Pino", Email = "adrian@gmail.com", NumeroSocio = "U001" });

                Preferences.Set("IsDbInitialized", true);
            }
        }

        public static Task<List<Libro>> GetBooksAsync() => _database.Table<Libro>().ToListAsync();

        public static Task InsertBooksAsync(Libro libro) => _database.InsertAsync(libro);
        public static Task UpdateBooksAsync(Libro libro) => _database.UpdateAsync(libro);

        public static Task<int> GetAvailableBooksCountAsync() =>
            _database.Table<Inventario>().Where(i => i.Status == "Disponible").CountAsync();

        public static Task<int> GetActiveLoansCountAsync() =>
            _database.Table<Prestamo>().Where(l => l.FechaDevolucion == null).CountAsync();

        public static async Task<List<PrestamoExtendido>> GetPrestamosExtendidosAsync()
        {
            var todosLosPrestamos = await _database.Table<Prestamo>().ToListAsync();

            var prestamos = todosLosPrestamos
                .Where(p => p.FechaDevolucion == null || p.FechaPrestamo.AddDays(15) < DateTime.Now)
                .ToList();

            var extendidos = new List<PrestamoExtendido>();
            foreach (var p in prestamos)
            {
                var libro = await _database.Table<Libro>().Where(l => l.Id == p.LibroId).FirstOrDefaultAsync();
                var usuario = await _database.Table<Usuario>().Where(u => u.Id == p.UsuarioId).FirstOrDefaultAsync();

                extendidos.Add(new PrestamoExtendido
                {
                    Id = p.Id,
                    LibroId = p.LibroId,
                    UsuarioId = p.UsuarioId,
                    FechaPrestamo = p.FechaPrestamo,
                    FechaDevolucion = p.FechaDevolucion,
                    Libro = libro,
                    Usuario = usuario
                });
            }

            return extendidos;
        }

        public static Task UpdatePrestamoAsync(Prestamo p) => _database.UpdateAsync(p);

        public static async Task RegistrarPrestamoAsync(int libroId, int usuarioId)
        {
            var inventarioDisponible = await _database.Table<Inventario>()
                .Where(i => i.LibroId == libroId && i.Status == "Disponible")
                .FirstOrDefaultAsync();

            if (inventarioDisponible == null)
                throw new Exception("No hay ejemplares disponibles.");

            var nuevoPrestamo = new Prestamo
            {
                LibroId = libroId,
                UsuarioId = usuarioId,
                FechaPrestamo = DateTime.Now,
                FechaDevolucion = null
            };

            await _database.InsertAsync(nuevoPrestamo);

            // Actualiza inventario
            inventarioDisponible.Status = "Prestado";
            await _database.UpdateAsync(inventarioDisponible);
        }

        public static async Task<List<Libro>> GetLibrosDisponiblesAsync()
        {
            var inventario = await _database.Table<Inventario>().Where(i => i.Status == "Disponible").ToListAsync();
            var librosDisponibles = new List<Libro>();

            foreach (var item in inventario)
            {
                var libro = await _database.Table<Libro>().Where(l => l.Id == item.LibroId).FirstOrDefaultAsync();
                if (libro != null && !librosDisponibles.Any(l => l.Id == libro.Id))
                    librosDisponibles.Add(libro);
            }

            return librosDisponibles;
        }

        public static Task<List<Usuario>> GetUsuariosAsync() => _database.Table<Usuario>().ToListAsync();

        public static async Task<List<InventarioExtendido>> GetInventarioExtendidoAsync()
        {
            var inventario = await _database.Table<Inventario>().ToListAsync();
            var resultado = new List<InventarioExtendido>();

            foreach (var item in inventario)
            {
                var libro = await _database.Table<Libro>().Where(l => l.Id == item.LibroId).FirstOrDefaultAsync();
                if (libro != null)
                {
                    resultado.Add(new InventarioExtendido
                    {
                        InventarioId = item.Id,
                        Libro = libro,
                        Status = item.Status
                    });
                }
            }

            return resultado;
        }

    }
}

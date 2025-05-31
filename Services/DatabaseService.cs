using SQLite;
using BibliotecaProyectoIntegrado.Models;
using BibliotecaProyectoIntegrado.ViewModels;
using Plugin.LocalNotification;

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
                    new() { Titulo = "The Hobbit", Autor = "J.R.R. Tolkien", Genero = "Fantasy", ISBN = "1234567890", Anio = 1937, Descripcion = "Un viaje inesperado.", Imagen = "https://images-na.ssl-images-amazon.com/images/I/712cDO7d73L.jpg" },
                    new() { Titulo = "1984", Autor = "George Orwell", Genero = "Fiction", ISBN = "1234567891", Anio = 1949, Descripcion = "Distopía totalitaria.", Imagen = "https://images-na.ssl-images-amazon.com/images/I/71kxa1-0mfL.jpg" },
                    new() { Titulo = "To Kill a Mockingbird", Autor = "Harper Lee", Genero = "Fiction", ISBN = "1234567892", Anio = 1960, Descripcion = "Justicia y racismo.", Imagen = "https://images-na.ssl-images-amazon.com/images/I/71FxgtFKcQL.jpg" },
                    new() { Titulo = "Dune", Autor = "Frank Herbert", Genero = "Science Fiction", ISBN = "1234567893", Anio = 1965, Descripcion = "Planeta desértico y poder.", Imagen = "https://images-na.ssl-images-amazon.com/images/I/81zN7udGRUL.jpg" },
                    new() { Titulo = "Pride and Prejudice", Autor = "Jane Austen", Genero = "Romance", ISBN = "1234567894", Anio = 1813, Descripcion = "Orgullo, prejuicio y amor.", Imagen = "https://images-na.ssl-images-amazon.com/images/I/712P0p5cXIS.jpg" },
                    new() { Titulo = "The Great Gatsby", Autor = "F. Scott Fitzgerald", Genero = "Fiction", ISBN = "1234567895", Anio = 1925, Descripcion = "El sueño americano.", Imagen = "https://images-na.ssl-images-amazon.com/images/I/81af+MCATTL.jpg" },
                    new() { Titulo = "Harry Potter y la Piedra Filosofal", Autor = "J.K. Rowling", Genero = "Fantasy", ISBN = "1234567896", Anio = 1997, Descripcion = "Magia en Hogwarts.", Imagen = "https://images-na.ssl-images-amazon.com/images/I/81YOuOGFCJL.jpg" },
                    new() { Titulo = "El nombre del viento", Autor = "Patrick Rothfuss", Genero = "Fantasy", ISBN = "1234567897", Anio = 2007, Descripcion = "La historia de Kvothe.", Imagen = "https://images-na.ssl-images-amazon.com/images/I/91dSMhdIzTL.jpg" },
                    new() { Titulo = "Fahrenheit 451", Autor = "Ray Bradbury", Genero = "Science Fiction", ISBN = "1234567898", Anio = 1953, Descripcion = "Libros prohibidos.", Imagen = "https://images-na.ssl-images-amazon.com/images/I/71OFqSRFDgL.jpg" },
                    new() { Titulo = "Crónica de una muerte anunciada", Autor = "Gabriel García Márquez", Genero = "Fiction", ISBN = "1234567899", Anio = 1981, Descripcion = "Tragedia anunciada.", Imagen = "https://images-na.ssl-images-amazon.com/images/I/71XHZB2RjBL.jpg" },
                    new() { Titulo = "Don Quijote de la Mancha", Autor = "Miguel de Cervantes", Genero = "Classic", ISBN = "1234567810", Anio = 1605, Descripcion = "Caballería y locura.", Imagen = "https://images-na.ssl-images-amazon.com/images/I/71-++hbbERL.jpg" },
                    new() { Titulo = "La sombra del viento", Autor = "Carlos Ruiz Zafón", Genero = "Mystery", ISBN = "1234567811", Anio = 2001, Descripcion = "Libros y secretos.", Imagen = "https://images-na.ssl-images-amazon.com/images/I/81rRDrZnucL.jpg" },
                    new() { Titulo = "It", Autor = "Stephen King", Genero = "Horror", ISBN = "1234567812", Anio = 1986, Descripcion = "El terror en Derry.", Imagen = "https://images-na.ssl-images-amazon.com/images/I/71QKQ9mwV7L.jpg" },
                    new() { Titulo = "Cien años de soledad", Autor = "Gabriel García Márquez", Genero = "Fiction", ISBN = "1234567813", Anio = 1967, Descripcion = "La saga de los Buendía.", Imagen = "https://images-na.ssl-images-amazon.com/images/I/81MI6+TpYzL.jpg" },
                    new() { Titulo = "Ready Player One", Autor = "Ernest Cline", Genero = "Science Fiction", ISBN = "1234567814", Anio = 2011, Descripcion = "Realidad virtual.", Imagen = "https://images-na.ssl-images-amazon.com/images/I/A1JVqNMI7UL.jpg" },
                    new() { Titulo = "El psicoanalista", Autor = "John Katzenbach", Genero = "Thriller", ISBN = "1234567815", Anio = 2002, Descripcion = "Juego psicológico mortal.", Imagen = "https://images-na.ssl-images-amazon.com/images/I/71vFKBEi6HL.jpg" },
                    new() { Titulo = "El Código Da Vinci", Autor = "Dan Brown", Genero = "Thriller", ISBN = "1234567816", Anio = 2003, Descripcion = "Misterios religiosos.", Imagen = "https://images-na.ssl-images-amazon.com/images/I/815WORuYMML.jpg" },
                    new() { Titulo = "Drácula", Autor = "Bram Stoker", Genero = "Horror", ISBN = "1234567817", Anio = 1897, Descripcion = "El vampiro original.", Imagen = "https://images-na.ssl-images-amazon.com/images/I/81F90H7hnML.jpg" },
                    new() { Titulo = "El señor de las moscas", Autor = "William Golding", Genero = "Fiction", ISBN = "1234567818", Anio = 1954, Descripcion = "Supervivencia infantil.", Imagen = "https://images-na.ssl-images-amazon.com/images/I/81P6QlauN3L.jpg" },
                    new() { Titulo = "Los juegos del hambre", Autor = "Suzanne Collins", Genero = "Dystopia", ISBN = "1234567819", Anio = 2008, Descripcion = "Lucha por sobrevivir.", Imagen = "https://images-na.ssl-images-amazon.com/images/I/71un2hI4mcL.jpg" }
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


                await _database.InsertAsync(new Usuario
                {
                    Nombre = "Adrian Pino",
                    Email = "adrian@gmail.com",
                    NumeroSocio = "U001",
                    Contrasena = "1234" // CONTRASEÑA DE EJEMPLO
                });

                await _database.InsertAsync(new Usuario
                {
                    Nombre = "Pepe Lopez",
                    Email = "pepe@gmail.com",
                    NumeroSocio = "U002",
                    Contrasena = "1234" // CONTRASEÑA DE EJEMPLO
                });

                await _database.InsertAsync(new Usuario
                {
                    Nombre = "Maria Perez",
                    Email = "maria@gmail.com",
                    NumeroSocio = "U003",
                    Contrasena = "1234" // CONTRASEÑA DE EJEMPLO
                });


                Preferences.Set("IsDbInitialized", false);
            }
        }

        public static Task<List<Libro>> GetBooksAsync() => _database.Table<Libro>().ToListAsync();

        public static Task InsertBooksAsync(Libro libro) => _database.InsertAsync(libro);
        public static Task UpdateBooksAsync(Libro libro) => _database.UpdateAsync(libro);

        public static Task<int> GetAvailableBooksCountAsync() =>
            _database.Table<Inventario>().Where(i => i.Status == "Disponible").CountAsync();

        public static Task<int> GetActiveLoansCountAsync() =>
            _database.Table<Prestamo>().Where(l => l.FechaDevolucion == null).CountAsync();

        public static async Task<List<PrestamoExtendido>> GetPrestamosExtendidosDelUsuarioAsync(int usuarioId)
        {
            var prestamos = await _database.Table<Prestamo>()
                .Where(p => p.UsuarioId == usuarioId && p.FechaDevolucion == null)
                .ToListAsync();

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

        // En DatabaseService.cs
        public static async Task<(bool Exito, Libro? Libro, Usuario? Usuario)> RegistrarPrestamoYObtenerDatosAsync(int libroId, int usuarioId)
        {
            var inventarioDisponible = await _database.Table<Inventario>()
                .Where(i => i.LibroId == libroId && i.Status == "Disponible")
                .FirstOrDefaultAsync();

            if (inventarioDisponible == null)
                return (false, null, null);

            var nuevoPrestamo = new Prestamo
            {
                LibroId = libroId,
                UsuarioId = usuarioId,
                FechaPrestamo = DateTime.Now,
                FechaDevolucion = null
            };

            await _database.InsertAsync(nuevoPrestamo);

            inventarioDisponible.Status = "Prestado";
            await _database.UpdateAsync(inventarioDisponible);

            var libro = await _database.Table<Libro>().Where(l => l.Id == libroId).FirstOrDefaultAsync();
            var usuario = await _database.Table<Usuario>().Where(u => u.Id == usuarioId).FirstOrDefaultAsync();

            return (true, libro, usuario);
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

        //USUARIOS
        public static async Task<Usuario?> LoginAsync(string email, string contrasena)
        {
            return await _database.Table<Usuario>()
                .Where(u => u.Email == email && u.Contrasena == contrasena)
                .FirstOrDefaultAsync();
        }

        public static Task<List<Prestamo>> GetPrestamosDelUsuarioAsync(int usuarioId)
        {
            return _database.Table<Prestamo>()
                .Where(p => p.UsuarioId == usuarioId && p.FechaDevolucion == null)
                .ToListAsync();
        }


        public static Task<Inventario?> GetInventarioPorLibroAsync(int libroId)
        {
            return _database.Table<Inventario>()
                .Where(i => i.LibroId == libroId && i.Status == "Prestado")
                .FirstOrDefaultAsync();
        }

        public static async Task<string> GetEstadoInventarioAsync(int libroId)
        {
            var inventarios = await _database.Table<Inventario>()
                .Where(i => i.LibroId == libroId)
                .ToListAsync();

            if (inventarios.Any())
            {
                int total = inventarios.Count;
                int disponibles = inventarios.Count(i => i.Status == "Disponible");
                return $"{disponibles} disponibles de {total}";
            }

            return "0 disponibles de 0";
        }

        public static async Task<bool> UpdateInventarioAsync(Inventario inventario)
        {
            try
            {
                // Aquí usamos una consulta SQL directa para evitar problemas de serialización
                string query = $"UPDATE Inventario SET Status = ? WHERE Id = ?";
                int result = await _database.ExecuteAsync(query, inventario.Status, inventario.Id);
                return result > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en UpdateInventarioAsync: {ex.Message}");
                throw;
            }
        }

        public static async Task<Inventario> GetInventarioByIdAsync(int inventarioId)
        {
            try
            {
                return await _database.Table<Inventario>()
                    .Where(i => i.Id == inventarioId)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en GetInventarioByIdAsync: {ex.Message}");
                throw;
            }
        }


        public static async Task<List<InventarioExtendido>> GetInventarioExtendidoAsync()
        {
            try
            {
                var resultado = new List<InventarioExtendido>();
                var libros = await _database.Table<Libro>().ToListAsync();

                foreach (var libro in libros)
                {
                    var inventarios = await _database.Table<Inventario>()
                        .Where(i => i.LibroId == libro.Id)
                        .ToListAsync();

                    bool algunoPrestado = inventarios.Any(i => i.Status == "Prestado");

                    resultado.Add(new InventarioExtendido
                    {
                        InventarioId = inventarios.FirstOrDefault()?.Id ?? 0,
                        Libro = libro,
                        Status = algunoPrestado ? "Prestado" : "Disponible"
                    });
                }

                return resultado;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en GetInventarioExtendidoAsync: {ex.Message}");
                throw;
            }
        }

        //public static async Task<bool> HayEjemplaresPrestadosAsync(int libroId)
        //{
        //    var inventarios = await _database.Table<Inventario>()
        //        .Where(i => i.LibroId == libroId)
        //        .ToListAsync();

        //    return inventarios.Any(i => i.Status == "Prestado");
        //}

        public static async Task<bool> HayEjemplaresPrestadosAsync(int libroId)
        {
            var inventarios = await _database.Table<Inventario>()
                .Where(i => i.LibroId == libroId && i.Status == "Prestado")
                .CountAsync();
            return inventarios > 0;
        }

        public static async Task<List<string>> GetTableNamesAsync()
        {
            var result = await _database.QueryAsync<TableInfo>("SELECT name FROM sqlite_master WHERE type='table'");
            return result.Select(r => r.name).ToList();
        }

        public class TableInfo
        {
            public string name { get; set; }
        }

        // Agregar estos métodos a tu DatabaseService

        public static async Task<List<Prestamo>> GetPrestamosActivosAsync()
        {
            var prestamos = await _database.Table<Prestamo>()
                .Where(p => p.FechaDevolucion == null) // Solo préstamos sin devolver
                .ToListAsync();
            return prestamos;
        }

        public static async Task<List<PrestamoExtendido>> GetTodosLosPrestamosExtendidosAsync()
        {
            var prestamos = await _database.Table<Prestamo>()
                .Where(p => p.FechaDevolucion == null)
                .ToListAsync();

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


    }
}

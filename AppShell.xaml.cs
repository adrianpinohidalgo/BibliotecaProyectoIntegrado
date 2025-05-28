using BibliotecaProyectoIntegrado.Services;
using BibliotecaProyectoIntegrado.Views;
namespace BibliotecaProyectoIntegrado;
public partial class AppShell : Shell
{
    private static bool rutasRegistradas = false;

    public AppShell()
    {
        InitializeComponent();
        // Ejecutar la inicialización de la BD de forma asíncrona pero no esperar aquí
        Task.Run(async () => await InicializarAplicacionAsync());
    }

    private async Task InicializarAplicacionAsync()
    {
        try
        {
            // 1. Registra rutas primero
            if (!rutasRegistradas)
            {
                MainThread.BeginInvokeOnMainThread(() => {
                    Routing.RegisterRoute("WelcomePage", typeof(WelcomePage));
                    Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
                    Routing.RegisterRoute("LoginPage", typeof(LoginPage));
                    Routing.RegisterRoute(nameof(LibroFormPage), typeof(LibroFormPage));
                    // otras rutas...
                    rutasRegistradas = true;
                });
            }

            // 2. Inicializar la base de datos
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "biblioteca.db");

            // IMPORTANTE: No borrar la DB en cada inicio si ya existe
            // Solo para desarrollo/pruebas, quitar en producción
            //if (File.Exists(dbPath)) File.Delete(dbPath);

            // Inicializar la base de datos y ESPERAR que termine
            await DatabaseService.InitAsync(dbPath);

            // 3. Después de la inicialización, configurar la UI según el usuario
            int usuarioId = Preferences.Get("UsuarioId", 0);
            var usuarios = await DatabaseService.GetUsuariosAsync();
            var usuario = usuarios.FirstOrDefault(u => u.Id == usuarioId);

            if (usuario != null && usuario.NumeroSocio == "U001")
            {
                MainThread.BeginInvokeOnMainThread(() => {
                    var inventarioTab = new ShellContent
                    {
                        Title = "Inventario",
                        Icon = "inventory.png",
                        Route = "InventarioPage",
                        ContentTemplate = new DataTemplate(typeof(Views.InventarioPage))
                    };

                    if (this.Items.FirstOrDefault() is TabBar tabBar)
                    {
                        tabBar.Items.Add(inventarioTab);
                    }
                });
            }

            // 4. Verificar que las tablas se hayan creado correctamente
            var tablas = await DatabaseService.GetTableNamesAsync();
            foreach (var tabla in tablas)
            {
                System.Diagnostics.Debug.WriteLine($"Tabla encontrada: {tabla}");
            }
        }
        catch (Exception ex)
        {
            // Manejar excepciones durante la inicialización
            System.Diagnostics.Debug.WriteLine($"Error inicializando la aplicación: {ex.Message}");
            MainThread.BeginInvokeOnMainThread(() => {
                Application.Current.MainPage.DisplayAlert("Error", $"Error inicializando la aplicación: {ex.Message}", "OK");
            });
        }
    }
}
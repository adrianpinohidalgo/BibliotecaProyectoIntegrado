using BibliotecaProyectoIntegrado.Services;
using BibliotecaProyectoIntegrado.Views;

namespace BibliotecaProyectoIntegrado;

public partial class AppShell : Shell
{
    private static bool rutasRegistradas = false;

    public AppShell()
    {
        InitializeComponent();
        InicializarShellAsync(); // llamado asincrónico seguro



        if (!rutasRegistradas)
        {
            Routing.RegisterRoute("WelcomePage", typeof(WelcomePage));
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute("LoginPage", typeof(LoginPage));
            Routing.RegisterRoute(nameof(LibroFormPage), typeof(LibroFormPage));

            // otras rutas...
            rutasRegistradas = true;
        }

    }

    private async void InicializarShellAsync()
    {
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "biblioteca.db");
        await DatabaseService.InitAsync(dbPath);

        int usuarioId = Preferences.Get("UsuarioId", 0);
        var usuarios = await DatabaseService.GetUsuariosAsync();
        var usuario = usuarios.FirstOrDefault(u => u.Id == usuarioId);

        if (usuario != null && usuario.NumeroSocio == "U001")
        {
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
        }
    }
}

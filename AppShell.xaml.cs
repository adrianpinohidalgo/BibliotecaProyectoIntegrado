using BibliotecaProyectoIntegrado.Views;

namespace BibliotecaProyectoIntegrado;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("WelcomePage", typeof(WelcomePage));
        Routing.RegisterRoute("MainPage", typeof(MainPage));

    }
}

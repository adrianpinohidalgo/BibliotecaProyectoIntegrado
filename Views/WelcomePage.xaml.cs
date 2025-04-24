namespace BibliotecaProyectoIntegrado.Views;

public partial class WelcomePage : ContentPage
{
    public WelcomePage()
    {
        InitializeComponent();
    }

    private void OnGetStartedClicked(object sender, EventArgs e)
    {
        // Marca que el usuario ya pasó por la pantalla de bienvenida
        Preferences.Set("IsFirstLaunch", false);

        // Cambia la MainPage a AppShell para mostrar la navegación con tabs
        Application.Current.MainPage = new AppShell();
    }
}

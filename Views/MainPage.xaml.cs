namespace BibliotecaProyectoIntegrado.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void OnResetWelcomeClicked(object sender, EventArgs e)
    {
        Preferences.Set("IsFirstLaunch", true);
        Application.Current.MainPage = new NavigationPage(new WelcomePage());
    }

    private async void OnLogoutTapped(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Cerrar sesión", "¿Deseas cerrar sesión?", "Sí", "Cancelar");
        if (!confirm) return;

        Preferences.Remove("IsLoggedIn");
        Preferences.Remove("UsuarioId");

        Application.Current.MainPage = new LoginPage(); // Navega a la pantalla de login
    }


}

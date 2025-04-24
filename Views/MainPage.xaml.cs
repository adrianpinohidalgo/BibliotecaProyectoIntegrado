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

}

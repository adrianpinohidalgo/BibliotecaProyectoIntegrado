using BibliotecaProyectoIntegrado.Views;

namespace BibliotecaProyectoIntegrado
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Si es la primera vez que se ejecuta la app
            if (Preferences.Get("IsFirstLaunch", true))
            {
                MainPage = new NavigationPage(new WelcomePage());
            }
            else if (Preferences.Get("IsLoggedIn", false))
            {
                MainPage = new AppShell();
            }
            else
            {
                MainPage = new NavigationPage(new LoginPage());
            }

        }
    }
}

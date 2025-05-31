using BibliotecaProyectoIntegrado.Services;
using BibliotecaProyectoIntegrado.Views;

namespace BibliotecaProyectoIntegrado.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();

            Preferences.Set("IsDbInitialized", false);


        }

        private async void OnResetWelcomeClicked(object sender, EventArgs e)
        {
            Preferences.Set("IsFirstLaunch", true);
            Application.Current.MainPage = new NavigationPage(new WelcomePage());
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "biblioteca.db3");
            await DatabaseService.InitAsync(dbPath);
        }


        private async void OnLoginClicked(object sender, EventArgs e)
        {
            var email = EmailEntry.Text?.Trim();
            var password = ContrasenaEntry.Text?.Trim();

            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(email))
            {
                ErrorLabel.Text = "Todos los campos son obligatorios.";
                ErrorLabel.IsVisible = true;
                return;
            }

            var usuario = await DatabaseService.LoginAsync(email, password);

            if (usuario != null)
            {
                Preferences.Set("IsLoggedIn", true);
                Preferences.Set("UsuarioId", usuario.Id);
                Application.Current.MainPage = new AppShell(); // Cambia a la interfaz principal
            }
            else
            {
                ErrorLabel.Text = "Credenciales incorrectas.";
                ErrorLabel.IsVisible = true;
            }
        }
    }
}

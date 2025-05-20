using Plugin.LocalNotification;

namespace BibliotecaProyectoIntegrado.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnResetWelcomeClicked(object sender, EventArgs e)
    {
        //Preferences.Set("IsFirstLaunch", true);
        //Application.Current.MainPage = new NavigationPage(new WelcomePage());
        await ProbarNotificacionAsync();

    }

    public async Task ProbarNotificacionAsync()
    {
        try
        {
            // Verificar permisos
            if (!await LocalNotificationCenter.Current.AreNotificationsEnabled())
            {
                await LocalNotificationCenter.Current.RequestNotificationPermission();
            }

            // Crear notificación
            var notification = new NotificationRequest
            {
                NotificationId = (int)(DateTime.Now.Ticks % int.MaxValue),
                Title = "¡Notificación de prueba!",
                Description = "Si estás viendo esto, ¡funciona correctamente!",
                ReturningData = "noti_dummy", // Solo si usas OnNotificationActionTapped
                BadgeNumber = 1,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddSeconds(1), // Casi inmediato
                    NotifyRepeatInterval = TimeSpan.Zero,
                    RepeatType = NotificationRepeat.No
                }
            };

            // Mostrar notificación
            await LocalNotificationCenter.Current.Show(notification);

            // Confirmación visual
            await Shell.Current.DisplayAlert("Test enviada", "Espera unos segundos...", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"No se pudo enviar: {ex.Message}", "OK");
        }
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

using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.View;

namespace BibliotecaProyectoIntegrado
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Cambiar color de status bar
            Window?.SetStatusBarColor(Android.Graphics.Color.ParseColor("#7EBB94"));

            // Para texto claro en status bar
            WindowCompat.GetInsetsController(Window, Window.DecorView)
                .AppearanceLightStatusBars = false;
        }
    }
}

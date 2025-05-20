using Android.App;
using Android.Runtime;
[assembly: UsesPermission(Android.Manifest.Permission.Vibrate)]
[assembly: UsesPermission("android.permission.POST_NOTIFICATIONS")]
[assembly: UsesPermission(Android.Manifest.Permission.WakeLock)]
[assembly: UsesPermission(Android.Manifest.Permission.ReceiveBootCompleted)]


namespace BibliotecaProyectoIntegrado
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}

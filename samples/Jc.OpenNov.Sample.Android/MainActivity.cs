using Android.App;
using Android.Content.PM;
using Avalonia;
using Avalonia.Android;
using Jc.OpenNov.Nfc.Android;

namespace Jc.OpenNov.Sample.Android;

[Activity(
    Label = "Jc.OpenNov.Sample.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App>
{
    internal static NfcController NfcController;
    
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .UseOpenNovo(this)
            .WithInterFont();
    }
}

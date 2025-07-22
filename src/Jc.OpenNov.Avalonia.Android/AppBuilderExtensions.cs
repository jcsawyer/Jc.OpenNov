using Avalonia;
using Jc.OpenNov.Nfc.Android;
using Activity = Android.App.Activity;

namespace Jc.OpenNov.Avalonia.Android;

public static class AppBuilderExtensions
{
    public static AppBuilder UseOpenNov(this AppBuilder appBuilder, Activity activity, Action<Action>? runner = null)
    {
        return appBuilder.AfterSetup(_ =>
        {
            OpenNov.Current.Nfc = new Nfc(new NfcController(activity, runner));
        });
    }
}
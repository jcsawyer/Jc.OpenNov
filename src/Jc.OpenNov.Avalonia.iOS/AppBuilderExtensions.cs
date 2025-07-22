using Avalonia;
using Jc.OpenNov.Nfc.iOS;

namespace Jc.OpenNov.Avalonia.iOS;

public static class AppBuilderExtensions
{
    public static AppBuilder UseOpenNov(this AppBuilder appBuilder, Action<Action>? runner = null)
    {
        return appBuilder.AfterSetup(_ =>
        {
            OpenNov.Current.Nfc = new Nfc(new NfcController(runner));
        });
    }
}
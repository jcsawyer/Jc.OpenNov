using System;
using System.Diagnostics;
using Avalonia;
using Jc.OpenNov.Data;
using Jc.OpenNov.Nfc.Android;
using Activity = Android.App.Activity;

namespace Jc.OpenNov.Sample.Android;

internal static class AppBuilderExtensions
{
    public static AppBuilder UseOpenNovo(this AppBuilder appBuilder, Activity activity)
    {
        return appBuilder.AfterSetup(_ =>
        {
            MainActivity.NfcController = new NfcController(activity, action => action());
            MainActivity.NfcController.MonitorNfc(
                    onDataRead: result =>
                    {
                        Debug.WriteLine("Data read from NFC tag");
                        if (result is PenResult.Success success)
                        {
                            // Handle successful data read
                        }
                        else if (result is PenResult.Failure failure)
                        {
                            // Handle failure
                        }
                    },
                    onTagDetected: tag =>
                    {
                        Debug.WriteLine("NFC tag detected: " + tag.GetId());
                        // Handle tag detection
                    },
                    onDataSent: data =>
                    {
                        Debug.WriteLine("Data sent: " + BitConverter.ToString(data));
                        // Handle data sent
                    },
                    onDataReceived: data =>
                    {
                        Debug.WriteLine("Data received: " + BitConverter.ToString(data));
                        // Handle data received
                    },
                    onError: exception =>
                    {
                        Debug.WriteLine("Error occurred: " + exception.Message);
                        // Handle error
                    },
                    stopCondition: (model, doses) => false);
        });
    }
}
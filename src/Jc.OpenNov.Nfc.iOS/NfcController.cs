using CoreFoundation;
using CoreNFC;
using Jc.OpenNov.Data;

namespace Jc.OpenNov.Nfc.iOS;

public class NfcController : NSObject, INFCTagReaderSessionDelegate
{
    private readonly Action<Action> _runner;

    private NFCTagReaderSession? _session;
    private Func<string, List<InsulinDose>, bool>? _stopCondition;

    public event EventHandler<PenResult>? OnDataRead;
    public event EventHandler<INFCIso7816Tag>? OnTagDetected;
    public event EventHandler<byte[]>? OnDataSent;
    public event EventHandler<byte[]>? OnDataReceived;
    public event EventHandler<Exception>? OnError;

    public NfcController(Action<Action>? runner = null)
    {
        _runner = runner ?? (action => action());
    }

    public void MonitorNfc(Func<string, List<InsulinDose>, bool>? stopCondition = null)
    {
        stopCondition ??= (_, _) => false;
        _stopCondition = stopCondition;

        _session = new NFCTagReaderSession(NFCPollingOption.Iso14443 | NFCPollingOption.Iso15693, this,
            DispatchQueue.CurrentQueue)
        {
            AlertMessage = "Please hold your insulin pen near your device.",
        };

        _session?.BeginSession();
    }

    public void StopNfc()
    {
        _session?.InvalidateSession();
    }

    public void DidDetectTags(NFCTagReaderSession session, INFCTag[] tags)
    {
        var tag = tags.FirstOrDefault();
        if (tag is null || tag.Type != NFCTagType.Iso7816Compatible)
        {
            session.InvalidateSession("Unsupported tag type or no tag detected.");
            return;
        }
        _runner(() =>
        {
            Task.Run(() =>
            {
                session.ConnectTo(tag, async void (error) =>
                {
                    try
                    {
                        if (error is not null)
                        {
                            session.InvalidateSession(error.LocalizedDescription);
                            return;
                        }

                        var reader = new NfcDataReader(tag.AsNFCIso7816Tag);
                        var controller = new NvpController(reader);

                        reader.OnDataSent += (_, data) => OnDataSent?.Invoke(this, data);
                        reader.OnDataReceived += (_, data) => OnDataReceived?.Invoke(this, data);
                        OnTagDetected?.Invoke(this, tag.AsNFCIso7816Tag);


                        var result = await controller.DataRead(_stopCondition).ConfigureAwait(false);
                        OnDataRead?.Invoke(this, result);
                        session.InvalidateSession();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        session.InvalidateSession("Error reading tag");
                    }
                });
            });
        });
    }

    public void DidInvalidate(NFCTagReaderSession session, NSError error)
    {
        if (error is not null)
        {
            OnError?.Invoke(this, new NSErrorException(error));
        }
    }
}
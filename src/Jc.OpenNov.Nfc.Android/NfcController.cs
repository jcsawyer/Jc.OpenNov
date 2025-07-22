using Android.Nfc;
using Android.Nfc.Tech;
using Jc.OpenNov.Data;

namespace Jc.OpenNov.Nfc.Android;

public class NfcController
{
    private readonly Activity _activity;
    private readonly Action<Action> _runner;
    private readonly NfcAdapter? _nfcAdapter;

    public event EventHandler<PenResult>? OnDataRead;
    public event EventHandler<Tag>? OnTagDetected;
    public event EventHandler<byte[]>? OnDataSent;
    public event EventHandler<byte[]>? OnDataReceived;
    public event EventHandler<Exception>? OnError;

    public NfcController(Activity activity, Action<Action>? runner = null)
    {
        _activity = activity;
        _runner = runner ?? (action => action());
        _nfcAdapter = NfcAdapter.GetDefaultAdapter(activity);
        if (_nfcAdapter is null)
        {
            throw new InvalidOperationException("NFC is not supported on this device.");
        }
    }

    public void MonitorNfc(Func<string, List<InsulinDose>, bool>? stopCondition = null)
    {
        stopCondition ??= (_, _) => false;

        _nfcAdapter?.EnableReaderMode(
            _activity,
            new TagReaderCallbackImpl(tag =>
                {
                    if (tag is null)
                    {
                        OnError?.Invoke(this, new InvalidOperationException("Tag is null"));
                        return;
                    }

                    _runner(async void () =>
                    {
                        try
                        {
                            OnTagDetected?.Invoke(this, tag);
                            var isoDep = IsoDep.Get(tag);
                            if (isoDep != null)
                            {
                                isoDep.Connect();
                                isoDep.Timeout = 1000;

                                var dataReader = new NfcDataReader(isoDep);

                                dataReader.OnDataSent += (_, data) =>
                                    OnDataSent?.Invoke(this, data);
                                dataReader.OnDataReceived += (_, data) =>
                                    OnDataReceived?.Invoke(this, data);

                                var controller = new NvpController(dataReader);

                                var result = await controller.DataRead(stopCondition).ConfigureAwait(false);
                                OnDataRead?.Invoke(this, result);

                                isoDep.Close();
                            }
                            else
                            {
                                throw new InvalidOperationException("Incorrect tag detected");
                            }
                        }
                        catch (Exception e)
                        {
                            OnError?.Invoke(this, e);
                        }
                    });
                }
            ),
            NfcReaderFlags.NfcA | NfcReaderFlags.NfcV | NfcReaderFlags.SkipNdefCheck,
            null
        );
    }

    public void StopNfc()
    {
        _nfcAdapter?.DisableReaderMode(_activity);
    }

    private class TagReaderCallbackImpl : Java.Lang.Object, NfcAdapter.IReaderCallback
    {
        private readonly Action<Tag?> _callback;

        public TagReaderCallbackImpl(Action<Tag?> callback)
        {
            _callback = callback;
        }

        public void OnTagDiscovered(Tag? tag)
        {
            _callback(tag);
        }
    }
}
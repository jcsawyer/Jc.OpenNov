using Android.Nfc;
using Android.Nfc.Tech;
using Jc.OpenNov.Data;

namespace Jc.OpenNov.Nfc.Android;

public class NfcController
    {
        private readonly Activity _activity;
        private readonly Action<Action> _runner;
        private readonly NfcAdapter _nfcAdapter;

        public NfcController(Activity activity, Action<Action> runner = null)
        {
            _activity = activity;
            _runner = runner ?? (action => action());
            _nfcAdapter = NfcAdapter.GetDefaultAdapter(activity);
        }

        public void MonitorNfc(
            Action<PenResult> onDataRead,
            Action<Tag> onTagDetected = null,
            Action<byte[]> onDataSent = null,
            Action<byte[]> onDataReceived = null,
            Action<Exception> onError = null,
            Func<string, List<InsulinDose>, bool> stopCondition = null)
        {
            onTagDetected ??= _ => { };
            onDataSent ??= _ => { };
            onDataReceived ??= _ => { };
            onError ??= _ => { };
            stopCondition ??= (_, _) => false;

            _nfcAdapter?.EnableReaderMode(
                _activity,
                new TagReaderCallbackImpl(
                    tag =>
                    {
                        _runner(() =>
                        {
                            onTagDetected(tag);
                            try
                            {
                                var isoDep = IsoDep.Get(tag);
                                if (isoDep != null)
                                {
                                    isoDep.Connect();
                                    isoDep.Timeout = 1000;

                                    var dataReader = new CustomNfcDataReader(isoDep, onDataSent, onDataReceived);
                                    var controller = new NvpController(dataReader);

                                    var result = controller.DataRead(stopCondition);
                                    onDataRead(result);

                                    isoDep.Close();
                                }
                                else
                                {
                                    throw new InvalidOperationException("Incorrect tag detected");
                                }
                            }
                            catch (Exception e)
                            {
                                onError(e);
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
            private readonly Action<Tag> _callback;

            public TagReaderCallbackImpl(Action<Tag> callback)
            {
                _callback = callback;
            }

            public void OnTagDiscovered(Tag tag)
            {
                _callback(tag);
            }
        }

        private class CustomNfcDataReader : NfcDataReader
        {
            private readonly Action<byte[]> _onDataSent;
            private readonly Action<byte[]> _onDataReceived;

            public CustomNfcDataReader(IsoDep isoDep, Action<byte[]> onDataSent, Action<byte[]> onDataReceived)
                : base(isoDep)
            {
                _onDataSent = onDataSent;
                _onDataReceived = onDataReceived;
            }

            public override void OnDataSent(byte[] data)
            {
                _onDataSent(data);
            }

            public override void OnDataReceived(byte[] data)
            {
                _onDataReceived(data);
            }
        }
    }
namespace Jc.OpenNov.Data;

public abstract class PenResult
{
    private PenResult() { }

    public sealed class Success : PenResult
    {
        public PenResultData Data { get; }

        public Success(PenResultData data)
        {
            Data = data;
        }
    }

    public sealed class Failure : PenResult
    {
        public string Message { get; }

        public Failure(string message)
        {
            Message = message;
        }
    }
}
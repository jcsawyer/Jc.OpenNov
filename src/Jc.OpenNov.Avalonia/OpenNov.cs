namespace Jc.OpenNov.Avalonia;

public static class OpenNov
{
    private static readonly Lazy<IOpenNov> Implementation =
        new (() => new InternalOpenNov());
    
    public static IOpenNov Current => Implementation.Value;
}
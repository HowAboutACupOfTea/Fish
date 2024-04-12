namespace FishInterpreter.Lib;

internal class FishyException : Exception
{
    public FishyException(Exception innerException) : base("something smells fishy...", innerException)
    {
    }
}

namespace WebTesting.ProductTests;

public interface ILoggable
{
    Action<string>? Log { get; set; }
}
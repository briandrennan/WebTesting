namespace WebTesting.Contrib;

public interface ILoggable
{
    Action<string>? Log { get; set; }
}
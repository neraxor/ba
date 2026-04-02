namespace OrderSystem.Application.Ports;

public interface IClock
{
    DateTime UtcNow { get; }
}
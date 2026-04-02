using OrderSystem.Application.Ports;

namespace OrderSystem.Infrastructure.External;

public class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
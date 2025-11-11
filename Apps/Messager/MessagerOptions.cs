using Microsoft.Extensions.Logging;

namespace Messager;

public class MessagerOptions
{
	public bool EnableLogging { get; set; } = true;
	public LogLevel LogLevel { get; set; } = LogLevel.Error;
}


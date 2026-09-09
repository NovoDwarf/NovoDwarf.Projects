using SmartHome.Core;

namespace SmartHome.HomeAssistant.Interfaces;

/// <summary>
/// Platform-agnostic domain handler that executes HA actions for one device type.
/// Bot-specific code (screen building, keyboard markup) lives in bot-side subclasses.
///
/// Register one implementation per HA domain; bots discover them via
/// <c>IEnumerable&lt;IDomainExecutor&gt;</c> and route <see cref="BotAction"/>
/// without knowing about specific domains.
/// </summary>
public interface IDomainExecutor
{
    public bool OwnsGroup(string group);
    public bool OwnsEntity(string entityId);
    public bool OwnsAction(string verb);
    
    public Task<string?> ExecuteAsync(BotAction action, CancellationToken ct);
}

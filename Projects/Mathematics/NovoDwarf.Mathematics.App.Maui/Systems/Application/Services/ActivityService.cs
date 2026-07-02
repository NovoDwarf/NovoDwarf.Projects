using System.Text.Json;
using NovoDwarf.Mathematics.App.Systems.Application.Constants;

namespace NovoDwarf.Mathematics.App.Systems.Application.Services;

public class ActivityEntry
{
    public string Route { get; set; } = string.Empty;
    public Dictionary<string, object?> Parameters { get; set; } = new();
    public DateTime LastAccessUtc { get; set; } = DateTime.UtcNow;

    // Новое поле: список категорий/типов активности
    public HashSet<ActivityType> Types { get; set; } = new();
}

[Flags]
public enum ActivityType
{
    Recent = 1,
    Popular = 2,
    Featured = 4
}


public interface INavigable
{
    public string Route { get; }
}

public interface IStateable : INavigable
{
    public void OnLoad(IDictionary<string, object?> parameters);

    public IDictionary<string, object?> OnSave();
}

public class ActivityService
{
    private const int MaxRecents = 10;
    
    private readonly List<ActivityEntry> _items = [];

    public ActivityService()
    {
        LoadFromPreferences();
    }

    public IReadOnlyList<ActivityEntry> GetAll(ActivityType? type = null)
    {
        var query = _items.AsEnumerable();

        if (type is not null)
            query = query.Where(x => x.Types.Contains((ActivityType)type));
        return query.OrderByDescending(x => x.LastAccessUtc).ToList();
    }

    public void SaveState(string key, IDictionary<string, object?> state)
    {
        var entry = _items.FirstOrDefault(x => x.Route == key);
        if (entry != null)
        {
            entry.Parameters = new Dictionary<string, object?>(state);
            entry.LastAccessUtc = DateTime.UtcNow;
        }
        else
        {
            _items.Add(new ActivityEntry
            {
                Route = key,
                Parameters = new Dictionary<string, object?>(state),
                LastAccessUtc = DateTime.UtcNow,
                Types = new HashSet<ActivityType>()
            });
        }

        SaveToPreferences();
    }

    public IDictionary<string, object?>? GetState(string key)
    {
        return _items.FirstOrDefault(x => x.Route == key)?.Parameters;
    }
    
    public void MarkRecent(string key)
    {
        var entry = _items.FirstOrDefault(x => x.Route == key);
        if (entry == null) return;

        entry.LastAccessUtc = DateTime.UtcNow;
        entry.Types.Add(ActivityType.Recent);

        var recents = _items
            .Where(x => x.Types.Contains(ActivityType.Recent))
            .OrderByDescending(x => x.LastAccessUtc)
            .ToList();

        if (recents.Count > MaxRecents)
        {
            foreach (var toRemove in recents.Skip(MaxRecents))
                toRemove.Types.Remove(ActivityType.Recent);
        }

        UpdatePopularity(entry);
        SaveToPreferences();
    }
    
    private void UpdatePopularity(ActivityEntry entry)
    {
        entry.Parameters.TryAdd("AccessCount", 0);
        entry.Parameters["AccessCount"] = (int)entry.Parameters["AccessCount"]! + 1;

        if ((int)entry.Parameters["AccessCount"]! >= 5)
            entry.Types.Add(ActivityType.Popular);
    }

    public void MarkFeatured(string key)
    {
        var entry = _items.FirstOrDefault(x => x.Route == key);
        if (entry == null) return;

        entry.Types.Add(ActivityType.Featured);
        SaveToPreferences();
    }

    private void SaveToPreferences()
    {
        var json = JsonSerializer.Serialize(_items);
        Preferences.Set(PreferenceKeys.RecentActivities, json);
    }

    private void LoadFromPreferences()
    {
        if (!Preferences.ContainsKey(PreferenceKeys.RecentActivities)) return;

        var json = Preferences.Get(PreferenceKeys.RecentActivities, string.Empty);
        var items = JsonSerializer.Deserialize<List<ActivityEntry>>(json);

        if (items == null) return;
        _items.Clear();
        _items.AddRange(items);
    }
}

using System.Collections;
using Friflo.Engine.ECS;
using SultanDynasty.Instances;

namespace SultanDynasty.Simulation.Services;

public sealed class CharacterRegistry : IEnumerable<CharacterRecord>
{
	private readonly List<CharacterRecord> _records = [];
	private readonly Dictionary<Entity, int> _lookup = [];

	public CharacterRecord this[Entity entity] => _records[_lookup[entity]];
	
	public int Count => _records.Count;
	
	public IReadOnlyList<CharacterRecord> Records => _records;
	
	public IEnumerator<CharacterRecord> GetEnumerator() => _records.GetEnumerator();
	
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	
	public bool TryGet(Entity entity, out CharacterRecord record)
	{
		if (_lookup.TryGetValue(entity, out var index))
		{
			record = _records[index];
			return true;
		}

		record = null!;
		return false;
	}
	
	public bool Remove(Entity entity)
	{
		if (!_lookup.Remove(entity, out var index))
			return false;

		var last = _records.Count - 1;

		if (index != last)
		{
			_records[index] = _records[last];
			_lookup[_records[index].Entity] = index;
		}

		_records.RemoveAt(last);

		return true;
	}
	
	public CharacterRecord Register(CharacterRecord record)
	{
		_lookup.Add(record.Entity, _records.Count);
		_records.Add(record);

		return record;
	}
}

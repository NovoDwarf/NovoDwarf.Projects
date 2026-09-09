using SultanDynasty.Instances.Characters;

namespace SultanDynasty.Instances;

public class Relationships
{
	private readonly Dictionary<Guid, Relationship> _items = [];

	public int Count => _items.Count;

	public IReadOnlyCollection<Relationship> All => _items.Values;

	public bool Contains(Guid characterId)
	{
		return _items.ContainsKey(characterId);
	}

	public Relationship? Find(Guid characterId)
	{
		return _items.GetValueOrDefault(characterId);
	}

	public Relationship GetOrCreate(Guid characterId)
	{
		if (_items.TryGetValue(characterId, out var relationship))
			return relationship;

		relationship = new Relationship
		{
			Target = characterId
		};

		_items.Add(characterId, relationship);

		return relationship;
	}

	public bool Remove(Guid characterId)
	{
		return _items.Remove(characterId);
	}

	public void Clear()
	{
		_items.Clear();
	}
}
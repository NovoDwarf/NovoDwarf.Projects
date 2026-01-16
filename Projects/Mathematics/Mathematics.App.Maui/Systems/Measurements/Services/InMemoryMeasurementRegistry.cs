using Mathematics.App.Maui.Systems.Measurements.Domain;
using Mathematics.App.Maui.Systems.Measurements.Interfaces;
using Serilog;

namespace Mathematics.App.Maui.Systems.Measurements.Services;

public sealed class InMemoryMeasurementRegistry : IMeasurementRegistry
{
    private readonly Dictionary<string, Quantity> _quantities;
    private readonly Dictionary<string, UnitSystem> _systems;
    private readonly Dictionary<string, Unit> _units;
    private readonly Dictionary<string, Prefix> _prefixes;
    private readonly Dictionary<string, Unit> _unitsBySymbol;

    public IReadOnlyCollection<Quantity> Quantities => _quantities.Values;
    public IReadOnlyCollection<UnitSystem> Systems => _systems.Values;
    public IReadOnlyCollection<Unit> Units => _units.Values;
    public IReadOnlyCollection<Prefix> Prefixes => _prefixes.Values;

    public InMemoryMeasurementRegistry(
        IEnumerable<Quantity> quantities,
        IEnumerable<UnitSystem> systems,
        IEnumerable<Unit> baseUnits,
        IEnumerable<Prefix> prefixes)
    {
        _quantities = quantities.ToDictionary(q => q.Id);
        _systems = systems.ToDictionary(s => s.Id);
        _prefixes = prefixes.ToDictionary(p => p.Id);
        _units = [];
        _unitsBySymbol = [];

        var units = baseUnits as Unit[] ?? baseUnits.ToArray();

        ValidateQuantities();
        ValidateBaseUnits(units);
        GeneratePrefixedUnits(units);
        ValidateCanonicalUnits();
    }

    public Unit GetUnit(string unitId) =>
        _units.TryGetValue(unitId, out var unit) ? unit : throw new KeyNotFoundException($"Unit [{unitId}] not found");

    public Unit? GetUnitBySymbol(string symbol) =>
        _unitsBySymbol.GetValueOrDefault(symbol);

    public IEnumerable<Unit> GetUnitsByQuantity(string quantityId) =>
        _units.Values.Where(u => u.QuantityId == quantityId);

    public IEnumerable<Unit> GetUnitsBySystem(string systemId) =>
        _units.Values.Where(u => u.SystemId == systemId);

    public Quantity GetQuantity(string quantityId) =>
        _quantities.TryGetValue(quantityId, out var q) ? q : throw new KeyNotFoundException($"Quantity [{quantityId}] not found");

    public double Convert(double value, string fromUnitId, string toUnitId)
    {
        var from = GetUnit(fromUnitId);
        var to = GetUnit(toUnitId);

        if (from.QuantityId != to.QuantityId)
            throw new InvalidOperationException($"Units [{from.Id}] and [{to.Id}] belong to different quantities");

        var baseValue = (value + from.Offset) * from.Factor;

        return baseValue / to.Factor - to.Offset;
    }

    private void ValidateQuantities()
    {
        foreach (var q in _quantities.Values)
        {
            if (q.Dimension == null)
                throw new InvalidOperationException($"Quantity [{q.Id}] has no Dimension");

            if (string.IsNullOrWhiteSpace(q.CanonicalUnitId))
                throw new InvalidOperationException($"Quantity [{q.Id}] has no CanonicalUnitId");
        }
    }

    private void ValidateBaseUnits(IEnumerable<Unit> units)
    {
        foreach (var unit in units)
        {
            if (!_quantities.ContainsKey(unit.QuantityId))
                throw new InvalidOperationException($"Unit [{unit.Id}] references unknown Quantity [{unit.QuantityId}]");

            if (!_systems.ContainsKey(unit.SystemId))
                throw new InvalidOperationException($"Unit [{unit.Id}] references unknown System [{unit.SystemId}]");

            AddUnit(unit);
        }
    }

    private void ValidateCanonicalUnits()
    {
        foreach (var q in _quantities.Values)
        {
            if (!_units.TryGetValue(q.CanonicalUnitId, out var unit))
                throw new InvalidOperationException($"Canonical unit [{q.CanonicalUnitId}] not found for Quantity [{q.Id}]");

            if (unit.Factor != 1 || unit.Offset != 0)
                throw new InvalidOperationException($"Canonical unit [{unit.Id}] must have Factor = 1 and Offset = 0");

            if (!q.Dimension.Equals(_quantities[unit.QuantityId].Dimension))
                throw new InvalidOperationException($"Canonical unit [{unit.Id}] has wrong dimension");
        }
    }

    private void GeneratePrefixedUnits(IEnumerable<Unit> baseUnits)
    {
        foreach (var unit in baseUnits)
        {
            if (!unit.AllowPrefixes)
                continue;

            foreach (var prefix in _prefixes.Values)
            {
                if (string.IsNullOrWhiteSpace(prefix.Id))
                    continue;

                var prefixed = new Unit
                {
                    Id = prefix.Id + unit.Id,
                    DisplayName = $"{prefix.DisplayName}-{unit.DisplayName}",

                    QuantityId = unit.QuantityId,
                    SystemId = unit.SystemId,

                    Factor = unit.Factor * Math.Pow(10, prefix.Power),
                    Offset = unit.Offset,

                    AllowPrefixes = false,
                    Prefix = prefix
                };

                AddUnit(prefixed);
            }
        }
    }

    private void AddUnit(Unit unit)
    {
        if (!_units.TryAdd(unit.Id, unit))
        {
            Log.Warning("Unit [{Id}] was already added", unit.Id);
            return;
        }

        _unitsBySymbol.TryAdd(unit.Id, unit);
    }
}
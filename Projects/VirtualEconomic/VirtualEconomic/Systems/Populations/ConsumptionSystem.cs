using Grekov.Definitions.Interfaces;
using Komissar.Core.Enums;
using Komissar.Runtime;
using Komissar.Systems.Abstractions;
using VirtualEconomic.Components.Persons;
using VirtualEconomic.TTags.Identity;

namespace VirtualEconomic.Systems.Populations;

public sealed class ConsumptionSystem : SimulationSystem<EconomyState>
{
	private readonly ArchetypeQuery<Demographics, Needs, Money> _people;

	public ConsumptionSystem(EntityStore world, IDefCatalog defs)
	{
		_people = world.Query<Demographics, Needs, Money>().AllTags(Tags.Get<PersonTag>());
	}

	public override SimExecMode ExecutionMode => SimExecMode.Parallel;
	
	public override void Execute(SimContext<EconomyState> context)
	{
		var days = context.Delta.TotalDays;

		_people.ForEachEntity((ref demographics, ref needs, ref money, entity) => ConsumePerson(ref demographics, ref needs, ref money, days));
	}
	
	private void ConsumePerson(ref Demographics demographics, ref Needs needs, ref Money money, double days)
	{
		
	}
}
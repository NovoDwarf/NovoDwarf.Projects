using SultanDynasty.Simulation.Generators.Interfaces;
using SultanDynasty.Enums;
using SultanDynasty.Instances.Characters;

namespace SultanDynasty.Simulation.Generators.Steps;

public sealed class GeneticsGenerationStep : ICharacterGenerationStep
{
	public int Order => 30;

	public void Execute(CharacterBuilderContext context)
	{
		context.Profile.Genetics = new Genetics
		{
			HeightPotential = context.Random.NextFloat(155, 180),
			BodyFrame = context.Random.PickEnum<BodyFrame>(),
			BodyType = context.Random.PickEnum<BodyType>(),
			EyeColor = context.Random.PickEnum<EyeColor>(),
			HairColor = context.Random.PickEnum<HairColor>(),
			HairType = context.Random.PickEnum<HairType>(),
			SkinTone = context.Random.PickEnum<SkinTone>(),
			FaceShape = context.Random.PickEnum<FaceShape>(),
			NoseShape = context.Random.PickEnum<NoseShape>(),
			LipShape = context.Random.PickEnum<LipShape>(),
			VoiceType = context.Random.PickEnum<VoiceType>(),
			BreastSizePotential = context.Random.PickEnum<PartSizeType>(),
			GenitalSizePotential = context.Random.PickEnum<GenitalSize>(),
			WaistPotential = context.Random.PickEnum<PartSizeType>(),
			HipPotential = context.Random.PickEnum<PartSizeType>(),
			MuscleGainRate = context.Random.NextFloat(0.7f, 1.3f),
			FatGainRate = context.Random.NextFloat(0.7f, 1.3f),
			FatStorageLowerBody = context.Random.NextFloat(0.2f, 0.8f),
			FatStorageUpperBody = context.Random.NextFloat(0.2f, 0.8f),
			Immunity = context.Random.NextFloat(0.4f, 1f),
			Libido = context.Random.NextFloat(0.2f, 1f),
			Fertility = context.Random.NextFloat(0.2f, 1f),
			Temperament = context.Random.NextFloat(0.2f, 1f),
			AgingRate = context.Random.NextFloat(0.8f, 1.2f)
		};
	}
}

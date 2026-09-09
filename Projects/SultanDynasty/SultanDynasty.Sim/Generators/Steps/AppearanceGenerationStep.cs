using SultanDynasty.Instances.Characters;
using SultanDynasty.Simulation.Generators.Interfaces;

namespace SultanDynasty.Simulation.Generators.Steps;

public sealed class AppearanceGenerationStep : ICharacterGenerationStep
{
	public int Order => 40;

	public void Execute(CharacterBuilderContext context)
	{
		var genetics = context.Profile.Genetics;

		context.Profile.Appearance = new Appearance
		{
			CurrentHeight = genetics.HeightPotential,
			CurrentWeight = genetics.HeightPotential - context.Random.NextFloat(100f, 112f),
			FatPercent = context.Random.NextFloat(18f, 32f),
			MuscleMass = context.Random.NextFloat(0.2f, 0.6f),
			BodyFrame = genetics.BodyFrame,
			BodyType = genetics.BodyType,
			ChestCircumference = context.Random.NextFloat(78f, 105f),
			WaistCircumference = context.Random.NextFloat(58f, 86f),
			HipCircumference = context.Random.NextFloat(82f, 112f),
			NeckCircumference = context.Random.NextFloat(28f, 38f),
			ArmCircumference = context.Random.NextFloat(22f, 34f),
			ThighCircumference = context.Random.NextFloat(44f, 64f),
			FootSize = context.Random.NextFloat(35f, 42f),
			HandSize = context.Random.NextFloat(15f, 20f),
			HairLength = context.Random.NextFloat(10f, 80f),
			HairType = genetics.HairType,
			HairColor = genetics.HairColor,
			EyeColor = genetics.EyeColor,
			SkinTone = genetics.SkinTone,
			Tan = context.Random.NextFloat(0f, 0.4f),
			Scars = context.Random.NextFloat(0f, 0.15f),
			Tattoos = context.Random.NextFloat(0f, 0.05f),
			Birthmarks = context.Random.NextFloat(0f, 0.2f),
			FaceShape = genetics.FaceShape,
			NoseShape = genetics.NoseShape,
			LipShape = genetics.LipShape,
			FacialHair = 0f,
			BreastSize = genetics.BreastSizePotential,
			GenitalSize = genetics.GenitalSizePotential,
			VoiceType = genetics.VoiceType,
			VoiceSoftness = context.Random.NextFloat(0.4f, 1f),
			ScentIntensity = context.Random.NextFloat(0.2f, 0.8f),
			Grooming = context.Random.NextFloat(0.45f, 0.95f),
			Cleanliness = context.Random.NextFloat(0.55f, 1f)
		};
	}
}

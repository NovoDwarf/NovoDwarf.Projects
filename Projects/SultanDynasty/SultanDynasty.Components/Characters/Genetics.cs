using Friflo.Engine.ECS;
using SultanDynasty.Enums;

namespace SultanDynasty.Instances.Characters;

public struct Genetics : IComponent
{
	public float HeightPotential;
	public BodyFrame BodyFrame;
	public BodyType BodyType;

	public EyeColor EyeColor;
	public HairColor HairColor;
	public HairType HairType;
	public SkinTone SkinTone;

	public FaceShape FaceShape;
	public NoseShape NoseShape;
	public LipShape LipShape;
	public VoiceType VoiceType;

	public PartSizeType BreastSizePotential;
	public GenitalSize GenitalSizePotential;
	public PartSizeType WaistPotential;
	public PartSizeType HipPotential;

	public float MuscleGainRate;
	public float FatGainRate;
	public float FatStorageLowerBody;
	public float FatStorageUpperBody;

	public float Immunity;
	public float Libido;
	public float Fertility;
	public float Temperament;
	public float AgingRate;
}

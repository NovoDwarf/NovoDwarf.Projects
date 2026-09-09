using Friflo.Engine.ECS;
using SultanDynasty.Enums;

namespace SultanDynasty.Instances.Characters;

public struct Appearance : IComponent
{
	public float CurrentHeight;
	public float CurrentWeight;

	public float FatPercent;
	public float MuscleMass;
	public BodyFrame BodyFrame;
	public BodyType BodyType;

	public float ChestCircumference;
	public float WaistCircumference;
	public float HipCircumference;
	public float NeckCircumference;
	public float ArmCircumference;
	public float ThighCircumference;

	public float FootSize;
	public float HandSize;

	public float HairLength;
	public HairType HairType;
	public HairColor HairColor;
	public EyeColor EyeColor;

	public SkinTone SkinTone;
	public float Tan;
	public float Scars;
	public float Tattoos;
	public float Birthmarks;

	public FaceShape FaceShape;
	public NoseShape NoseShape;
	public LipShape LipShape;
	public float FacialHair;

	public BreastShape BreastShape;
	public PartSizeType BreastSize;
	public GenitalSize GenitalSize;

	public VoiceType VoiceType;
	public float VoiceSoftness;

	public float ScentIntensity;
	public float Grooming;
	public float Cleanliness;
}

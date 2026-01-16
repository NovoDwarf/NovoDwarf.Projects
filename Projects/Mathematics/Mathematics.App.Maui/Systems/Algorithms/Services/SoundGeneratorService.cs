namespace Mathematics.App.Maui.Services.Views;

public enum WaveType
{
	Sine,
	Square,
	Triangle,
	Sawtooth
}

public class SoundGeneratorService
{
	private const int SampleRate = 44100;
	private const short BitsPerSample = 16;

	public static byte[] GenerateWave(double frequency = 440, double durationSeconds = 1, WaveType waveType = WaveType.Sine, double amplitude = 0.5)
	{
		var sampleCount = (int)(SampleRate * durationSeconds);
		var buffer = new byte[sampleCount * 2];

		for (var i = 0; i < sampleCount; i++)
		{
			var t = (double)i / SampleRate;
			var sampleValue = waveType switch
			{
				WaveType.Sine => Math.Sin(2 * Math.PI * frequency * t),
				WaveType.Square => Math.Sign(Math.Sin(2 * Math.PI * frequency * t)),
				WaveType.Triangle => (2 * Math.Abs(2 * ((t * frequency) - Math.Floor((t * frequency) + 0.5)))) - 1,
				WaveType.Sawtooth => 2 * ((t * frequency) - Math.Floor((t * frequency) + 0.5)),
				_ => 0
			};

			var intSample = (short)(sampleValue * amplitude * short.MaxValue);

			buffer[i * 2] = (byte)(intSample & 0xFF);
			buffer[(i * 2) + 1] = (byte)((intSample >> 8) & 0xFF);
		}

		return buffer;
	}

	public static byte[] GenerateSin(double frequency = 440, double durationSeconds = 1, double amplitude = 0.5)
	{
		return GenerateWave(frequency, durationSeconds, WaveType.Sine, amplitude);
	}
}

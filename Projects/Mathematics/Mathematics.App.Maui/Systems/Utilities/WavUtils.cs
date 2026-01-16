namespace Mathematics.App.Maui.Systems.Utilities;

public static class WavUtils
{
	public static Stream ConvertPcmToWav(byte[] pcmData, int sampleRate = 44100, short channels = 1, int bitsPerSample = 16)
	{
		var memoryStream = new MemoryStream();

		// Размер данных без заголовка (44 байта)
		var dataSize = pcmData.Length;
		var fileSize = dataSize + 36; // 36 + 8 = 44 байта на заголовок

		// Записываем RIFF заголовок
		StreamUtils.WriteString(memoryStream, "RIFF");
		StreamUtils.WriteInt(memoryStream, fileSize);
		StreamUtils.WriteString(memoryStream, "WAVE");

		// Форматный чанк
		StreamUtils.WriteString(memoryStream, "fmt ");
		StreamUtils.WriteInt(memoryStream, 16); // Размер fmt чанка
		StreamUtils.WriteShort(memoryStream, 1); // Аудио формат (1 = PCM)
		StreamUtils.WriteShort(memoryStream, channels); // Количество каналов
		StreamUtils.WriteInt(memoryStream, sampleRate); // Частота дискретизации
		StreamUtils.WriteInt(memoryStream, sampleRate * channels * bitsPerSample / 8); // Байтрейт
		StreamUtils.WriteShort(memoryStream, (short)(channels * bitsPerSample / 8)); // Блок выравнивания
		StreamUtils.WriteShort(memoryStream, (short)bitsPerSample); // Бит на сэмпл

		StreamUtils.WriteString(memoryStream, "data");
		StreamUtils.WriteInt(memoryStream, dataSize);
		memoryStream.Write(pcmData, 0, pcmData.Length);

		memoryStream.Position = 0;
		return memoryStream;
	}


}
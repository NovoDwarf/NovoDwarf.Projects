namespace NovoDwarf.Mathematics.App.Systems.Application.Constants;

public static class Alerts
{
	public static void SensorNotSupported(string key)
		=> Shell.Current.DisplayAlertAsync("Уведомление", "Барометр не поддерживается данным устройством!", "Ок!");
}
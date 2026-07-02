namespace NovoDwarf.Mathematics.App.Systems.Utilities;

public static class PermissionUtils
{
	public static async Task<bool> EnsurePermissionsAsync<TPermission>() where TPermission : Permissions.BasePermission, new()
	{
		var status = await Permissions.CheckStatusAsync<TPermission>();

		if (status != PermissionStatus.Granted)
			status = await Permissions.RequestAsync<TPermission>();

		return status == PermissionStatus.Granted;
	}
}
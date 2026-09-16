namespace NovoDwarf.FS.Interfaces;

public interface IPathService
{
	public string GetExtension(string path);
	public string GetDirectoryName(string path);
	
	public string Combine(params string[] paths);
}

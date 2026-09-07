namespace NovoDwarf.FS.Interfaces;

public interface IPathService
{
	string GetExtension(string path);
	string Combine(params string[] paths);
}

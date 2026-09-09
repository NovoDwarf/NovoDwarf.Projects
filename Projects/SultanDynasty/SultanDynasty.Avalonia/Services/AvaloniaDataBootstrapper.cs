using Grekov.Packaging.Interfaces;

namespace SultanDynasty.Avalonia.Services;

public sealed class AvaloniaDataBootstrapper
{
	private readonly IPackageRuntime _packages;

	public AvaloniaDataBootstrapper(IPackageRuntime packages)
	{
		_packages = packages;
	}

	public void Load()
	{
		_packages.LoadAll();
	}
}

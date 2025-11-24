using System.Drawing;

namespace Mathematics.Server.Controllers.Noises;

public interface IColorMapper
{
	Color Map(float value, ColorSchemeType schemeType);
}
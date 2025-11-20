namespace Modeling.Core.Models.Abstracts.Commons.Options;

public class RouteOptions : StorageOptions
{
	/// <summary>
	///     Gets or sets the maximum number of allowed input links.
	///     Values less than or equal to 0 indicate no limit.
	///     Default value is -1 (unlimited).
	/// </summary>
	public int InputMaxCount { get; set; } = -1;

	/// <summary>
	///     Gets or sets the maximum number of allowed output links.
	///     Values less than or equal to 0 indicate no limit.
	///     Default value is -1 (unlimited).
	/// </summary>
	public int OutputMaxCount { get; set; } = -1;
}
namespace Modeling.Core.Models.Base;

public class Request
{
	private Request()
	{
	}

	public Guid Id { get; set; }
	public Guid SourceId { get; set; }

	public static Request Create(Guid sourceId)
	{
		return new Request
		{
			Id = Guid.NewGuid(),
			SourceId = sourceId
		};
	}
}
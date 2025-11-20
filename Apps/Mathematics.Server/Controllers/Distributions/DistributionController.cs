using Mathematics.Distributions.Base;
using Mathematics.Distributions.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Mathematics.Server.Controllers.Distributions;

[ApiController]
[Route("/distributions")]
public class DistributionController<T> : ControllerBase where T : Distribution
{

	private readonly ILogger<DistributionController<T>> _logger;
	
	public DistributionController(ILogger<DistributionController<T>> logger)
	{
		_logger = logger;
	}
	
	protected T _distribution;
	
	[HttpGet]
	public IActionResult Get()
	{
		return Ok(_distribution.Calculate());
	}
	
	[HttpGet("expected")]
	public IActionResult GetExpectedValue()
	{
		return Ok(_distribution.GetExpectedValue());
	}
	
	[HttpGet("variance")]
	public IActionResult GetVariance()
	{
		return Ok(_distribution.GetVariance());
	}
	
	[HttpGet("max")]
	public IActionResult GetMaxValue()
	{
		return Ok(_distribution.GetMaxValue());
	}
	
	[HttpGet("min")]
	public IActionResult GetMinValue()
	{
		return Ok(_distribution.GetMinValue());
	}
}
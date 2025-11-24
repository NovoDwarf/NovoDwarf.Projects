using Mathematics.Core.Extensions.Noises;
using Mathematics.Graphics.Noises.Gradient;
using Mathematics.Server.Base.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Mathematics.Server.Controllers.Noises;

/// <summary>
/// Controller for generating Perlin noise images.
/// Perlin noise is a type of gradient noise used in computer graphics for procedural texture generation.
/// </summary>
[ApiController]
[Route("noises/perlin")]
public class PerlinNoiseController : ControllerBase
{
    private readonly ILogger<PerlinNoiseController> _logger;
    private readonly IImageEncoder _encoder;
    private readonly IColorMapper _colors;

    public PerlinNoiseController(
	    IImageEncoder encoder,
	    IColorMapper colors, 
	    ILogger<PerlinNoiseController> logger)
    {
	    _encoder = encoder;
	    _colors = colors;
	    _logger = logger;
    }
    
    /// <summary>
    /// Generates a Perlin noise image
    /// </summary>
    /// <param name="request">Request for the Perlin noise generation</param>
    /// <param name="options">Options for the Perlin noise generation</param>
    /// <returns>Returns a PNG image of the Perlin noise</returns>
    [HttpPost("")]
    [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
    public IActionResult Generate(
	    [FromBody] PerlinNoiseRequest? request = null)
    {
	    request ??= new PerlinNoiseRequest();
	
	    var noise = new PerlinNoise(request.Options);
	    var pixels = noise.Make(request.Size);

	    var bitmap = pixels.ToBitmap(_colors, request.ColorScheme);

	    var bytes = _encoder.Encode(bitmap, request.Format);
	    var mime = _encoder.GetMimeType(request.Format);

	    return File(bytes, mime);
    }
}
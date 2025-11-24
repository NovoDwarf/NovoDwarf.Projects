using System.ComponentModel.DataAnnotations;
using System.Drawing.Imaging;
using System.Numerics;
using Mathematics.Core.Extensions.Noises;
using Mathematics.Graphics.Noises.Gradient;
using Microsoft.AspNetCore.Mvc;
using NovoUtils.Web;
using Scalar.AspNetCore;

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

    public PerlinNoiseController(ILogger<PerlinNoiseController> logger) => _logger = logger;
    
    /// <summary>
    /// Generates a Perlin noise image as PNG.
    /// </summary>
    /// <param name="width">Width of the generated image in pixels (range: 1 - 4096)</param>
    /// <param name="height">Height of the generated image in pixels (range: 1 - 4096)</param>
    /// <returns>Returns a PNG image of the Perlin noise</returns>
    [HttpGet("")]
    [Stability(Stability.Stable)]
    [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK, contentType: "image/png")]
    public IActionResult Generate(
        [FromQuery, Range(1, 4096)] int width = 256,
        [FromQuery, Range(1, 4096)] int height = 256)
    {
            var noise = new PerlinNoise();
            var pixels = noise.Make(new Vector2(width, height));
            var bitmap = noise.ToHeatmapBitmap(pixels);
            
            using var stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Png);

            var bytes = stream.ToArray();
            
            return File(bytes, "image/png");
    }
    
    /// <summary>
    /// Generates a Perlin noise image as PNG.
    /// </summary>
    /// <param name="size">Size of the generated image in pixels (range: 1 - 4096)</param>
    /// <returns>Returns a PNG image of the Perlin noise</returns>
    [HttpGet("")]
    [Stability(Stability.Stable)]
    [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK, contentType: "image/png")]
    public IActionResult Generate(Vector2 size)
    {
	    var noise = new PerlinNoise();
	    var pixels = noise.Make(size);
	    var bitmap = noise.ToHeatmapBitmap(pixels);
            
	    using var stream = new MemoryStream();
	    bitmap.Save(stream, ImageFormat.Png);

	    var bytes = stream.ToArray();
            
	    return File(bytes, "image/png");
    }
}
using DocumentToPdf.Core;
using Microsoft.AspNetCore.Mvc;

namespace DocumentToPdf.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConvertController : ControllerBase
{
    private readonly IDocumentToPdfConverter _converter;

    public ConvertController(IDocumentToPdfConverter converter)
    {
        _converter = converter;
    }


    /// <summary>
    /// ToPdf
    /// </summary>
    /// <param name="file"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPost("to-pdf")]
    [RequestSizeLimit(50_000_000)] // 50 MB
    public async Task<IActionResult> ToPdf([FromForm] IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest("File is required.");

        await using var input = file.OpenReadStream();
        byte[] pdfBytes;

        try
        {
            pdfBytes = await _converter.ConvertToPdfAsync(input, file.FileName, ct);
        }
        catch (NotSupportedException ex)
        {
            return BadRequest(ex.Message);
        }

        var outputName = Path.GetFileNameWithoutExtension(file.FileName) + ".pdf";
        return File(pdfBytes, "application/pdf", outputName);
    }

    // Simple health check
    [HttpGet("health")]
    public IActionResult Health() => Ok(new { status = "ok" });
}

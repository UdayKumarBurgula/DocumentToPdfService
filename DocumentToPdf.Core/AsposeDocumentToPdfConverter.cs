using System.Globalization;

namespace DocumentToPdf.Core;

/// <summary>
/// AsposeDocumentToPdfConverter
/// </summary>
public sealed class AsposeDocumentToPdfConverter : IDocumentToPdfConverter
{

    /// <summary>
    /// ConvertToPdfAsync
    /// </summary>
    /// <param name="input"></param>
    /// <param name="fileName"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    public async Task<byte[]> ConvertToPdfAsync(Stream input, string fileName, CancellationToken ct = default)
    {
        if (input is null) throw new ArgumentNullException(nameof(input));
        if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("File name is required.", nameof(fileName));

        var ext = Path.GetExtension(fileName)?.ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(ext))
            throw new NotSupportedException("File extension not found.");

        // Aspose APIs are mostly synchronous; we keep async boundary for WebAPI.
        // Ensure we start reading from beginning:
        if (input.CanSeek) input.Position = 0;

        return ext switch
        {
            ".doc" or ".docx" or ".rtf" or ".odt" => ConvertWordToPdf(input),
            ".xls" or ".xlsx" or ".csv" => ConvertExcelToPdf(input),
            ".ppt" or ".pptx" => ConvertSlidesToPdf(input),
            _ => throw new NotSupportedException($"Unsupported file type: {ext}")
        };
    }

    /// <summary>
    /// ConvertWordToPdf
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private static byte[] ConvertWordToPdf(Stream input)
    {
        // Aspose.Words
        var doc = new Aspose.Words.Document(input);

        using var ms = new MemoryStream();
        doc.Save(ms, Aspose.Words.SaveFormat.Pdf);
        return ms.ToArray();
    }

    /// <summary>
    /// ConvertExcelToPdf
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private static byte[] ConvertExcelToPdf(Stream input)
    {
        // Aspose.Cells
        var workbook = new Aspose.Cells.Workbook(input);

        using var ms = new MemoryStream();
        workbook.Save(ms, Aspose.Cells.SaveFormat.Pdf);
        return ms.ToArray();
    }

    /// <summary>
    /// ConvertSlidesToPdf
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private static byte[] ConvertSlidesToPdf(Stream input)
    {
        // Aspose.Slides
        var pres = new Aspose.Slides.Presentation(input);

        using var ms = new MemoryStream();
        pres.Save(ms, Aspose.Slides.Export.SaveFormat.Pdf);
        return ms.ToArray();
    }
}

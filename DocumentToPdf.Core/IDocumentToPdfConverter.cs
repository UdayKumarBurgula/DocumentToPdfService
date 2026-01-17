namespace DocumentToPdf.Core;

public interface IDocumentToPdfConverter
{
    /// <summary>
    /// Converts an input document to PDF.
    /// </summary>
    /// <param name="input">Input file stream (position will be read from current).</param>
    /// <param name="fileName">Original file name (used to detect extension).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>PDF bytes.</returns>
    Task<byte[]> ConvertToPdfAsync(Stream input, string fileName, CancellationToken ct = default);
}

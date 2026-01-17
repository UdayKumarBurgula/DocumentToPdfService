namespace DocumentToPdf.Core;

public static class AsposeLicenseLoader
{
    /// <summary>
    /// Load Aspose licenses from a single license file stream.
    /// If you have multiple Aspose products, you may need to set license per product.
    /// </summary>
    public static void TryLoadFromPath(string? licensePath)
    {
        if (string.IsNullOrWhiteSpace(licensePath)) return;
        if (!File.Exists(licensePath)) return;

        using var fs = File.OpenRead(licensePath);

        // If you use multiple products, set each license if required:
        // new Aspose.Words.License().SetLicense(fs);  // can't reuse same stream without resetting
        // So we reopen per product.

        using (var w = File.OpenRead(licensePath))
            new Aspose.Words.License().SetLicense(w);

        //using (var c = File.OpenRead(licensePath))
        //    new Aspose.Cells.License().SetLicense(c);

        //using (var s = File.OpenRead(licensePath))
        //    new Aspose.Slides.License().SetLicense(s);
    }
}

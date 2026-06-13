using System;
using OpenCvSharp;

public class WatermarkDetector : IDisposable
{
    private readonly Mat _template;
    private readonly double _threshold;

    /// <summary>
    /// Initializes the detector with a known watermark template image.
    /// </summary>
    /// <param name="templatePath">Path to the crisp, isolated watermark image.</param>
    /// <param name="threshold">Match confidence threshold (0.0 to 1.0). 0.8 is usually ideal.</param>
    public WatermarkDetector(string templatePath, double threshold = 0.80)
    {
        _template = Cv2.ImRead(templatePath, ImreadModes.Grayscale);
        if (_template.Empty())
        {
            throw new ArgumentException("Failed to load watermark template image.");
        }
        _threshold = threshold;
    }

    /// <summary>
    /// Detects if a watermark exists within a specific region of an image.
    /// </summary>
    public bool IsWatermarked(string imagePath, out Rect matchedRegion)
    {
        matchedRegion = default;

        using var source = Cv2.ImRead(imagePath, ImreadModes.Grayscale);
        if (source.Empty()) return false;

        int roiWidth = source.Width / 2;
        int roiHeight = source.Height / 2;
        int roiX = source.Width - roiWidth;
        int roiY = source.Height - roiHeight;

        var roiRect = new Rect(roiX, roiY, roiWidth, roiHeight);

        if (roiWidth < _template.Width || roiHeight < _template.Height) return false;

        using var roiSource = new Mat(source, roiRect);

        int resultWidth = source.Cols - _template.Cols + 1;
        int resultHeight = source.Rows - _template.Rows + 1;

        if (resultWidth <= 0 || resultHeight <= 0) return false;

        using var result = new Mat(resultHeight, resultWidth, MatType.CV_32FC1);

        Cv2.MatchTemplate(source, _template, result, TemplateMatchModes.CCoeffNormed);

        Cv2.MinMaxLoc(result, out _, out double maxVal, out _, out Point maxLoc);

        if (maxVal >= _threshold)
        {
            matchedRegion = new Rect(maxLoc.X, maxLoc.Y, _template.Width, _template.Height);
            return true;
        }

        return false;
    }

    public void Dispose()
    {
        _template?.Dispose();
    }
}

using System;
using System.IO;
using OpenCvSharp;

var templatePath = "./Assets/Watermark.png";
var replacementPath = "./Assets/Filler.png";
var outputDir = "./Assets/Output/";

Directory.CreateDirectory(outputDir);

using var detector = new WatermarkDetector(templatePath, 0.85);

using var replacementWatermark = Cv2.ImRead(replacementPath, ImreadModes.Color);
if (replacementWatermark.Empty())
{
    Console.WriteLine("Failed to load the replacement watermark image.");
    return;
}

var imageDir = "./Assets/Images/";
var files = Directory.GetFiles(imageDir);
Console.WriteLine($"Starting scan and replace of {files.Length} files...\n");

foreach(var file in files)
{
    try
    {
        if (detector.IsWatermarked(file, out var region))
        {
            Console.WriteLine($"[MATCH] Replacing watermark in {Path.GetFileName(file)} at X:{region.X} Y:{region.Y}");

            using var colorTarget = Cv2.ImRead(file, ImreadModes.Color);

            using var resizedReplacement = new Mat();
            Cv2.Resize(replacementWatermark, resizedReplacement, new Size(region.Width, region.Height));

            using var targetRoi = new Mat(colorTarget, region);

            resizedReplacement.CopyTo(targetRoi);

            var outputPath = Path.Combine(outputDir, Path.GetFileName(file));
            Cv2.ImWrite(outputPath, colorTarget);
        }
        else
        {
            Console.WriteLine($"[MISS]  No watermark detected in {Path.GetFileName(file)}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERROR] File: {Path.GetFileName(file)} - {ex.Message}");
    }
}

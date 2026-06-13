# WatermarkSwapper

A high-performance, low-allocation .NET utility designed to automatically detect and replace specific simple watermarks across millions of images. Utilizing native OpenCV bindings via OpenCvSharp4, this tool processes images at bare-metal speeds with a minimal memory footprint.

## Features

- **High-Throughput Template Matching:** Rapidly scans images using Normalized Cross-Correlation (`CCoeffNormed`) in grayscale to drastically reduce CPU arithmetic cycles.
- **In-Memory Pixel Substitution:** Leverages OpenCV Region of Interest (ROI) pointers to overwrite target pixels directly without full-image degradation or heavy I/O overhead.

## Prerequisites

- **SDK:** .NET 8.0 / .NET 9.0 / .NET 10.0
- **Native Runtime:** Requires the OpenCV native library wrapper for your deployment OS.

## Installation

Install the core library along with the optimized headless runtime for your specific environment:

```bash
# Core bindings
dotnet add package OpenCvSharp4

# Headless Linux (Docker / Server / WSL)
dotnet add package OpenCvSharp4.official.runtime.linux-x64.slim

# Windows Environment
dotnet add package OpenCvSharp4.runtime.win

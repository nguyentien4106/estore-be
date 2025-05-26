using MediaInfo.DotNetWrapper.Enumerations;

namespace EStore.Application.Models.Files;

public class VideoInfo 
{
    public string Codec { get; private set; }
    public int Width { get; private set; }
    public int Heigth { get; private set; }
    public double FrameRate { get; private set; }
    public string FrameRateMode { get; private set; }
    public string ScanType { get; private set; }
    public TimeSpan Duration { get; private set; }
    public int Bitrate { get; private set; }
    public string AspectRatioMode { get; private set; }
    public double AspectRatio { get; private set; }

    public VideoInfo(MediaInfo.DotNetWrapper.MediaInfo mi)
    {
        Codec=mi.Get(StreamKind.Video, 0, "Format");
        Duration = TimeSpan.FromSeconds(int.Parse(mi.Get(StreamKind.Video, 0, "Duration")));
        Heigth = int.Parse(mi.Get(StreamKind.Video, 0, "Height"));
        Width = int.Parse(mi.Get(StreamKind.Video, 0, "Width"));
        Bitrate = int.Parse(mi.Get(StreamKind.Video, 0, "BitRate"));
        AspectRatioMode = mi.Get(StreamKind.Video, 0, "AspectRatio/String"); //as formatted string
        AspectRatio =double.Parse(mi.Get(StreamKind.Video, 0, "AspectRatio"));
        FrameRate = double.Parse(mi.Get(StreamKind.Video, 0, "FrameRate"));
        FrameRateMode = mi.Get(StreamKind.Video, 0, "FrameRate_Mode");
        ScanType = mi.Get(StreamKind.Video, 0, "ScanType");
    }
}

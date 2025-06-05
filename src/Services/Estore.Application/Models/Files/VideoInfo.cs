using MediaInfo.DotNetWrapper;
using MediaInfo.DotNetWrapper.Enumerations;

namespace EStore.Application.Models.Files;

public class VideoInfo 
{
    public string Codec { get; private set; }
    public int Width { get; private set; }
    public int Heigth { get; private set; }
    public double FrameRate { get; private set; }
    public string ScanType { get; private set; }
    public TimeSpan Duration { get; private set; }
    public int Bitrate { get; private set; }

    public VideoInfo(MediaInfoWrapper mi)
    {
        Codec = mi.VideoCodec;
        Duration = TimeSpan.FromSeconds(mi.Duration);
        Heigth = mi.Height;
        Width = mi.Width;
        FrameRate = mi.Framerate;
        ScanType = mi.ScanType;
        Bitrate = mi.VideoRate;
    }

}

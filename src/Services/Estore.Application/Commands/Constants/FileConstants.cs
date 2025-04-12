namespace EStore.Application.Constants;

public static class FileSizeLimits
{
    public static int OneMB = 1024 * 1024;
    
    public static int BandwidthLimit = 500 * 1024;
    public static int FiveMBs = 5 * OneMB;
    public static long OneGB = 1024 * OneMB;

    public static long FreeTierLimit = 20 * OneMB;

    public static long ProTierLimit = 2 * OneGB;

    public static long PlusTierLimit = 5 * OneGB;
}

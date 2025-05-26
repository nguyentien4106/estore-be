namespace EStore.Application.Helpers;

public static class R2Helper
{
    public static string GetR2FileKey(string userName, string fileName) => $"{userName}/{fileName}";
} 
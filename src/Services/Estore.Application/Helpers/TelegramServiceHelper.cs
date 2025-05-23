namespace EStore.Application.Helpers;

public static class TelegramServiceHelper
{

    public static string GetCaption(string fileName){
        return $"File: {fileName}";
    }
}

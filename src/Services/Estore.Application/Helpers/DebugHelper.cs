using System.Text.Json;

namespace Estore.Application.Helpers;

public class DebugHelper
{
    public static void Log(string name, object obj)
    {
        string json = JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
        Console.WriteLine(name);
        Console.WriteLine(json);
    }
}
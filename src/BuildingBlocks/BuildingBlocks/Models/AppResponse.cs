namespace BuildingBlocks.Models;

public class AppResponse<T>
{
    public T? Data { get; set; }
    
    public string Message { get; set; }
    
    public bool Succeed{ get; set; }
    
    private AppResponse<T> SetSuccess(T data, string? message)
    {
        Data = data;
        Message = message;
        Succeed = true;
        
        return this;
    }

    private AppResponse<T> SetError(string message, T data)
    {
        Data = data;
        Message = message;
        Succeed = false;

        return this;
    }

    public static AppResponse<T> Success(T data, string? message = null)
    {
        return new AppResponse<T>().SetSuccess(data, message);
    }

    public static AppResponse<T> Error(string message, T data = default)
    {
        return new AppResponse<T>().SetError(message, data);
    }
}
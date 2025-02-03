namespace Exptour.Common.Shared;

public class BaseResponse<T>
{
    public T Payload { get; set; }
    public string Message { get; set; }
    public string State { get; set; }

    public BaseResponse()
    {
    }
    public BaseResponse(string message)
    {
        Message = message;
    }
    public BaseResponse(string message, T payload)
    {
        Message = message;
        Payload = payload;
    }
    public BaseResponse(string message, string state)
    {
        Message = message;
        State = state;
    }
    public BaseResponse(string message, string state, T payload)
    {
        Message = message;
        State = state;
        Payload = payload;
    }
}

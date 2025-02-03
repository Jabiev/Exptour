using Exptour.Common.Shared;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections;
using System.Net;
using System.Text.Json.Serialization;
using JsonIgnoreAttribute = Newtonsoft.Json.JsonIgnoreAttribute;

namespace Exptour.Common.Shared;

public class APIResponse<T> : BaseResponse<T>
{
    [JsonIgnore]
    public HttpStatusCode ResponseCode { get; set; } = HttpStatusCode.OK;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public IDictionary ErrorDetails { get; set; }

    public APIResponse(HttpStatusCode status = HttpStatusCode.OK, string message = "Request executed successfully", string state = "Success")
    {
        ResponseCode = status;
        Message = message;
        State = state;
    }
    public APIResponse(T _payload, HttpStatusCode status = HttpStatusCode.OK, string message = "Request executed successfully", string state = "Success")
    {
        Payload = _payload;
        ResponseCode = status;
        Message = message;
        State = state;
    }

    public APIResponse(string message) : base(message)
    {
    }

    public APIResponse(string message, string state) : base(message, state)
    {
    }

    public APIResponse(string message, T payload) : base(message, payload)
    {
    }
    public APIResponse(string message, string state, T payload) : base(message, state, payload)
    {
    }

    public IActionResult ToActionResult(bool passParam = true)
        => ResponseCode switch
        {
            HttpStatusCode.OK => passParam ? new OkObjectResult(this) : new OkResult(),
            HttpStatusCode.Unauthorized => new UnauthorizedResult(),
            HttpStatusCode.NotFound => passParam ? new NotFoundObjectResult(this) : new NotFoundResult(),
            _ => passParam ? new BadRequestObjectResult(this) : new BadRequestResult()
        };
}

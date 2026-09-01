using System.Text.Json.Serialization;

namespace FinanceManager.Responses
{
    public class Response<T> where T : class
    {
        public T? Data { get; set; }
        public int Code { get; set; } = 200;
        public string Message { get; set; } = string.Empty;

        [JsonConstructor]
        public Response()
        {
        }

        public Response(T? data, int code = 200, string? message = null)
        {
            Data = data;
            Code = code;
            Message = message ?? string.Empty;
        }

        [JsonIgnore]
        public bool IsSuccess => Code is >= 200 and < 300;
    }
}

using System.Text.Json.Serialization;

namespace FinanceManager.Responses
{
    public class Response<T> where T : class
    {
        private readonly int _code;
        public T? Data { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }


        [JsonConstructor]
        public Response() 
            =>  _code = 200;
        

   
        public Response(T? data, int code = 200, string? message = null) 
        {
            Data = data;
            Code = code;
            Message = message ?? string.Empty;
        }

        [JsonIgnore]
        public bool IsSuccess => Code >= 200 && Code < 299;
    }
}

namespace FinanceManager.Responses
{
    public class Response<T> where T : class
    {
        public T Data { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }

     

        public Response() { }

        public Response(int code, string message) { 
            Code = code;
            Message = message;
        }

        public Response(int code, string message, T data) 
        {
            Data = data;
            Code = code;
            Message = message;
        }
    }
}

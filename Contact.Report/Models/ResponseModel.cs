namespace Contact.Report.Models
{
    public class ResponseModel
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

        public ResponseModel(int statusCode, string message, object data)
        {
            StatusCode = statusCode;
            Message = message;
            Data = data;
        }

        public ResponseModel(int statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }
    }
}

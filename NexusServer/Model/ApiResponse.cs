namespace NexusServer.Model
{
    public class apiResponse
    {
        public string code { get; set; }
        public string message { get; set; }
        public object? responseData { get; set; }
    }
    public enum responseType
    {
        Success,
        NotFound,
        Failure
    }
}

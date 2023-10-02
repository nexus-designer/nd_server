namespace NexusServer.Model
{
    public class ResponseHandler
    {
        public static apiResponse GetExceptionResponse(Exception ex)
        {
            apiResponse response = new apiResponse();
            response.code = "1";
            response.responseData = ex.Message;
            return response;
        }
        public static apiResponse GetAppResponse(responseType type, object? contract)
        {
            apiResponse response;

            response = new apiResponse { responseData = contract };
            switch (type)
            {
                case responseType.Success:
                    response.code = "0";
                    response.message = "Success";

                    break;
                case responseType.NotFound:
                    response.code = "2";
                    response.message = "No record available";
                    break;
            }
            return response;
        }
    }
}

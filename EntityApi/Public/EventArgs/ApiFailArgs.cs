namespace EntityApi.Public.EventArgs
{
    public class ApiFailArgs : ApiResponseArgs
    {
        public ApiFailArgs(int statusCode, string info) : base(statusCode, info)
        {
        }
    }
}

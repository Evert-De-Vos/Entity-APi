namespace EntityApi.Public.EventArgs
{
    public class ApiSuccesArgs : ApiResponseArgs
    {
        public ApiSuccesArgs(int statusCode, string info) : base(statusCode, info)
        {

        }
    }

    public class ApiSuccesArgs<T> : ApiResponseArgs
    {
        public T Content { get; internal set; }
        public bool HasContent => Content != null;
        
        public ApiSuccesArgs(int statusCode, string info) : base(statusCode, info)
        {

        }
    } 
}

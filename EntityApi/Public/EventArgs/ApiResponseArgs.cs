using System;

namespace EntityApi.Public.EventArgs
{
    public class ApiResponseArgs : System.EventArgs
    {
        private int _statusCode;
        public int StatusCode
        {
            get => _statusCode;
            private set
            {
                if(value < 100 || value > 599)
                    throw new ArgumentException(message:"This is not a valid statuscode");
                _statusCode = value;
            }
        }

        public string Info { get; }

        public ApiResponseArgs(int statusCode, string info)
        {
            StatusCode = statusCode;
            Info = info;
        }
    }
}

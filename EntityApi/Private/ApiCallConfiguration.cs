using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using EntityApi.Public;
using EntityApi.Public.Enums;
using EntityApi.Public.Identity;
using Newtonsoft.Json;

namespace EntityApi.Private
{
    internal class ApiCallConfiguration
    {
        //reference to apiservice singleton
        private ApiService Api => ApiService.GetInstance();

        internal Dictionary<string, string> RequestHeaders;
        internal Dictionary<string, string> UrlParameters;
        internal Dictionary<string, string> ContentHeaders;

        internal HttpContent Body { get; private set; }

        private string _host;
        internal string Host
        {
            get => _host;
            set
            {
                if (!value.StartsWith("http://") && !value.StartsWith("https://"))
                    value = "https://" + value;

                _host = value;
            }
        }

        internal string Uri { get; set; }

        internal string Url => BuildUrl();

        internal int AuthenticationLevel { get; set; }

        internal HttpMethod RequestMethod;

        internal IdentityProvider Identity { get; set; }

        internal bool HasIdentity => Identity != null;
        internal bool HasBody => Body != null;

        internal ApiCallConfiguration()
        {
            RequestHeaders = new Dictionary<string, string>();
            UrlParameters = new Dictionary<string, string>();
            ContentHeaders = new Dictionary<string, string>();
            AuthenticationLevel = 0;
        }

        public void AddBody(object content, ContentEncoding encoding = ContentEncoding.AppJson)
        {
            switch (encoding)
            {
                case ContentEncoding.AppFormUrlEncoded:
                    if (content.GetType() != typeof(string))
                        throw new ArgumentException("formurlencoded content must be a string");
                    Body = new StringContent((string)content);
                    Body.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                    break;
                case ContentEncoding.AppJson:
                default:
                    Body = new StringContent(JsonConvert.SerializeObject(content));
                    Body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    break;
            }
        }

        /// <summary>
        ///     Add parameters to URL
        /// </summary>
        /// <param name="key"> URL placeholder </param>
        /// <param name="value"> actual value</param>
        internal void AddParam(string key, string value)
        {
            key = key.Trim().ToLower();

            UrlParameters.Add(key, value);
        }

        /// <summary>
        ///     add header to request
        /// </summary>
        /// <param name="key"> header key</param>
        /// <param name="value"> header value </param>
        internal void AddHeader(string key, string value)
        {
            RequestHeaders.Add(key, value);
        }

        internal void AddContentHeader(string key, string value)
        {
            ContentHeaders.Add(key, value);
        }

        /// <summary>
        ///     create apicall for this request
        /// </summary>
        /// <returns></returns>
        public virtual ApiCall CreateCall()
        {
            ValidateRequest();

            return new ApiCall(this);
        }
        /// <summary>
        ///     create apicall for this request
        /// </summary>
        /// <typeparam name="T"> type compatibel with response body </typeparam>
        /// <returns></returns>
        public ApiCall<T> CreateCall<T>()
        {
            ValidateRequest();

            return new ApiCall<T>(this);
        }

        /// <summary>
        ///     checks if current information can build a valid request
        /// </summary>
        private void ValidateRequest()
        {
            if (Host == null)
                throw new ArgumentNullException($"Host", "Tried to create API call without sprecifying targetted host");

            if (Uri == null)
                throw new ArgumentNullException($"Location", "Tried to create API call without sprecifying location");

            if (RequestMethod == null)
                throw new ArgumentNullException($"Method", "Tried to create API call without sprecifying a request method");

            BuildUrl();
        }

        /// <summary>
        ///     replace all parameters in url and add host to url
        /// </summary>
        /// <returns></returns>
        private string BuildUrl()
        {
            var stringBuilder = new StringBuilder(Uri);

            var rx = new Regex("({[^{ }]*})");

            var matches = rx.Matches(Uri);

            foreach (Match match in matches)
            {
                var param = match.Value.Trim('{', '}').ToLower();

                if (!UrlParameters.ContainsKey(param))
                    throw new ArgumentException("not all parameters in route are defined");

                stringBuilder.Replace(match.Value, UrlParameters[param]);
            }

            stringBuilder.Insert(0, Host + "/");

            return stringBuilder.ToString();
        }
    }
}

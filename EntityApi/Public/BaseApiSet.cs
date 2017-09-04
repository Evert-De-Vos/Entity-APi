using System;
using System.Net.Http;
using EntityApi.Private;
using EntityApi.Public.Enums;
using EntityApi.Public.Identity;

namespace EntityApi.Public
{
    public abstract class BaseApiSet
    {

        private ApiCallConfiguration _currentConfiguration;

        /// <summary>
        ///     creates a new instance of ApiCzllConfiguartion if the current one is null
        /// </summary>
        internal ApiCallConfiguration CurrentConfiguration => _currentConfiguration ?? (_currentConfiguration = new ApiCallConfiguration());

        internal string HostAdress;

        internal IdentityProvider IdentityProvider;

        protected BaseApiSet()
        {
        }

        protected BaseApiSet(string hostAdress)
        {
            HostAdress = hostAdress;
        }

        //methoden om apicallconfig vol te proppen
        protected void Host(string hostAdress)
        {
            CurrentConfiguration.Host = hostAdress;
        }

        /// <summary>
        ///     sets the request route
        /// </summary>
        /// <param name="uri"></param>
        protected void Route(string uri)
        {
            CurrentConfiguration.Uri = uri;
        }

        /// <summary>
        ///     sets the request method
        /// </summary>
        /// <param name="method"></param>
        protected void RequestMethod(HttpMethod method)
        {
            CurrentConfiguration.RequestMethod = method;
        }
        
        /// <summary>
        ///     adds a header to the request 
        /// </summary>
        /// <param name="key"> header key </param>
        /// <param name="value"> header value </param>
        protected void Header(string key, string value)
        {
            CurrentConfiguration.AddHeader(key, value);
        }

        protected void ContentHeader(string key, string value)
        {
            CurrentConfiguration.AddContentHeader(key, value);
        }

        /// <summary>
        ///     adds a parameter to the route url
        /// </summary>
        /// <param name="key"> placeholder name </param>
        /// <param name="value"> actual value </param>
        protected void Parameter(string key, string value)
        {
            CurrentConfiguration.AddParam(key, value);
        }

        /// <summary>
        ///     adds the object to the request body
        /// </summary>
        /// <param name="content"></param>
        protected void Body(object content, ContentEncoding encoding = ContentEncoding.AppJson)
        {
            _currentConfiguration.AddBody(content,encoding);
        }

        /// <summary>
        ///     sets the authenticationlevel required to execute this request
        /// </summary>
        /// <param name="level"></param>
        protected void AuthenticationLevel( int level )
        {
            if(IdentityProvider == null) throw new Exception("You can't add an authenticationlevel to a context without an identity provider");
            CurrentConfiguration.AuthenticationLevel = level;
        }
        
        internal void ResetCurrentConfiguration()
        {
            _currentConfiguration = null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EntityApi.Public.Identity
{
    public abstract class OAuth2Identity : IdentityProvider
    {
        protected string AuthenticationToken;
        protected DateTime AuthenticationExpirationDate;
        protected TimeSpan AuthenticationBuffer;


        protected int _authenticationLevel;

        protected string RefreshToken;
        protected DateTime RefreshExpirationDate;
        protected TimeSpan RefreshBuffer;

        protected OAuth2Identity()
        {
            ExtendTokens();
        }

        public override bool IsAuthenticated()
        {
            var flag = AuthenticationToken != null && AuthenticationExpirationDate > DateTime.Now;

            if (flag)
                ExtendTokens();

            return flag;
        }

        public override bool IsAuthenticated(int authorizationLevel)
        {
            return IsAuthenticated() && authorizationLevel > _authenticationLevel;
        }

        /// <summary>
        ///     checks if current identity has a refresh token
        /// </summary>
        /// <returns></returns>
        public bool HasRefreshtoken()
        {
            return RefreshToken != null && RefreshExpirationDate > DateTime.Now;
        }

        /// <summary>
        ///     autthenticates the user using it's refreshtoken
        /// </summary>
        /// <returns></returns>
        public virtual async Task<AuthenticationResult> Authenticate()
        {
            if (HasRefreshtoken())
                return await RefreshAuthenticationTokens();

            return AuthenticationResult.Unauthorized;
        }

        /// <summary>
        ///     authenticates the user using username and password
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="password"></param>
        /// <param name="forceAuthentication"></param>
        /// <returns></returns>
        public virtual async Task<AuthenticationResult> Authenticate(string identity, string password, bool forceAuthentication = false)
        {
            if (forceAuthentication)
                return await AquireAuthenticationToken(identity, password);


            if (IsAuthenticated())
                return AuthenticationResult.Authenticated;

            if (HasRefreshtoken())
            {
                var response = await RefreshAuthenticationTokens();

                if (response > AuthenticationResult.Unauthorized)
                {
                    return response;
                }
            }

            return await AquireAuthenticationToken(identity, password);
        }

        /// <summary>
        ///     method used to refresh authenticationtokens
        /// </summary>
        /// <returns></returns>
        public abstract Task<AuthenticationResult> RefreshAuthenticationTokens();

        /// <summary>
        ///     method used to aquire authenticationtokens
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public abstract Task<AuthenticationResult> AquireAuthenticationToken(string username, string password);
        
        /// <summary>
        ///     method used to add authenticationheaders to request
        /// </summary>
        /// <returns></returns>
        protected override KeyValuePair<string, string> BuildAuthenticationHeader()
        {
            return new KeyValuePair<string, string>("Authentication", "bearer " + AuthenticationToken);
        }

        /// <summary>
        ///     extends tokens if system has a refreshtoken
        /// </summary>
        /// <param name="forceRefresh"></param>
        private async void ExtendTokens(bool forceRefresh = false)
        {
            if (!HasRefreshtoken())
                return;

            if (forceRefresh || AuthenticationExpirationDate - AuthenticationBuffer < DateTime.Now || RefreshExpirationDate - RefreshBuffer < DateTime.Now)
                await RefreshAuthenticationTokens();
        }
    }
}

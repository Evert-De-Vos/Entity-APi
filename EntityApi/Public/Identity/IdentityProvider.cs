﻿using System.Collections.Generic;

namespace EntityApi.Public.Identity
{
    public abstract class IdentityProvider : BaseApiSet
    {
        
        internal KeyValuePair<string, string> AuthenticationHeader => BuildAuthenticationHeader();

        /// <summary>
        ///     Check wether the user is authenticated
        /// </summary>
        /// <returns></returns>
        public abstract bool IsAuthenticated();
        
        /// <summary>
        ///     Check wether the user has given authenticationlevel
        /// </summary>
        /// <param name="authorizationLevel"></param>
        /// <returns></returns>
        public abstract bool IsAuthenticated(int authorizationLevel);
                
        /// <summary>
        ///     Method used to add authenticationheader to request
        /// </summary>
        /// <returns></returns>
        protected abstract KeyValuePair<string, string> BuildAuthenticationHeader();

    }

    
}

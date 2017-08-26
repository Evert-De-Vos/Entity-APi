namespace EntityApi.Public
{
    public class ApiSet : BaseApiSet
    {
        /// <summary>
        ///     creates the apicall object needed to activate the request
        /// </summary>
        /// <returns></returns>
        protected virtual ApiCall Create()
        {
            if (HostAdress != null && CurrentConfiguration.Host == null)
                CurrentConfiguration.Host = HostAdress;

            var call = CurrentConfiguration.CreateCall();
            ResetCurrentConfiguration();
            return call;
        }

        /// <summary>
        ///     creates the apicall object needed to activate the request
        /// </summary>
        /// <typeparam name="T"> response will be cast to this type</typeparam>
        /// <returns></returns>
        protected virtual ApiCall<T> Create<T>()
        {
            if (HostAdress != null && CurrentConfiguration.Host == null)
                CurrentConfiguration.Host = HostAdress;

            if (IdentityProvider != null)
                CurrentConfiguration.Identity = IdentityProvider;

            var call = CurrentConfiguration.CreateCall<T>();
            ResetCurrentConfiguration();
            return call;
        }

    }
}

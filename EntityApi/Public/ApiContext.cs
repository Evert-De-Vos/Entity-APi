using System;
using EntityApi.Private.Utitlities;
using EntityApi.Public.Identity;

namespace EntityApi.Public
{
    public abstract class ApiContext
    {
        internal readonly bool IsInstantiated;

        private string _standardHost;
        public string StandardHost
        {
            get => _standardHost;
            protected set
            {
                if (IsInstantiated) throw new Exception("you can only set the standardhost in the initContext method");

                _standardHost = value;
            }
        }

        private IdentityProvider _identity;
        public IdentityProvider IdentityProvider
        {
            get => _identity;
            protected set
            {
                if (IsInstantiated) throw new Exception("you can only set Identity in the initContext method");

                _identity = value;
            }
        }

        protected ApiContext()
        {
            InitContext();
            IsInstantiated = true;
            new ApiSetDiscoveryService(this);
        }

        protected virtual void InitContext()
        {
            
        }
       
    }
}

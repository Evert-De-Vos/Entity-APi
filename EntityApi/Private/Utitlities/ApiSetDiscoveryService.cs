using System;
using System.Collections.Generic;
using System.Reflection;
using EntityApi.Public;
using EntityApi.Public.Identity;

namespace EntityApi.Private.Utitlities
{
    internal class ApiSetDiscoveryService
    {
        //reference to the context that needs initialization
        private readonly ApiContext _context;

        public ApiSetDiscoveryService(ApiContext context)
        {
            _context = context;
            InitializeApiSets();
        }

        /// <summary>
        ///     This method looks for APIset variables and instantiates them
        /// </summary>
        private void InitializeApiSets()
        {
            var entitySets = new List<PropertyInfo>();
            foreach (var propertyInfo in _context.GetType().GetProperties())
            {
                var flag = true;
                var derrived = propertyInfo.PropertyType;
                while (flag
                    && derrived != null
                    && derrived != typeof(IdentityProvider)
                    )
                {
                    if (derrived == typeof(BaseApiSet)) {
                        entitySets.Add(propertyInfo);
                        flag = false;
                        continue;
                    }

                    derrived = derrived.GetTypeInfo().BaseType;
                }
            }

            foreach (var set in entitySets)
            {
                var instance = (BaseApiSet)Activator.CreateInstance(set.PropertyType);

                if (_context.StandardHost != null && instance.HostAdress == null)
                    instance.HostAdress = _context.StandardHost;

                if (_context.IdentityProvider != null)
                    instance.IdentityProvider = _context.IdentityProvider;

                //if(instance.GetType() == typeof(TrackedApiSet<>))
                //    ((TrackedApiSet<>) instance).SaveChangesEvent += _context.

                _context.GetType().GetRuntimeProperty(set.Name).SetValue(_context, instance, null);

            }
        }
    }
}

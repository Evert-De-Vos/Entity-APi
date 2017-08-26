using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using EntityApi.Private.Utitlities;

namespace EntityApi.Public
{
    public abstract class TrackedApiSet<T> : BaseApiSet where T : INotifyPropertyChanged, new()
    {
        private readonly ChangeTracker<T> _trackedEntities;
        private List<PropertyInfo> _idProperties;
        
        protected TrackedApiSet()
        {

        }
        
        protected ApiCall<T> Create()
        {
            if (HostAdress != null && CurrentConfiguration.Host == null)
                CurrentConfiguration.Host = HostAdress;

            var call = CurrentConfiguration.CreateCall<T>();
            ResetCurrentConfiguration();

            call.OnSuccess(( args) =>
            {
                if (args.HasContent)
                    _trackedEntities.TrackExternalEntity(args.Content);
            });

            return call;
        }

        protected ApiCall<List<T>> CreateBulkCall()
        {
            if (HostAdress != null && CurrentConfiguration.Host == null)
                CurrentConfiguration.Host = HostAdress;

            var call = CurrentConfiguration.CreateCall<List<T>>();
            ResetCurrentConfiguration();

            call.OnSuccess(( args) =>
            {
                if (!args.HasContent) return;

                args.Content.ForEach(item => _trackedEntities.TrackExternalEntity(item));

            });

            return call;
        }

        public abstract string GenerateIdentity(T entity);

        protected abstract ApiCall UpdateEntity(T entity);

        protected abstract ApiCall DeleteEntity(T entity);

        protected abstract ApiCall AddEntity(T entity);

        public void Add(T entity)
        {
            _trackedEntities.Add(entity);
        }

        public void Remove(T entity)
        {
            _trackedEntities.Remove(entity);
        }

        protected virtual void OnSaveChanges(IEnumerable<EntityTracker<T>> changes)
        {
            
        }

        public void SaveChanges()
        {
            var trackers =  _trackedEntities.ChangedEntities.ToList();
            
            OnSaveChanges(trackers);

            foreach (var entity in trackers)
            {
                switch (entity.Status)
                {
                    case ChangeStatus.Added:
                        AddEntity(entity.reference);
                        break;
                    case ChangeStatus.Deleted:
                        DeleteEntity(entity.reference);
                        break;
                    case ChangeStatus.Updated:
                        UpdateEntity(entity.reference);
                        break;
                    case ChangeStatus.Original:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        //private string GenerateIdentity(T entity)
        //{
        //    var idString = new StringBuilder();

        //    foreach (var property in _idProperties)
        //    {
        //        var value = entity.GetType().GetProperty(property.Name)?.GetValue(entity).ToString();
        //        if (value == null)
        //            throw new ArgumentNullException(this.GetType().Name,"identityfields MUST be filled in!");
        //        idString.Append(value);
        //    }

        //    return idString.ToString();
        //}
    }
}

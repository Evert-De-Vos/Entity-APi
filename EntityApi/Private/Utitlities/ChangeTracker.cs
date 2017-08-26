using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace EntityApi.Private.Utitlities
{
    internal class ChangeTracker<T> where T: INotifyPropertyChanged
    {
        //list of tracked entities
        private readonly Dictionary<string,EntityTracker<T>> _trackers;

        internal Func<T, string> ExtractIdentityFunction;
        
        /// <summary>
        ///     returns a list of changed entities
        /// </summary>
        public IEnumerable<EntityTracker<T>> ChangedEntities => _trackers.Values.Where(t => t.HasChanged()).ToList();

        public ChangeTracker(Func<T,string> extractIdentFunc)
        {
            ExtractIdentityFunction = extractIdentFunc;
            _trackers = new Dictionary<string, EntityTracker<T>>();
        }


        /// <summary>
        ///    Add a tracker for an entity aquired in an api response
        /// </summary>
        /// <param name="entity"> Entity that needs to be tracked </param>
        public void TrackExternalEntity(T entity)
        {
            var id = ExtractIdentityFunction(entity);
            var existingTracker = _trackers[id];

            if (existingTracker == null)
                _trackers.Add(id, new EntityTracker<T>(entity));
            
            existingTracker?.Update(entity);
        }

        /// <summary>
        ///     Add a new Entity
        /// </summary>
        /// <param name="entity"></param>
        public void Add(T entity)
        {
            var id = ExtractIdentityFunction(entity);
            var existingTracker = _trackers[id];

            if (existingTracker == null)
            {
                _trackers.Add(id,new EntityTracker<T>(entity){Status = ChangeStatus.Added});
            }
                
        }

        /// <summary>
        ///     Remove Existing entity
        /// </summary>
        /// <param name="entity"></param>
        public void Remove(T entity)
        {
            var id = ExtractIdentityFunction(entity);
            var existingTracker = _trackers[id];

            if (existingTracker != null)
                existingTracker.Status = ChangeStatus.Deleted;
        }
    }
}

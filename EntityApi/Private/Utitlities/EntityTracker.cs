using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace EntityApi.Private.Utitlities
{
    public class EntityTracker<T> where T : INotifyPropertyChanged
    {
        //reference to tracked entity
        public T reference { get; set; }
        //private T original;
        
        public ChangeStatus Status;

        private EntityTracker()
        {

        }

        internal EntityTracker(T entity)
        {
            reference = entity;
            //original = CloneEntity(entity);
            Status = ChangeStatus.Original;

            entity.PropertyChanged += OnEntityChanged;
        }

        /// <summary>
        ///     Update reference with new information from API
        /// </summary>
        /// <param name="entity"></param>
        internal void Update(T entity)
        {
            foreach (var prop in entity.GetType().GetProperties().Where(p => p.SetMethod != null))
            {
                reference.GetType().GetProperty(prop.Name)?.SetMethod.Invoke(reference,new [] {prop.GetValue(prop)});
            }

            Status = ChangeStatus.Original;
        }

        /// <summary>
        ///     returns an exact copy of given entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private static T CloneEntity(T entity)
        {
            var clone = JsonConvert.SerializeObject(entity); // cloning is hard so for now i just serialize -> deserialize.
            return JsonConvert.DeserializeObject<T>(clone);
        }

        /// <summary>
        ///     checks if given entity matches any of the tracked entities
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        internal bool MatchInternalEntity(T entity)
        {
            return object.ReferenceEquals(reference, entity);
        }

        /// <summary>
        ///     checks this entity for changes 
        /// </summary>
        /// <returns></returns>
        public bool HasChanged()
        {
            if (Status != ChangeStatus.Original) return true;

            //later on this method should check for changes instead of using the entitychanged event
            return false;
        }
        
        private void OnEntityChanged(object entity, PropertyChangedEventArgs args)
        {
            if (Status < ChangeStatus.Updated)
                Status = ChangeStatus.Updated;
        }
    }
}

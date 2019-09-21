using System;
using System.Collections.Generic;
using System.Diagnostics;
using DnDTracker.Web.Logging;

namespace DnDTracker.Web
{
    /// <summary>
    /// Because using Siege is for chubs.
    /// </summary>
    public class Singleton
    {
        private static Singleton _instance;

        private Dictionary<Type, object> _instances;

        /// <summary>
        /// Initializes the static Singleton instance on the first time only.
        /// </summary>
        /// <returns>The new, or original, instance of the static Singleton.</returns>
        public static Singleton Initialize()
        {
            if (_instance != null) return _instance;

            _instance = new Singleton()
            {
                _instances = new Dictionary<Type, object>()
            };
            return _instance;
        }
        
        /// <summary>
        /// Functions the same as <see cref="Add{T}(object)"/> but allows reinstancing during testing.
        /// </summary>
        /// <typeparam name="T">The type to use as a key when retrieving an instance during runtime.</typeparam>
        /// <param name="o">The instance to use as a value when retrieved by the key.</param>
        /// <returns>The current Singleton instance.</returns>
        public Singleton Override<T>(object o)
        {
            // Return immediately if this is not being called from a Test class.
            var callingType = new StackTrace().GetFrame(1).GetMethod().DeclaringType;
            if (!callingType.Name.Contains("Test"))
                return this;

            var type = typeof(T);

            if (_instances.ContainsKey(type))
                _instances[type] = o;
            else
                _instances.Add(type, o);

            return this;
        }

        /// <summary>
        /// Adds a key <typeparamref name="T"/> mapped to an instance <paramref name="o"/>. This does not allow duplication or reinstancing.
        /// </summary>
        /// <typeparam name="T">The type to use as a key when retrieving an instance during runtime.</typeparam>
        /// <param name="o">The instance to use as a value when retrieved by the key.</param>
        /// <returns>The current Singleton instance.</returns>
        public Singleton Add<T>(object o)
        {
            var type = typeof(T);

            if (_instances.ContainsKey(type))
                Log.Error($"Duplicate instance registered during Singleton registration! Type: {type.Name}");
            else
                _instances.Add(type, o);

            return this;
        }

        /// <summary>
        /// Retrieves the registered Singleton instance for the specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to use as a key when retrieving an instance.</typeparam>
        /// <returns>The instance of type <typeparamref name="T"/> OR null.</returns>
        public static T Get<T>()
        {
            var type = typeof(T);
            if (_instance?._instances?.ContainsKey(type) ?? false)
                return (T)_instance._instances[type];
            return default;
        }
    }
}

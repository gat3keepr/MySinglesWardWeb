using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Enyim.Caching;
using Enyim.Caching.Memcached;
using System.Data.Linq;
using System.Reflection;
using MemcachedProviders.Cache;

namespace MSW.Utilities
{
    public static class Cache
    {
        //private static Enyim.Caching.MemcachedClient _cache = new Enyim.Caching.MemcachedClient();
		public static string getCacheKey<T>(object id) { return typeof(T).FullName + ":" + id.ToString(); }

		/// <summary>
		/// Gets an object out of cache. Looks in the HTTPContext first, then will hit the cache server.
		/// If it is found in the cache server, it will put it in HTTPContext incase the object
		/// is needed again during this request.
		/// </summary>
		/// <param name="key">Cache Key</param>
		/// <returns>returns the object found in cache or null if the object is not in cache</returns>
        public static object Get(string key)
        {
			object obj = null;

           if (!requestCache(key))
            {
				obj = DistCache.Get(key);

				if (obj != null)
					requestCacheSet(key, obj);

				return obj;
            }

			return getRequestCache(key);
        }

		/// <summary>
		/// Sets the object in cache. This puts it in HTTPContext in case the object is 
		/// needed again during this request.
		/// </summary>
		/// <param name="key">Cache Key</param>
		/// /// <param name="obj">Cacheable Object</param>
		/// <returns>returns the object found in cache or null if the object is not in cache</returns>
        public static void Set(string key, object obj)
        {
            DistCache.Add(key, obj);
			//_cache.Store(StoreMode.Set, key, obj);
			requestCacheSet(key, obj);
        }

		/// <summary>
		/// Removes the object from the cache server and the HTTPContext
		/// </summary>
		/// <param name="key">Cache Key</param>
		/// <returns>returns the object found in cache or null if the object is not in cache</returns>
        public static void Remove(string key)
        {
            DistCache.Remove(key);
			//_cache.Remove(key);

			if(requestCache(key))
				requestCacheRemove(key);
        }

        /// <summary>
        /// delegate that resolves the cache key given an argument of type <typeparamref name="KT"/>
        /// </summary>
        /// <typeparam name="KT">the type</typeparam>
        /// <param name="i">the type</param>
        /// <returns>should return the cache key based on <paramref name="i"/>.</returns>
        public delegate string KeyResolver<KT>(KT i);

        /// <summary>
        /// delegate that resolves an object of type <typeparamref name="T"/> given
        /// an identifier of type <typeparamref name="KT"/>.  The delegate has the 
        /// responsibility to cache the returned object if necessary.
        /// </summary>
        /// <typeparam name="T">the type to return</typeparam>
        /// <typeparam name="KT">the type of the key</typeparam>
        /// <param name="id">the identifier of the object to fetch</param>
        /// <returns>the fetched object.</returns>
        public delegate T ObjectFactory<T, KT>(KT id);

        /// <summary>
        /// Get an ordered set of items from the cache, using a set of key values, a key resolver, and an object factory for missing objects.
        /// </summary>
        /// <typeparam name="T">the type of the objects being retrieved</typeparam>
        /// <typeparam name="KT">the type of the key used to build cache keys</typeparam>
        /// <param name="keys">a collection of <typeparamref name="KT"/>, used to build the cache keys and order the fetched collection</param>
        /// <param name="keyResolver">a delegate to call to transorm <typeparamref name="KT"/> to cache keys</param>
        /// <param name="objectFactory">a delegate to call to retrieve the objects from their 
        /// permanent storage, should the cache fail to have them.  The objectFactory is also responsible for 
        /// caching the entities, if necessary.</param>
        /// <returns>an IEnumerable of <typeparamref name="T"/> in the same order as <paramref name="keys"/>.</returns>
        public static List<T> GetList<T, KT>(IEnumerable<KT> keys, KeyResolver<KT> keyResolver, ObjectFactory<T, KT> objectFactory) where T : class
        {
            List<T> results = new List<T>();

            // build the cache key array
            IEnumerable<string> cacheKeys = from x in keys select keyResolver(x);
            // fetch it.
            IDictionary<string, object> cacheResults = null;

            cacheResults = DistCache.Get(cacheKeys.ToArray());//_cache.Get(cacheKeys);
            
            // did it come back complete?
            if (cacheResults == null)
                cacheResults = new Dictionary<string, object>();

            // put the collection in the desired order
            foreach (KT i in keys)
            {
                string cacheKey = keyResolver(i);
                if (cacheResults.ContainsKey(cacheKey))
                {
                    if (cacheResults[cacheKey] as T != null)
                    {
                        results.Add(cacheResults[cacheKey] as T);
                        continue;
                    }
                }

                // it was missing, or of the wrong type, call the object factory.
                results.Add(objectFactory(i));
            }

            return results;
        }

		private static bool requestCache(string key)
		{
			if (HttpContext.Current.Items["requestCache"] == null)
				HttpContext.Current.Items["requestCache"] = new Dictionary<string, object>();

			if ((HttpContext.Current.Items["requestCache"] as Dictionary<string, object>).ContainsKey(key))
				return true;

			return false;
		}

		private static void requestCacheSet(string key, object obj)
		{
			if (HttpContext.Current.Items["requestCache"] == null)
				HttpContext.Current.Items["requestCache"] = new Dictionary<string, object>();

			(HttpContext.Current.Items["requestCache"] as Dictionary<string, object>)[key] = obj;
		}

		private static void requestCacheRemove(string key)
		{
			(HttpContext.Current.Items["requestCache"] as Dictionary<string, object>).Remove(key);
		}

		private static object getRequestCache(string key)
		{
			return (HttpContext.Current.Items["requestCache"] as Dictionary<string, object>)[key];
		}
    }

	public static class Extensions
	{
		public static void ClearCache(this DataContext context)
		{
			const BindingFlags FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

			var method = context.GetType().GetMethod("ClearCache", FLAGS);
			method.Invoke(context, null);
		}
	} 
}
using System.Collections.Generic;
using UnityEngine;

namespace Core.Common.Extensions.Hashing
{
    public static class HashingExtensions
    {
        private static readonly Dictionary<string, ulong> CachedHashes = new Dictionary<string, ulong>();

        public static ulong GetTypeHash(this Object obj)
        {
            return GetHash(obj);
        }
        
        public static ulong GetTypeHash(this Component obj)
        {
            return GetHash(obj);
        }

        private static ulong GetHash(Object obj)
        {
            var typeName = obj.GetType().FullName;
            if (CachedHashes.ContainsKey(typeName))
                return CachedHashes[typeName];

            var hash = 14695981039346656037UL; //offset
            for (var i = 0; i < typeName.Length; i++)
            {
                hash = hash ^ typeName[i];
                hash *= 1099511628211UL; //prime
            }

            CachedHashes.Add(typeName, hash);
            return hash;
        }
    }
}
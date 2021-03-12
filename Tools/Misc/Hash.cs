using System.Collections.Generic;

namespace Core.Tools.Misc
{
    public class Hash
    {
        private static readonly Dictionary<string, ulong> CachedHashes = new Dictionary<string, ulong>();
        
        public static ulong GetHash(string value)
        {
            var typeName = value;
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
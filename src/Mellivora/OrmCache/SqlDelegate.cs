using System;
using System.Collections.Concurrent;
using System.Data;

namespace Mellivora.DynamicCache
{
    public static class SqlDelegate<T>
    {
        public delegate void GetCommandByInstance(ref IDbCommand source, object instance);
        public delegate void GetGenericCommand(ref IDbCommand source, T instance);
        public delegate void GetCommandByObject(ref IDbCommand source, object[] instances);
        public delegate T GetReaderInstance(IDataReader reader);
        public readonly static ConcurrentDictionary<int, ConcurrentDictionary<string, GetCommandByInstance>> CommandInstancesCache;
        public readonly static ConcurrentDictionary<string, GetCommandByObject> CommandObjectsCache;
        public readonly static ConcurrentDictionary<string, GetReaderInstance> SingleReaderCache;
        public readonly static ConcurrentDictionary<string, GetGenericCommand> CommandGenericCache;
        public readonly static ConcurrentDictionary<Tuple<string,int,int>, GetReaderInstance> ComplexReaderCache;

        static SqlDelegate()
        {
            CommandInstancesCache = new ConcurrentDictionary<int, ConcurrentDictionary<string, GetCommandByInstance>>();
            CommandObjectsCache = new ConcurrentDictionary<string, GetCommandByObject>();
            CommandGenericCache = new ConcurrentDictionary<string, GetGenericCommand>();
            SingleReaderCache = new ConcurrentDictionary<string, GetReaderInstance>();
            ComplexReaderCache = new ConcurrentDictionary<Tuple<string, int, int>, GetReaderInstance>();
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Vasily
{
    public static class Cache
    {
        public static ConcurrentDictionary<Type, SqlModel> SqlCache;
        public static ConcurrentDictionary<Type, ModelStruction> StructionCache;
        public static ConcurrentDictionary<Type, List<Type>> OuterTypeCache;
        public static ConcurrentDictionary<Type, string> OuterSelectCache;
        public static ConcurrentDictionary<Type, ConcurrentDictionary<string, string>> ColumnMapCache;

        static Cache()
        {
            SqlCache = new ConcurrentDictionary<Type, SqlModel>();
            OuterTypeCache = new ConcurrentDictionary<Type, List<Type>>();
            OuterSelectCache = new ConcurrentDictionary<Type, string>();
            ColumnMapCache = new ConcurrentDictionary<Type, ConcurrentDictionary<string, string>>();
            StructionCache = new ConcurrentDictionary<Type, ModelStruction>();
        }
    }
}

using System;
using System.Collections.Concurrent;

namespace Vasily
{
    public static class Sql<T>
    {
        public static ConcurrentDictionary<string, string> ColumnToRealMap;
        public static ConcurrentDictionary<string, string> RealToColumnMap;
        public static ConcurrentDictionary<string, string> ALMap;
        public static ModelStruction Struction;
        static Sql()
        {
            ColumnToRealMap = new ConcurrentDictionary<string, string>();
            RealToColumnMap = new ConcurrentDictionary<string, string>();
            ALMap = new ConcurrentDictionary<string, string>();
        }

        public static string GetColumnName(string key)
        {
            if (RealToColumnMap.ContainsKey(key))
            {
                return RealToColumnMap[key];
            }
            return key;
        }
        public static string GetRealName(string key)
        {
            if (ColumnToRealMap.ContainsKey(key))
            {
                return ColumnToRealMap[key];
            }
            return key;
        }

        public static string AL(string key)
        {
            if (ALMap.ContainsKey(key))
            {
                return ALMap[key];
            }
            return null;
        }

        public static bool IsMaunally;
        public static string Table;
        public static string PrimaryKey;


        public static string Insert;
        public static string Update;
        public static string Select;
        public static string Delete;

        public static string ConditionUpdate;
        public static string ConditionDelete;
        public static string ConditionSelect;

        public static string SelectAll;
        public static string CheckRepeate;
        public static string GetPrimaryKey;
    }
}

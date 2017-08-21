using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Vasily.Utils;

namespace Vasily
{
    public static class VasilyHandler
    {
        /// <summary>
        /// 开局必须调用的函数
        /// </summary>
        /// <param name="interfaceName">如果自己有特殊接口，那么可以写自己的接口名</param>
        public static void Initialize(string interfaceName = "IVasily")
        {
            Assembly assmbly = Assembly.GetEntryAssembly();
            if (assmbly == null) { return; }
            IEnumerator<Type> typeCollection = assmbly.ExportedTypes.GetEnumerator();
            Type temp_Type = null;
            while (typeCollection.MoveNext())
            {
                temp_Type = typeCollection.Current;
                if (temp_Type.IsClass && !temp_Type.IsAbstract)
                {
                    if (temp_Type.GetInterface(interfaceName) != null)
                    {
                        ModelAnalyser.Initialization(temp_Type);
                    }
                }
            }
        }
    }
}

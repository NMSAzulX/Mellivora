using System.Collections.Generic;
using System.Data;
using Vasily;

namespace Mellivora
{
    public static class DbConnectionALExtension
    {
        /// <summary>
        /// 通过实体类对Command进行填充，通过Vasily的AL逻辑进行数据查询
        /// </summary>
        /// <typeparam name="T">需要返回数据的类型</typeparam>
        /// <typeparam name="S">AL实体类类型</typeparam>
        /// <param name="connection">对IDbConnection进行扩展</param>
        /// <param name="key">AL的键</param>
        /// <param name="instance">实体类</param>
        /// <returns>数据集合</returns>
        public static IEnumerable<T> GetIAL<T,S>(this IDbConnection connection, string key, object instance)
        {
            return connection.QueryByInstance<T>(Sql<S>.ALMap[key], instance);
        }
        /// <summary>
        /// 通过object数组对Command进行填充，通过Vasily的AL逻辑进行数据查询
        /// </summary>
        /// <typeparam name="T">需要返回数据的类型</typeparam>
        /// <typeparam name="S">AL实体类类型</typeparam>
        /// <param name="connection">对IDbConnection进行扩展</param>
        /// <param name="key">AL的键</param>
        /// <param name="instance">object参数数组</param>
        /// <returns>数据集合</returns>
        public static IEnumerable<T> GetOAL<T,S>(this IDbConnection connection, string key, params object[] instance)
        {
            return connection.QueryByObjects<T>(Sql<S>.ALMap[key], instance);
        }
        /// <summary>
        /// 执行AL逻辑的ExecuteNonQuery操作
        /// </summary>
        /// <typeparam name="T">忽略</typeparam>
        /// <param name="connection">对IDbConnection进行扩展</param>
        /// <param name="key">AL的键</param>
        /// <param name="instance">command参数化所需的实体类</param>
        /// <returns>数据库数据变化数量</returns>
        public static int ExecuteIAL<T>(this IDbConnection connection, string key, T instance)
        {
            return connection.ExecuteNonQueryByInstance(Sql<T>.ALMap[key], instance);
        }
        /// <summary>
        /// 执行AL逻辑的ExecuteNonQuery操作
        /// </summary>
        /// <typeparam name="T">AL实体类类型</typeparam>
        /// <param name="connection">对IDbConnection进行扩展</param>
        /// <param name="key">AL的键</param>
        /// <param name="instance">object参数数组</param>
        /// <returns>数据库数据变化数量</returns>
        public static int ExecuteOAL<T>(this IDbConnection connection, string key, params object[] instance)
        {
            return connection.ExecuteNonQueryByObject(Sql<T>.ALMap[key], instance);
        }
    }
}

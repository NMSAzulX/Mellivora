using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vasily;

namespace Mellivora
{
    public static class DbConnectionCurdExtension
    {
        /// <summary>
        /// 向数据库中增加实例
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="connection">对IDbConnection扩展</param>
        /// <param name="instance">实例</param>
        /// <returns>影响行数</returns>
        public static int Add<T>(this IDbConnection connection, T instance)
        {
           return connection.ExecuteNonQueryByInstance(Sql<T>.Insert,instance);
        }
        /// <summary>
        /// 删除数据库中实例
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="connection">对IDbConnection扩展</param>
        /// <param name="instance">实例</param>
        /// <returns>影响行数</returns>
        public static int Delete<T>(this IDbConnection connection, T instance)
        {
            return connection.ExecuteNonQueryByInstance(Sql<T>.Delete, instance);
        }
        /// <summary>
        /// 修改数据库中实例
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="connection">对IDbConnection扩展</param>
        /// <param name="instance">实例</param>
        /// <returns>影响行数</returns>
        public static int Modify<T>(this IDbConnection connection,T instance)
        {
            return connection.ExecuteNonQueryByInstance(Sql<T>.Update, instance);
        }
        /// <summary>
        /// 获取当前实例集合
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="connection">对IDbConnection扩展</param>
        /// <returns>结果集</returns>
        public static IEnumerable<T> Get<T>(this IDbConnection connection)
        {
            return connection.GetCollection<T>(Sql<T>.Select);
        }
    }
}

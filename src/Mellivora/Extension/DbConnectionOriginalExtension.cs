using Mellivora.DynamicCache;
using System.Collections.Generic;
using System.Data;

namespace Mellivora
{
    public static class DbConnectionOriginalExtension
    {

        #region original
        /// <summary>
        /// 执行Sql语句并返回结果
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="connection">对IDbConnection进行扩展</param>
        /// <param name="commandText">Sql语句</param>
        /// <returns>结果集</returns>
        public static IEnumerable<T> GetCollection<T>(this IDbConnection connection, string commandText)
        {
            List<T> resultCollection = null;
            SqlDelegate<T>.GetReaderInstance instance_func = null;
            IDbCommand command = connection.CreateCommand();
            command.CommandText = commandText;
            IDataReader reader = null;
            bool CloseFlag = (connection.State == ConnectionState.Closed);
            try
            {
                if (CloseFlag) { connection.Open(); }
                reader = command.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess | CommandBehavior.SingleResult);
                CloseFlag = false;
                instance_func = SqlDynamicCache.GetReaderDelegate<T>(reader, commandText);
                resultCollection = new List<T>(reader.FieldCount);
                while (reader.Read())
                {
                    resultCollection.Add(instance_func(reader));
                }
                while (reader.NextResult()) { }
                reader.Dispose();
                reader = null;
                return resultCollection;
            }
            finally
            {
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        try { command.Cancel(); }
                        catch { }
                    }
                    reader.Dispose();
                }

                if (CloseFlag) connection.Close();
                command?.Dispose();
            }
        }
        /// <summary>
        /// 执行Sql语句并返回结果
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="connection">对IDbConnection进行扩展</param>
        /// <param name="commandText">Sql语句</param>
        /// <param name="startField">reader起始列</param>
        /// <param name="length">从起始列继续向后查询列的个数</param>
        /// <returns>结果集</returns>
        public static IEnumerable<T> GetCollection<T>(this IDbConnection connection, string commandText, int startField, int length)
        {
            List<T> resultCollection = null;
            SqlDelegate<T>.GetReaderInstance instance_func = null;
            IDbCommand command = connection.CreateCommand();
            command.CommandText = commandText;
            IDataReader reader = null;
            bool CloseFlag = (connection.State == ConnectionState.Closed);
            try
            {
                if (CloseFlag) { connection.Open(); }
                reader = command.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess | CommandBehavior.SingleResult);
                CloseFlag = false;
                instance_func = SqlDynamicCache.GetReaderDelegate<T>(reader, commandText, startField, length);
                resultCollection = new List<T>(reader.FieldCount);
                while (reader.Read())
                {
                    resultCollection.Add(instance_func(reader));
                }
                while (reader.NextResult()) { }
                reader.Dispose();
                reader = null;
                return resultCollection;
            }
            finally
            {
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        try { command.Cancel(); }
                        catch { }
                    }
                    reader.Dispose();
                }

                if (CloseFlag) connection.Close();
                command?.Dispose();
            }
        }

        /// <summary>
        /// 通过ExecuteNonQuery方式来操作数据
        /// </summary>
        /// <param name="connection">对IDbConnection扩展</param>
        /// <param name="commandText">SQL语句</param>
        /// <returns>影响行数</returns>
        public static int ExecuteNonQuery(this IDbConnection connection, string commandText)
        {
            IDbCommand command = connection.CreateCommand();
            command.CommandText = commandText;
            bool CloseFlag = (connection.State == ConnectionState.Closed);
            try
            {
                if (CloseFlag) { connection.Open(); }
                return command.ExecuteNonQuery();
            }
            finally
            {
                connection.Close();
                command?.Dispose();
            }
        }
        /// <summary>
        /// 通过ExecuteScalar方式来操作数据
        /// </summary>
        /// <param name="connection">对IDbConnection扩展</param>
        /// <param name="commandText">SQL语句</param>
        /// <returns>数据库返回结果</returns>
        public static object ExecuteScalar(this IDbConnection connection, string commandText)
        {
            IDbCommand command = connection.CreateCommand();
            command.CommandText = commandText;
            bool CloseFlag = (connection.State == ConnectionState.Closed);
            try
            {
                if (CloseFlag) { connection.Open(); }
                return command.ExecuteScalar();
            }
            finally
            {
                connection.Close();
                command?.Dispose();
            }
        }

        /// <summary>
        /// 通过ExecuteScalar方式来操作数据
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="connection">对IDbConnection扩展</param>
        /// <param name="commandText">SQL语句</param>
        /// <returns>数据库返回结果</returns>
        public static T ExecuteScalar<T>(this IDbConnection connection, string commandText)
        {
            IDbCommand command = connection.CreateCommand();
            command.CommandText = commandText;
            bool CloseFlag = (connection.State == ConnectionState.Closed);
            try
            {
                if (CloseFlag) { connection.Open(); }
                return (T)command.ExecuteScalar();
            }
            finally
            {
                connection.Close();
                command?.Dispose();
            }
        }
        #endregion
    }
}

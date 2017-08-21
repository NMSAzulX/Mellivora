using Mellivora.DynamicCache;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace Mellivora
{
    public static class DbConnectionByInstanceExtension
    {
        #region NormalQuery_ByInstance
        /// <summary>
        /// 通过多个实例查询满足条件的数据
        /// </summary>
        /// <typeparam name="T">需要返回数据的类型</typeparam>
        /// <param name="connection">对IDbConnection扩展</param>
        /// <param name="commandText">SQL语句</param>
        /// <param name="values">映射到SQL语句的实例数组</param>
        /// <returns>T类型集合</returns>
        public static IEnumerable<T> QueryByInstances<T>(this IDbConnection connection, string commandText, object[] values)
        {

            List<T> resultCollection = new List<T>();
            SqlDelegate<T>.GetReaderInstance instance_func = null;
            SqlDelegate<T>.GetCommandByInstance command_func = SqlDynamicCache.GetInstanceCommandDelegate<T>(commandText, values[0]);
            int i_length = values.Length;
            IDbCommand command = connection.CreateCommand();
            IDataReader reader = null;
            bool CloseFlag = (connection.State == ConnectionState.Closed);
            try
            {
                if (CloseFlag) { connection.Open(); }
                for (int i = 0; i < i_length; i += 1)
                {
                    command_func(ref command, values[i]);
                    reader = command.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess | CommandBehavior.SingleResult);
                    CloseFlag = false;
                    if (instance_func == null)
                    {
                        instance_func = SqlDynamicCache.GetReaderDelegate<T>(reader, commandText);
                    }
                    while (reader.Read())
                    {
                        T tNode = instance_func(reader);
                        resultCollection.Add(tNode);
                    }
                    while (reader.NextResult()) { }
                    reader.Dispose();
                    reader = null;
                }
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
        /// 通过实例，查询数据
        /// </summary>
        /// <typeparam name="T">需要返回数据类型</typeparam>
        /// <param name="connection">对IDbConnection扩展</param>
        /// <param name="commandText">SQL语句</param>
        /// <param name="value">映射到SQL语句的实例</param>
        /// <returns>T类型数据集合</returns>
        public static IEnumerable<T> QueryByInstance<T>(this IDbConnection connection, string commandText, object value)
        {
            //Stopwatch watch = new Stopwatch();
            List<T> resultCollection = null;
            SqlDelegate<T>.GetReaderInstance instance_func = null;
            SqlDelegate<T>.GetCommandByInstance command_func = SqlDynamicCache.GetInstanceCommandDelegate<T>(commandText, value);
            IDbCommand command = connection.CreateCommand();
            command_func(ref command, value);
            IDataReader reader = null;
            bool CloseFlag =( connection.State == ConnectionState.Closed );
            try
            {
                if (CloseFlag){ connection.Open();}
                reader = command.ExecuteReader(CommandBehavior.CloseConnection| CommandBehavior.SequentialAccess | CommandBehavior.SingleResult);
                CloseFlag = false;
                instance_func = SqlDynamicCache.GetReaderDelegate<T>(reader, commandText);
                resultCollection = new List<T>(reader.FieldCount);
                while (reader.Read())
                {
                    //watch.Restart();
                    //T t = instance_func(reader);
                    //watch.Stop();
                   // System.Console.WriteLine("执行Reader缓存：" + watch.Elapsed);
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
                        catch{}
                    }
                    reader.Dispose();
                }

                if (CloseFlag) connection.Close();
                command?.Dispose();
            }
        }
        /// <summary>
        /// 通过实例，查询一条数据
        /// </summary>
        /// <typeparam name="T">需要返回数据类型</typeparam>
        /// <param name="connection">对IDbConnection扩展</param>
        /// <param name="commandText">SQL语句</param>
        /// <param name="value">映射到SQL语句的实例</param>
        /// <returns>T类型数据</returns>
        public static T SingleByInstance<T>(this IDbConnection connection, string commandText, object value) where T : class
        {
            T node = null;
            SqlDelegate<T>.GetReaderInstance instance_func = null;
            SqlDelegate<T>.GetCommandByInstance command_func = SqlDynamicCache.GetInstanceCommandDelegate<T>(commandText, value);
            IDbCommand command = connection.CreateCommand();
            command_func(ref command, value);
            IDataReader reader = null;
            bool CloseFlag = (connection.State == ConnectionState.Closed);

            try
            {
                if (CloseFlag) { connection.Open(); }
                reader = command.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess | CommandBehavior.SingleResult);
                CloseFlag = false;
                instance_func = SqlDynamicCache.GetReaderDelegate<T>(reader, commandText);

                while (reader.Read())
                {
                    node = instance_func(reader);
                    reader.Dispose();
                    reader = null;
                    return node;
                }
                return null;
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
        #endregion

        #region ComplexQuery_ByInstance
        /// <summary>
        /// 通过多个实例以及reader的起始位置、长度来查询满足条件的数据
        /// </summary>
        /// <typeparam name="T">需要返回数据类型</typeparam>
        /// <param name="connection">对IDbConnection扩展</param>
        /// <param name="commandText">SQL语句</param>
        /// <param name="values">映射到SQL语句的实例数组</param>
        /// <param name="startField">reader起始列</param>
        /// <param name="length">从起始列继续向后查询列的个数</param>
        /// <returns>T类型数据集合</returns>
        public static IEnumerable<T> QueryByInstances<T>(this IDbConnection connection, string commandText, object[] values, int startField, int length)
        {
            List<T> resultCollection = new List<T>();
            int i_length = values.Length;
            IDbCommand command = connection.CreateCommand();

            SqlDelegate<T>.GetReaderInstance instance_func = null;
            SqlDelegate<T>.GetCommandByInstance command_func = SqlDynamicCache.GetInstanceCommandDelegate<T>(commandText, values[0]);

            bool CloseFlag = (connection.State == ConnectionState.Closed);
            IDataReader reader = null;
            try
            {
                if (CloseFlag) { connection.Open(); }
                for (int i = 0; i < i_length; i += 1)
                {
                    command_func(ref command, values[i]);
                    reader = command.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess | CommandBehavior.SingleResult);
                    CloseFlag = false;
                    if (instance_func == null) {
                        instance_func = SqlDynamicCache.GetReaderDelegate<T>(reader, commandText, startField, length);
                    }
                    while (reader.Read())
                    {
                        T tNode = instance_func(reader);
                        resultCollection.Add(tNode);
                    }
                    while (reader.NextResult()) { }
                    reader.Dispose();
                    reader = null;
                }
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
        /// 通过实例以及reader的起始位置、长度来查询满足条件的数据
        /// </summary>
        /// <typeparam name="T">需要返回数据类型</typeparam>
        /// <param name="connection">对IDbConnection扩展</param>
        /// <param name="commandText">SQL语句</param>
        /// <param name="value">映射到SQL语句的实例</param>
        /// <param name="startField">reader起始列</param>
        /// <param name="length">从起始列继续向后查询列的个数</param>
        /// <returns>T类型数据集合</returns>
        public static IEnumerable<T> QueryByInstance<T>(this IDbConnection connection, string commandText, object value, int startField, int length)
        {
            IDataReader reader = null;
            List<T> resultCollection = new List<T>();
            bool CloseFlag = (connection.State == ConnectionState.Closed);
            SqlDelegate<T>.GetReaderInstance instance_func = null;
            SqlDelegate<T>.GetCommandByInstance command_func = SqlDynamicCache.GetInstanceCommandDelegate<T>(commandText, value);
            IDbCommand command = connection.CreateCommand();
            command_func(ref command, value);
           

            try
            {
                if (CloseFlag) { connection.Open(); }
                reader = command.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess | CommandBehavior.SingleResult);
                CloseFlag = false;
                instance_func = SqlDynamicCache.GetReaderDelegate<T>(reader, commandText, startField, length);
               
                while (reader.Read())
                {
                    T tNode = instance_func(reader);
                    resultCollection.Add(tNode);
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
        /// 通过实例以及reader的起始位置、长度查询一条数据
        /// </summary>
        /// <typeparam name="T">需要返回数据类型</typeparam>
        /// <param name="connection">对IDbConnection扩展</param>
        /// <param name="commandText">SQL语句</param>
        /// <param name="value">映射到SQL语句的实例</param>
        /// <param name="startField">reader起始列</param>
        /// <param name="length">从起始列继续向后查询列的个数</param>
        /// <returns>T类型数据</returns>
        public static T SingleByInstance<T>(this IDbConnection connection, string commandText, object value, int startField, int length) where T : class
        {
            T node = null;
            IDataReader reader = null;
            SqlDelegate<T>.GetReaderInstance instance_func = null;
            SqlDelegate<T>.GetCommandByInstance command_func = SqlDynamicCache.GetInstanceCommandDelegate<T>(commandText, value);
            IDbCommand command = connection.CreateCommand();
            command_func(ref command, value);
            bool CloseFlag = (connection.State == ConnectionState.Closed);
            try
            {
                if (CloseFlag) { connection.Open(); }
                reader = command.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess | CommandBehavior.SingleResult);
                CloseFlag = false;
                instance_func = SqlDynamicCache.GetReaderDelegate<T>(reader, commandText, startField, length);
                while (reader.Read())
                {
                    node = instance_func(reader);
                    reader.Dispose();
                    reader = null;
                    return node;
                }
                while (reader.NextResult()) { }
                return null;
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
        #endregion

        #region ExecuteNonQuery_ByInstance
        /// <summary>
        /// 通过ExecuteNonQuery方式来操作数据
        /// </summary>
        /// <typeparam name="T">实例数据类型</typeparam>
        /// <param name="connection">对IDbConnection扩展</param>
        /// <param name="commandText">SQL语句</param>
        /// <param name="values">映射到SQL语句的实例数组</param>
        /// <returns>返回操作失误的实例数据集合</returns>
        public static List<T> ExecuteNonQueryByInstances<T>(this IDbConnection connection, string commandText, T[] values)
        {
            List<T> ResultList = new List<T>();
            SqlDelegate<T>.GetGenericCommand command_func = SqlDynamicCache.GetCommandGenericDelegate(commandText, values[0]);
            int i_length = values.Length;
            IDbCommand command = connection.CreateCommand();
            bool CloseFlag = (connection.State == ConnectionState.Closed);
            try
            {
                if (CloseFlag) { connection.Open(); }
                for (int i = 0; i < i_length; i += 1)
                {
                    command_func(ref command, values[i]);
                    int tempResult = command.ExecuteNonQuery();
                   
                    if (tempResult == 0)
                    {
                        ResultList.Add(values[i]);
                    }
                }
                return ResultList;
            }
            finally
            {
                connection.Close();
                command?.Dispose();
            }
        }
        /// <summary>
        /// 通过ExecuteNonQuery方式来操作数据
        /// </summary>
        /// <typeparam name="T">实例数据类型</typeparam>
        /// <param name="connection">对IDbConnection扩展</param>
        /// <param name="commandText">SQL语句</param>
        /// <param name="value">映射到SQL语句的实例</param>
        /// <returns>返回影响的行数</returns>
        public static int ExecuteNonQueryByInstance<T>(this IDbConnection connection, string commandText, T value)
        {
            SqlDelegate<T>.GetGenericCommand command_func = SqlDynamicCache.GetCommandGenericDelegate(commandText, value);
            IDbCommand command = connection.CreateCommand();
            command_func(ref command , value);
            bool CloseFlag = (connection.State == ConnectionState.Closed);
            try
            {
                if (CloseFlag) { connection.Open();}
                return command.ExecuteNonQuery();
            }
            finally 
            {

                connection.Close();
                command?.Dispose();
            }        
        }

        #endregion

        #region ExecuteScalar_ByInstance
        /// <summary>
        /// 通过ExecuteScalar方式来操作数据
        /// </summary>
        /// <typeparam name="T">实例数据类型</typeparam>
        /// <param name="connection">对IDbConnection扩展</param>
        /// <param name="commandText">SQL语句</param>
        /// <param name="values">映射到SQL语句的实例数组</param>
        /// <returns>返回结果集</returns>
        public static List<object> ExecuteScalarByInstances<T>(this IDbConnection connection, string commandText, T[] values)
        {
            List<object> ResultList = new List<object>();
            SqlDelegate<T>.GetGenericCommand command_func = SqlDynamicCache.GetCommandGenericDelegate(commandText, values[0]);
            IDbCommand command = connection.CreateCommand();
            int i_length = values.Length;
            bool CloseFlag = (connection.State == ConnectionState.Closed);

            try
            {
                if (CloseFlag) { connection.Open(); }
                for (int i = 0; i < i_length; i += 1)
                {
                    command_func(ref command, values[i]);
                   
                    ResultList.Add(command.ExecuteScalar());
                }
                return ResultList;
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
        /// <typeparam name="T">实例数据类型</typeparam>
        /// <param name="connection">对IDbConnection扩展</param>
        /// <param name="commandText">SQL语句</param>
        /// <param name="values">映射到SQL语句的实例</param>
        /// <returns>返回结果</returns>
        public static object ExecuteScalarByInstance<T>(this IDbConnection connection, string commandText, T value)
        {
            SqlDelegate<T>.GetGenericCommand command_func = SqlDynamicCache.GetCommandGenericDelegate(commandText, value);
            IDbCommand command = connection.CreateCommand();
            command_func(ref command, value);
            bool CloseFlag = (connection.State == ConnectionState.Closed);
            try
            {
                if (CloseFlag) { connection.Open();}
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
        /// <typeparam name="R">结果的类型</typeparam>
        /// <typeparam name="T">实例数据类型</typeparam>
        /// <param name="connection">对IDbConnection扩展</param>
        /// <param name="commandText">SQL语句</param>
        /// <param name="values">映射到SQL语句的实例数组</param>
        /// <returns>返回结果集</returns>
        public static List<R> ExecuteScalarByInstances<R, T>(this IDbConnection connection, string commandText, T[] values)
        {
            List<R> ResultList = new List<R>();
            SqlDelegate<T>.GetGenericCommand command_func = SqlDynamicCache.GetCommandGenericDelegate(commandText, values[0]);
            IDbCommand command = connection.CreateCommand();
            int i_length = values.Length;
            bool CloseFlag = (connection.State == ConnectionState.Closed);
            try
            {
                if (CloseFlag) { connection.Open();  }
                for (int i = 0; i < i_length; i += 1)
                {
                    command_func(ref command, values[i]);
                    ResultList.Add((R)(command.ExecuteScalar()));
                    command.Dispose();
                }
                return ResultList;
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
        /// <typeparam name="R">结果的类型</typeparam>
        /// <typeparam name="T">实例数据类型</typeparam>
        /// <param name="connection">对IDbConnection扩展</param>
        /// <param name="commandText">SQL语句</param>
        /// <param name="values">映射到SQL语句的实例</param>
        /// <returns>返回结果</returns>
        public static R ExecuteScalarByInstance<R, T>(this IDbConnection connection, string commandText, T value)
        {
            SqlDelegate<T>.GetGenericCommand command_func = SqlDynamicCache.GetCommandGenericDelegate(commandText, value);
            IDbCommand command = connection.CreateCommand();
            command_func(ref command, value);
            bool CloseFlag = (connection.State == ConnectionState.Closed);
            try
            {
                if (CloseFlag) { connection.Open(); }
                return (R)command.ExecuteScalar();
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

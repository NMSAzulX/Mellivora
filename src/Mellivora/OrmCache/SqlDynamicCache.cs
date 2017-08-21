using Natasha;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Vasily;
using Vasily.Utils;

namespace Mellivora.DynamicCache
{
    public static class SqlDynamicCache
    {
        private static ConcurrentDictionary<Type, DbType> SqlTypes;
        private static Regex ParameterRegex;
        static SqlDynamicCache()
        {
            ParameterRegex = new Regex(@"@(\w*)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            SqlTypes = new ConcurrentDictionary<Type, DbType>
            {
                [typeof(byte)] = DbType.Byte,
                [typeof(sbyte)] = DbType.SByte,
                [typeof(short)] = DbType.Int16,
                [typeof(ushort)] = DbType.UInt16,
                [typeof(int)] = DbType.Int32,
                [typeof(uint)] = DbType.UInt32,
                [typeof(long)] = DbType.Int64,
                [typeof(ulong)] = DbType.UInt64,
                [typeof(float)] = DbType.Single,
                [typeof(double)] = DbType.Double,
                [typeof(decimal)] = DbType.Decimal,
                [typeof(bool)] = DbType.Boolean,
                [typeof(string)] = DbType.String,
                [typeof(char)] = DbType.StringFixedLength,
                [typeof(Guid)] = DbType.Guid,
                [typeof(DateTime)] = DbType.DateTime,
                [typeof(DateTimeOffset)] = DbType.DateTimeOffset,
                [typeof(TimeSpan)] = DbType.Time,
                [typeof(byte[])] = DbType.Binary,
                [typeof(byte?)] = DbType.Byte,
                [typeof(sbyte?)] = DbType.SByte,
                [typeof(short?)] = DbType.Int16,
                [typeof(ushort?)] = DbType.UInt16,
                [typeof(int?)] = DbType.Int32,
                [typeof(uint?)] = DbType.UInt32,
                [typeof(long?)] = DbType.Int64,
                [typeof(ulong?)] = DbType.UInt64,
                [typeof(float?)] = DbType.Single,
                [typeof(double?)] = DbType.Double,
                [typeof(decimal?)] = DbType.Decimal,
                [typeof(bool?)] = DbType.Boolean,
                [typeof(char?)] = DbType.StringFixedLength,
                [typeof(Guid?)] = DbType.Guid,
                [typeof(DateTime?)] = DbType.DateTime,
                [typeof(DateTimeOffset?)] = DbType.DateTimeOffset,
                [typeof(TimeSpan?)] = DbType.Time,
                [typeof(object)] = DbType.Object
            };
        }
        /// <summary>
        /// 获取根据返回类型构建Command的动态缓存方法
        /// </summary>
        /// <typeparam name="T">函数返回类型</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="value">实例</param>
        /// <returns>动态缓存方法</returns>
        [System.Security.SecurityCritical]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SqlDelegate<T>.GetGenericCommand GetCommandGenericDelegate<T>(string sql, T value)
        {
            if (!SqlDelegate<T>.CommandGenericCache.ContainsKey(sql))
            {
                SqlDelegate<T>.CommandGenericCache[sql] = GetEmitCommandGenericCache(sql, value);
            }
            return SqlDelegate<T>.CommandGenericCache[sql];
        }

        /// <summary>
        /// 获取通过实例构建Command的动态缓存方法
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="value">实例</param>
        /// <returns>动态缓存方法</returns>
        [System.Security.SecurityCritical]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SqlDelegate<T>.GetCommandByInstance GetInstanceCommandDelegate<T>(string sql, object value)
        {
            int typeCode = value.GetType().GetHashCode();
            if (!SqlDelegate<T>.CommandInstancesCache.ContainsKey(typeCode))
            {
                SqlDelegate<T>.CommandInstancesCache[typeCode] = new ConcurrentDictionary<string, SqlDelegate<T>.GetCommandByInstance>() { };
                SqlDelegate<T>.CommandInstancesCache[typeCode][sql] = GetEmitCommandCache<T>(sql, value);
            }
            else if (!SqlDelegate<T>.CommandInstancesCache[typeCode].ContainsKey(sql))
            {
                SqlDelegate<T>.CommandInstancesCache[typeCode][sql] = GetEmitCommandCache<T>(sql, value);
            }
            return SqlDelegate<T>.CommandInstancesCache[typeCode][sql];
        }

        /// <summary>
        /// 获取通过动态对象数组构建Command的动态缓存方法
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="value">对象数组</param>
        /// <returns>动态缓存方法</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SqlDelegate<T>.GetCommandByObject GetObjectsCommandDelegate<T>(string sql, object[] value)
        {
            if (!SqlDelegate<T>.CommandObjectsCache.ContainsKey(sql))
            {
                SqlDelegate<T>.CommandObjectsCache[sql] = GetEmitCommandCache<T>(sql, value);
            }
            return SqlDelegate<T>.CommandObjectsCache[sql];
        }

        /// <summary>
        /// 获取复杂Reader映射缓存方法
        /// </summary>
        /// <typeparam name="T">返回的类型</typeparam>
        /// <param name="reader">数据库返回的DataReader</param>
        /// <param name="sql">SQL语句</param>
        /// <param name="startField">reader起始列</param>
        /// <param name="length">从起始列继续向后查询列的个数</param>
        /// <returns>动态缓存方法</returns>
        [System.Security.SecurityCritical]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SqlDelegate<T>.GetReaderInstance GetReaderDelegate<T>(IDataReader reader, string sql, int firstColumn, int length)
        {
            Tuple<string, int, int> temp = Tuple.Create(sql, firstColumn, length);

            if (!SqlDelegate<T>.ComplexReaderCache.ContainsKey(temp))
            {
                SqlDelegate<T>.ComplexReaderCache[temp] = GetEmitReaderCache<T>(reader, firstColumn, length);
            }
            return SqlDelegate<T>.ComplexReaderCache[temp];
        }
        /// <summary>
        /// 获取处理单个Reader结果的映射缓存方法
        /// </summary>
        /// <typeparam name="T">返回的类型</typeparam>
        /// <param name="reader">数据库返回的DataReader</param>
        /// <param name="sql">SQL语句</param>
        /// <returns>动态缓存方法</returns>
        [System.Security.SecurityCritical]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SqlDelegate<T>.GetReaderInstance GetReaderDelegate<T>(IDataReader reader, string sql)
        {
            if (!SqlDelegate<T>.SingleReaderCache.ContainsKey(sql))
            {
                SqlDelegate<T>.SingleReaderCache[sql] = GetEmitReaderCache<T>(reader, 0, reader.FieldCount);
            }
            return SqlDelegate<T>.SingleReaderCache[sql];
        }

        /// <summary>
        /// 获取Reader映射缓存方法
        /// </summary>
        /// <typeparam name="T">返回的类型</typeparam>
        /// <param name="reader">数据库返回的DataReader</param>
        /// <param name="sql">SQL语句</param>
        /// <returns>动态缓存方法</returns>
        private static SqlDelegate<T>.GetReaderInstance GetEmitReaderCache<T>(IDataReader reader, int startIndex, int length)
        {
            Type returnType = typeof(T);
            if (!Cache.SqlCache.ContainsKey(returnType))
            {
                ModelAnalyser.Initialization(returnType);
            }
            SqlModel sqlModel = Cache.SqlCache[returnType];

            Delegate dynamicMethod = EHandler.CreateMethod<IDataReader, T>((il) =>
            {
                EMethod dataHandler = typeof(IDataRecord);
                EVar parameterVar = EVar.CreateVarFromParameter<IDataRecord>(0);

                if (returnType == typeof(object) || returnType == typeof(string) || returnType == typeof(byte[]) || (returnType.IsValueType && returnType.IsPrimitive) || il.IsNullable(returnType))
                {

                    if (returnType.IsValueType && returnType.IsPrimitive)
                    {
                        LoadStrongTypeValue(returnType, parameterVar, startIndex);
                    }
                    else
                    {
                        EJudge.If(() => { EMethod.Load(parameterVar).ExecuteMethod<int>("IsDBNull", startIndex); })(() =>
                        {
                            if (il.IsNullable(returnType))
                            {
                                EModel model = EModel.CreateModel(returnType).UseDefaultConstructor();
                                model.Load();
                            }
                            else
                            {
                                ENull.LoadNull();
                            }

                        }).Else(() =>
                        {
                            LoadStrongTypeValue(returnType, parameterVar, startIndex);
                        });
                    }
                }
                else
                {
                    EModel model = EModel.CreateModel<T>().UseDefaultConstructor();

                    for (int i = startIndex; i < startIndex + length; i += 1)
                    {
                        string tempName = sqlModel.GetRealName(reader.GetName(i));
                        Type type = null;
                        if (!model.Struction.Properties.ContainsKey(tempName) && !model.Struction.Fields.ContainsKey(tempName))
                        {
                            continue;
                        }
                        else
                        {
                            type = sqlModel.Struction.ModelTypeCache[tempName];
                        }
                        if (type.IsValueType && type.IsPrimitive)
                        {
                            model.Set(tempName, () =>
                            {
                                LoadStrongTypeValue(type, parameterVar, i);
                            });
                        }
                        else
                        {
                            EJudge.IfTrue(() => { EMethod.Load(parameterVar).ExecuteMethod<int>("IsDBNull", i); })(() =>
                             {
                                 model.Set(tempName, () =>
                                 {
                                     LoadStrongTypeValue(type, parameterVar, i);
                                 });
                             });
                        }
                    }
                    model.Load();
                }
            }).Compile(typeof(SqlDelegate<T>.GetReaderInstance));

            return (SqlDelegate<T>.GetReaderInstance)dynamicMethod;
        }
        private static bool LoadStrongTypeValue(Type type, EVar model, int Index)
        {
            string fastMethodName = "GetValue";
            if (type == typeof(string))
            {
                fastMethodName = "GetString";

            }
            else if (type == typeof(int) || type == typeof(int?))
            {
                fastMethodName = "GetInt32";
            }
            else if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                fastMethodName = "GetDateTime";
            }
            else if (type == typeof(double) || type == typeof(double?))
            {
                fastMethodName = "GetDouble";
            }
            else if (type == typeof(bool) || type == typeof(double?))
            {
                fastMethodName = "GetBoolean";
            }
            else if (type == typeof(byte) || type == typeof(byte?))
            {
                fastMethodName = "GetByte";
            }
            else if (type == typeof(long) || type == typeof(long?))
            {
                fastMethodName = "GetInt64";
            }
            else if (type == typeof(Guid) || type == typeof(Guid?))
            {
                fastMethodName = "GetGuid";
            }
            else if (type == typeof(float) || type == typeof(float?))
            {
                fastMethodName = "GetFloat";
            }
            else if (type == typeof(char) || type == typeof(char?))
            {
                fastMethodName = "GetChar";
            }
            else if (type == typeof(short) || type == typeof(short?))
            {
                fastMethodName = "GetInt16";
            }
            else if (type == typeof(decimal) || type == typeof(decimal?))
            {
                fastMethodName = "GetDecimal";
            }
            else if (type == typeof(byte[]))
            {
                fastMethodName = "GetSqlBytes";
            }

            EMethod.Load(model).ExecuteMethod<int>(fastMethodName, Index);

            if (fastMethodName != "GetValue")
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 使用Natasha根据实例信息生成Command高速构建缓存，由返回类型以及SQL字符串决定缓存存储
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="value">实例</param>
        /// <returns>动态方法</returns>
        private static SqlDelegate<T>.GetCommandByInstance GetEmitCommandCache<T>(string sql, object value)
        {
            Type returnType = typeof(T);
            if (!Cache.SqlCache.ContainsKey(returnType))
            {
                ModelAnalyser.Initialization(returnType);
            }

            Delegate newMethod = EHandler.CreateMethod<ERef<IDbCommand>, object, ENull>((il) =>
            {
                EModel idbCommand = EModel.CreateModelFromParameter<IDbCommand>(0);
                idbCommand.UseRef();
                idbCommand.Set("CommandText", sql);

                MatchCollection collection = ParameterRegex.Matches(sql);
                int i_length = collection.Count;
                if (i_length > 0)
                {
                    Type type = value.GetType();
                    EModel valueArg = EModel.CreateModelFromParameter<T>(1);
                    EModel valueDate = EModel.CreateModel(type);
                    valueDate.Store(valueArg.InStackAndUnPacket);

                    if (!Cache.SqlCache.ContainsKey(type))
                    {
                        ModelAnalyser.Initialization(type);
                    }
                    EModel copyParameters = idbCommand.Load("Parameters");
                    IDictionary<string, Type> typeCache = Cache.StructionCache[type].ModelTypeCache;
                    for (int i = 0; i < i_length; i += 1)
                    {
                        string memberName = collection[i].Groups[1].Value;
                        Type tempType = typeCache[memberName];

                        copyParameters.Dup();                                                                       //+ Stack:[IDbCommand.Parameters] [IDbCommand.Parameters]
                        EModel copyParameter = EMethod.Load(idbCommand).ExecuteMethod("CreateParameter").Dup();    //+ Stack:[IDbCommand.Parameters] [IDbCommand.Parameters] [IDbParameter] [IDbParameter]
                        copyParameter.Set("ParameterName", "@".Append(memberName));                                 //+ Stack:[IDbCommand.Parameters] [IDbCommand.Parameters] [IDbParameter]                        Deal:([IDbParameter].ParameterName=@XXX)
                        copyParameter.Dup().Set("DbType", (int)SqlTypes[tempType]);                                 //+ Stack:[IDbCommand.Parameters] [IDbCommand.Parameters] [IDbParameter]                        Deal:([IDbParameter].DbType=XXX)
                        copyParameter.Dup().Set("Value", () =>
                        {
                            if (il.IsNullable(tempType))
                            {
                                EJudge.If(valueDate.DLoad(memberName).DLoadValue("HasValue").DelayAction)(() =>
                                {
                                    valueDate.Load(memberName).LoadValue("Value").Packet();

                                }).Else(() =>
                                {
                                    EDBNull.LoadValue();
                                });
                            }
                            else if (tempType.IsValueType)
                            {
                                valueDate.Load(memberName).Packet();
                            }
                            else
                            {
                                EJudge.If(ENull.IsNull(valueDate.DLoad(memberName).DelayAction))(() =>
                                {
                                    EDBNull.LoadValue();

                                }).Else(() =>
                                {
                                    valueDate.Load(memberName).Packet();
                                });
                            }
                        });                                                 //+ Stack:[IDbCommand.Parameters] [IDbCommand.Parameters] [IDbParameter]                        Deal:([IDbParameter].Value=XXX)
                        EMethod.Load<IList>().ExecuteMethod<object>("Add").Pop();                                   //+ Stack:[IDbCommand.Parameters]                                                               Deal:Add([IDbCommand.Parameters] [IDbParameter])  
                    }
                    copyParameters.Pop();
                }
            }).Compile(typeof(SqlDelegate<T>.GetCommandByInstance));

            return (SqlDelegate<T>.GetCommandByInstance)newMethod;
        }
        /// <summary>
        /// 使用Natasha根据参数数组信息生成Command高速构建缓存
        /// </summary>
        /// <typeparam name="T">实际执行函数返回的类型</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="values">object参数数组</param>
        /// <returns>动态方法</returns>
        private static SqlDelegate<T>.GetCommandByObject GetEmitCommandCache<T>(string sql, object[] values)
        {
            Type returnType = typeof(T);
            if (!Cache.SqlCache.ContainsKey(returnType))
            {
                ModelAnalyser.Initialization(returnType);
            }

            Delegate newMethod = EHandler.CreateMethod<ERef<IDbCommand>, object[], ENull>((il) =>
            {
                EModel idbCommand = EModel.CreateModelFromParameter<IDbCommand>(0);
                idbCommand.UseRef();
                idbCommand.Set("CommandText", sql);

                EArray arrayArg = EArray.CreateArrayFromParameter(1, typeof(object));
                EModel copyParameters = idbCommand.Load("Parameters");

                MatchCollection collection = ParameterRegex.Matches(sql);
                int length = collection.Count;
                for (int i = 0; i < length; i += 1)
                {
                    Type type = values[i].GetType();

                    string memberName = collection[i].Groups[1].Value;
                    copyParameters.Dup();
                    EModel copyParameter = EMethod.Load(idbCommand).ExecuteMethod("CreateParameter").Dup();
                    copyParameter.Set("ParameterName", "@".Append(memberName));
                    copyParameter.Dup().Set("DbType", (int)SqlTypes[type]);

                    if (type.IsPrimitive)
                    {
                        copyParameter.Dup().Set("Value", () => { arrayArg.LoadArray(i); });
                    }
                    else
                    {
                        EJudge.If(ENull.IsNull(() => { arrayArg.LoadArray(i); }))(() =>
                        {
                            copyParameter.Dup().Set("Value", EDBNull.LoadValue);

                        }).Else(() =>
                        {
                            copyParameter.Dup().Set("Value", () => { arrayArg.LoadArray(i); });
                        });
                    }
                    EMethod.Load<IList>().ExecuteMethod<object>("Add").Pop();
                }
                copyParameters.Pop();
            }).Compile(typeof(SqlDelegate<T>.GetCommandByObject));

            return (SqlDelegate<T>.GetCommandByObject)newMethod;
        }
        /// <summary>
        /// 使用Natasha根据实例信息生成Command高速构建缓存，由参数类型以及SQL字符串决定缓存存储
        /// </summary>
        /// <typeparam name="T">Command缓存方法中需要传入实例类型</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="value">实例</param>
        /// <returns>动态方法</returns>
        private static SqlDelegate<T>.GetGenericCommand GetEmitCommandGenericCache<T>(string sql, T value)
        {
            Type returnType = typeof(T);
            if (!Cache.SqlCache.ContainsKey(returnType))
            {
                ModelAnalyser.Initialization(returnType);
            }

            Delegate newMethod = EHandler.CreateMethod<ERef<IDbCommand>, T, ENull>((il) =>
            {
                EModel idbCommand = EModel.CreateModelFromParameter<IDbCommand>(0);
                idbCommand.UseRef();
                idbCommand.Set("CommandText", sql);

                MatchCollection collection = ParameterRegex.Matches(sql);
                int i_length = collection.Count;

                if (i_length > 0)
                {
                    Type type = value.GetType();
                    EModel valueDate = EModel.CreateModelFromParameter<T>(1);

                    if (!Cache.SqlCache.ContainsKey(type))
                    {
                        ModelAnalyser.Initialization(type);
                    }
                    EModel copyParameters = idbCommand.Load("Parameters");
                    IDictionary<string, Type> typeCache = Cache.StructionCache[type].ModelTypeCache;
                    for (int i = 0; i < i_length; i += 1)
                    {
                        string memberName = collection[i].Groups[1].Value;
                        Type tempType = typeCache[memberName];

                        copyParameters.Dup();                                                                       //+ Stack:[IDbCommand.Parameters] [IDbCommand.Parameters]
                        EModel copyParameter = EMethod.Load(idbCommand).ExecuteMethod("CreateParameter").Dup();     //+ Stack:[IDbCommand.Parameters] [IDbCommand.Parameters] [IDbParameter] [IDbParameter]
                        copyParameter.Set("ParameterName", "@".Append(memberName));                                 //+ Stack:[IDbCommand.Parameters] [IDbCommand.Parameters] [IDbParameter]                        Deal:([IDbParameter].ParameterName=@XXX)
                        copyParameter.Dup().Set("DbType", (int)SqlTypes[tempType]);                                 //+ Stack:[IDbCommand.Parameters] [IDbCommand.Parameters] [IDbParameter]                        Deal:([IDbParameter].DbType=XXX)
                        copyParameter.Dup().Set("Value", () =>
                        {
                            if (il.IsNullable(tempType))
                            {
                                EJudge.If(valueDate.DLoad(memberName).DLoadValue("HasValue").DelayAction)(() =>
                                {
                                    valueDate.Load(memberName).LoadValue("Value").Packet();

                                }).Else(() =>
                                {
                                    EDBNull.LoadValue();
                                });
                            }
                            else if (tempType.IsValueType)
                            {
                                valueDate.Load(memberName).Packet();
                            }
                            else
                            {
                                EJudge.If(ENull.IsNull(valueDate.DLoad(memberName).DelayAction))(() =>
                                {
                                    EDBNull.LoadValue();

                                }).Else(() =>
                                {
                                    valueDate.Load(memberName).Packet();
                                });
                            }
                        });                                                 //+ Stack:[IDbCommand.Parameters] [IDbCommand.Parameters] [IDbParameter]                        Deal:([IDbParameter].Value=XXX)
                        EMethod.Load<IList>().ExecuteMethod<object>("Add").Pop();                                   //+ Stack:[IDbCommand.Parameters]                                                               Deal:Add([IDbCommand.Parameters] [IDbParameter])  
                    }
                    copyParameters.Pop();
                }
            }).Compile(typeof(SqlDelegate<T>.GetGenericCommand));

            return (SqlDelegate<T>.GetGenericCommand)newMethod;
        }
    }
}

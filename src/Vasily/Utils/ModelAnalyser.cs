using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vasily.Model;
using Vasily.Utils;

namespace Vasily.Utils
{
    public class ModelAnalyser
    {
        public static void Initialization(Type type)
        {

            //创建数据库查询缓存实例
            SqlModel model = new SqlModel(type);

            GetPrimaryKey(model);
            GetTableName(model, type);

            if (model.Table != null)
            {
                GetCIN(model);
                GetAL(model);
                GetSqlString(model);
                Cache.SqlCache[type] = model;
            }
            SqlAnalyser.Initialize(model);
        }
        private static void GetPrimaryKey(SqlModel model)
        {
            int i = 0;
            int length = model.Struction.Members.Count;
            MemberInfo[] members = model.Struction.Members.ToArray();
            while (i < length)
            {
                if (model.PrimaryKey == null || model.PrimaryKey == string.Empty)
                {
                    PrimaryKeyAttribute temp_PrimaryKeyAttributer = members[i].GetCustomAttribute<PrimaryKeyAttribute>(true);
                    if (temp_PrimaryKeyAttributer != null)
                    {
                        model.PrimaryKey = members[i].Name;
                        model.IsMaunally = temp_PrimaryKeyAttributer.IsManually;
                        break;
                    }
                }
                i += 1;
            }
        }
        private static void GetTableName(SqlModel model, Type type)
        {
            TableAttribute temp_TableAttribute = type.GetCustomAttribute<TableAttribute>(true);
            if (temp_TableAttribute != null)
            {
                model.Table = temp_TableAttribute.Name;
            }

        }
        private static void GetCIN(SqlModel model)
        {
            int i = 0;
            int length = model.Struction.Members.Count;
            MemberInfo[] members = model.Struction.Members.ToArray();
            while (i < length)
            {
                string temp_Name = members[i].Name;
                ColumnAttribute temp_ColumnAttribute = members[i].GetCustomAttribute<ColumnAttribute>();
                IgnoreAttribute temp_IgnoreAttribute = members[i].GetCustomAttribute<IgnoreAttribute>();
                RepeateAttribute temp_RepeateAttribute = members[i].GetCustomAttribute<RepeateAttribute>();
                if (temp_ColumnAttribute != null)
                {
                    model.ColumnToRealMap[temp_ColumnAttribute.Name] = members[i].Name;
                    model.RealToColumnMap[members[i].Name] = temp_ColumnAttribute.Name;
                }
                if (temp_IgnoreAttribute != null)
                {
                    model.IgnoreMembers.Add(temp_Name);
                }
                if (temp_RepeateAttribute != null)
                {
                    model.RepeateMembers.Add(temp_Name);
                }
                i += 1;
            }
        }

        private static void GetAL(SqlModel model) {
            Dictionary<string, List<string>> UpdateDict = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> SelectDict = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> RepeateDict = new Dictionary<string, List<string>>();
            Dictionary<string, List<ALStruct>> Where = new Dictionary<string, List<ALStruct>>();
            MemberInfo[] infos = model.Struction.Members.ToArray();
            for (int i = 0; i < infos.Length; i+=1)
            {
                MemberInfo info = infos[i];
                GetLogicalList<UpdateAttribute>(info, UpdateDict);
                GetLogicalList<SelectAttribute>(info, SelectDict);
                GetLogicalList<RepeateAttribute>(info, RepeateDict);
                GetLogicalList<AndGtrAttribute>(info, Where);
                GetLogicalList<OrGtrAttribute>(info, Where);
                GetLogicalList<AndGeqAttribute>(info, Where);
                GetLogicalList<OrGeqAttribute>(info, Where);
                GetLogicalList<AndEquAttribute>(info, Where);
                GetLogicalList<OrEquAttribute>(info, Where);
                GetLogicalList<AndNeqAttribute>(info, Where);
                GetLogicalList<OrNeqAttribute>(info, Where);
                GetLogicalList<AndLssAttribute>(info, Where);
                GetLogicalList<OrLssAttribute>(info, Where);
                GetLogicalList<AndLeqAttribute>(info, Where);
                GetLogicalList<OrLeqAttribute>(info, Where);

            }
            SqlMaker maker = new SqlMaker(model);


            foreach (var item in UpdateDict)
            {
                string result = maker.GetUpdate(item.Value.ToArray());
                string condition = string.Empty;
                if (Where.ContainsKey(item.Key))
                {
                    List<ALStruct> models = Where[item.Key];
                    foreach (var alItem in models)
                    {
                        condition = condition.Append(maker.GetOperator(alItem.OChar, alItem.CChar, alItem.Data));
                    }
                    switch (models[0].OChar)
                    {
                        case OperatorChar.AND:
                            condition = condition.Remove(0, 4);
                            break;
                        case OperatorChar.OR:
                            condition = condition.Remove(0, 3);
                            break;
                        default:
                            break;
                    }
                }
                result = result.Append(condition);
                model.ALMap[item.Key] = result;
            }
            foreach (var item in SelectDict)
            {
                string result = maker.GetSelect(item.Value.ToArray());
                string condition = string.Empty;
                if (Where.ContainsKey(item.Key))
                {
                    List<ALStruct> models = Where[item.Key];
                    foreach (var alItem in models)
                    {
                        condition = condition.Append(maker.GetOperator(alItem.OChar, alItem.CChar, alItem.Data));
                    }
                    switch (models[0].OChar)
                    {
                        case OperatorChar.AND:
                            condition = condition.Remove(0, 4);
                            break;
                        case OperatorChar.OR:
                            condition = condition.Remove(0, 3);
                            break;
                        default:
                            break;
                    }
                }
                result = result.Append(condition);
                model.ALMap[item.Key] = result;
            }
            foreach (var item in RepeateDict)
            {
                model.ALMap[item.Key] = maker.GetRepeate(item.Value.ToArray());
            }
        }
        private static void GetSqlString(SqlModel model)
        {
            string insertFields = string.Empty;
            string insertValues = string.Empty;
            string updateFieldAndValues = string.Empty;
            string checkRepeate = string.Empty;
            string getModelId = string.Empty;

            int i = 0;
            int length = model.Struction.Members.Count;
            MemberInfo[] members = model.Struction.Members.ToArray();
            while (i < length)
            {
                string memberName = members[i].Name;
                string mapMemberName = model.GetColumnName(memberName);
                if (!model.IgnoreMembers.Contains(memberName))
                {
                    insertFields = NMSString.Append(insertFields, ",[", mapMemberName, "]");
                    insertValues = NMSString.Append(insertValues, ",@", memberName);

                    updateFieldAndValues = NMSString.Append(updateFieldAndValues, ",[", mapMemberName, "]=@", memberName);
                    getModelId = NMSString.Append(getModelId, " AND [", mapMemberName, "]=@", memberName);

                    if (model.RepeateMembers.Contains(memberName))
                    {
                        checkRepeate = NMSString.Append(checkRepeate, " AND [", mapMemberName, "]=@", memberName);
                    }
                }
                i += 1;
            }

            insertFields = insertFields.Remove(0, 1);
            insertValues = insertValues.Remove(0, 1);
            updateFieldAndValues = updateFieldAndValues.Remove(0, 1);
            if (checkRepeate.Length > 5)
            {
                checkRepeate = checkRepeate.Remove(0, 5);
            }
            getModelId = getModelId.Remove(0, 4);


            string byPrimaryKey = string.Empty;

            if (model.PrimaryKey != null)
            {
                string realPrimaryKey = model.GetColumnName(model.PrimaryKey);
                byPrimaryKey = NMSString.Append("[", realPrimaryKey, "]=@", model.PrimaryKey);
                if (!model.IsMaunally)
                {
                    insertFields = insertFields.Replace(NMSString.Append("[", realPrimaryKey, "]"), "").Replace(",,", ",");
                    insertValues = insertValues.Replace(NMSString.Append("@", model.PrimaryKey), "").Replace(",,", ",");
                    updateFieldAndValues = updateFieldAndValues.Replace(byPrimaryKey, "").Replace(",,", ",");
                    checkRepeate = checkRepeate.Replace(byPrimaryKey, "").Replace("AND  AND", "AND");
                    getModelId = getModelId.Replace(byPrimaryKey, "").Replace("AND  AND", "AND");
                }
                model.GetPrimaryKey = NMSString.Append("SELECT [", realPrimaryKey, "] FROM [", model.Table, "] WHERE", getModelId);
            }

            model.Update = NMSString.Append("UPDATE [", model.Table, "] SET ", updateFieldAndValues);
            model.Select = NMSString.Append("SELECT * FROM [", model.Table, "]");
            model.Delete = NMSString.Append("DELETE [", model.Table, "]");
            model.Insert = NMSString.Append("INSERT INTO [", model.Table, "] (", insertFields, ") VALUES (", insertValues, ")");

            model.ConditionDelete = model.Delete.Append(" WHERE ");
            model.ConditionSelect = model.Select.Append(" WHERE ");
            model.ConditionUpdate = model.Update.Append(" WHERE ");

            model.SelectAll = model.Select;
            if (model.PrimaryKey != null)
            {
                model.Update = model.ConditionUpdate.Append(byPrimaryKey);
                model.Select = model.ConditionSelect.Append(byPrimaryKey);
                model.Delete = model.ConditionDelete.Append(byPrimaryKey);
            }

            model.CheckRepeate = NMSString.Append("SELECT COUNT(*) FROM [", model.Table, "] WHERE ", checkRepeate);
        }

        private static void GetLogicalList<T>(MemberInfo info, Dictionary<string, List<string>> dict) where T:Attribute, IAttributesLogicalData
        {
            IAttributesLogicalData instance = info.GetCustomAttribute<T>(true);
            if (instance==null)
            {
                return;
            }
            string[] keys = instance.Keys;
            for (int i = 0; i < keys.Length; i += 1)
            {
                if (!dict.ContainsKey(keys[i]))
                {
                    dict[keys[i]] = new List<string>();

                }
                dict[keys[i]].Add(info.Name);
            }
        }
        private static void GetLogicalList<T>(MemberInfo info, Dictionary<string, List<ALStruct>> dict) where T : Attribute, IAttributesLogicalOperator
        {
            IAttributesLogicalOperator instance = info.GetCustomAttribute<T>(true);
            if (instance == null)
            {
                return;
            }
            string[] keys = instance.Keys;
            for (int i = 0; i < keys.Length; i += 1)
            {
                if (!dict.ContainsKey(keys[i]))
                {
                    dict[keys[i]] = new List<ALStruct>();
                }
                dict[keys[i]].Add(new ALStruct() { CChar = instance.CChar, OChar = instance.OChar, Data = info.Name });
            }
        }
    }
}

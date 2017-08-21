using Vasily.Model;

namespace Vasily.Utils
{
    public class SqlMaker
    {
        public SqlModel Model;
        public SqlMaker(SqlModel model)
        {
            Model = model;
        }
        
        public string GetUpdate(string[] data)
        {
            string update = string.Empty;
            for (int i = 0; i < data.Length; i+=1)
            {
                update = NMSString.Append(update, ",[", Model.GetColumnName(data[i]), "]=@", data[i]);
            }
            update = update.Remove(0, 1);
            return NMSString.Append("UPDATE [", Model.Table, "] SET ", update," WHERE"); ;
        }
        public string GetDelete()
        {
            return NMSString.Append("DELETE [", Model.Table, " WHERE"); ;
        }
        public string GetInsert(string[] data)
        {
            string insertFields = string.Empty;
            string insertValues = string.Empty;

            for (int i = 0; i < data.Length; i += 1)
            {
                insertFields = NMSString.Append(insertFields, ",[", Model.GetColumnName(data[i]), "]");
                insertValues = NMSString.Append(insertValues, ",@", data[i]);
            }
            insertFields = insertFields.Remove(0, 1);
            insertValues = insertValues.Remove(0, 1);
            return NMSString.Append("", Model.Table, "] (", insertFields, ") VALUES (", insertValues, ")");
        }
        public string GetRepeate(string[] data)
        {
            string where = string.Empty;
            for (int i = 0; i < data.Length; i += 1)
            {
                where = NMSString.Append(" AND [", Model.GetColumnName(data[i]), "]=@", data[i]);
            }
            where = where.Remove(0, 4);
            return NMSString.Append("SELECT COUNT(*) FROM [", Model.Table, "] WHERE", where);
        }
        public string GetSelect(string[] data)
        {
            string select = string.Empty;
            for (int i = 0; i < data.Length; i += 1)
            {
                select = NMSString.Append(select,",[", Model.GetColumnName(data[i]), "]");
            }
            select = select.Remove(0, 1);
            return NMSString.Append("SELECT ", select, " FROM [", Model.Table, "] WHERE");
        }
        public string GetOperator(OperatorChar ochar,OperatorChar cchar,string data)
        {
            string o = string.Empty;
            switch (ochar)
            {
                case OperatorChar.AND:
                    o = "AND";
                    break;
                case OperatorChar.OR:
                    o = "OR";
                    break;
                default:
                    break;
            }
            string c = string.Empty;
            switch (cchar)
            {
                case OperatorChar.EQU:
                    c = "=";
                    break;
                case OperatorChar.NEQ:
                    c = "<>";
                    break;
                case OperatorChar.LSS:
                    c = "<";
                    break;
                case OperatorChar.LEQ:
                    c = "<=";
                    break;
                case OperatorChar.GTR:
                    c = ">";
                    break;
                case OperatorChar.GEQ:
                    c = ">=";
                    break;
                default:
                    break;
            }
            string where = string.Empty;
            where = NMSString.Append(" ", o, " [", Model.GetColumnName(data), "]", c, "@", data);
            return where;
        }
    }
}

using Dapper;
using Mellivora;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vasily;

namespace Demo
{
    class Program
    {
        static int count = 10000;
        static string sql = "SELECT * FROM STUDENT WHERE Age=@Age AND Name=@Name";
        static Test param = null;
        static IDbConnection connection = null;
        static void Main(string[] args)
        {
            param = new Test() { Name = "new", Age = 101 };

            VasilyHandler.Initialize();
            //数据库在data文件夹里
            connection = new SqlConnection("Data Source=192.168.190.128;Initial Catalog=Test;Integrated Security=False;User ID=sa;Password=123456");


            var dapper_result  = connection.Query<Test>(sql, param);
            connection.Query<Test>(sql, param);
            var mellivora_result = connection.QueryByInstance<Test>(sql, param);
            connection.QueryByObjects<Test>(sql, param.Age, param.Name);

            PerfomanceHelper compare = new PerfomanceHelper(count);
            Thread.Sleep(0);
            compare.Add("DapperTest", DapperTest);
            compare.Add("DapperTest", DapperTest);
            Thread.Sleep(0);
            compare.Add("MellivoraTest", MellivoraTest);
            compare.Add("MellivoraTest", MellivoraTest);

            compare.ShowReuslt();
            Console.ReadKey();

        }
        public static void DapperTest()
        {
            for (int i = 0; i < count; i += 1)
            {
                IEnumerable<Test> r = connection.Query<Test>(sql, param);
            }
        }
        public static void MellivoraTest()
        {
            for (int i = 0; i < count; i += 1)
            {
                IEnumerable<Test> r = connection.QueryByInstance<Test>(sql, param);
            }
        }
    }
}

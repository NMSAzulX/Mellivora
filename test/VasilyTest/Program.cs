using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vasily;
using VasilyTest.Model;

namespace VasilyTest
{
    class Program
    {
        static void Main(string[] args)
        {
            VasilyHandler.Initialize();
            Console.WriteLine(Sql<AlTest>.AL("根据学生类型或者分数查询状态集合"));
            Console.ReadKey();
        }
    }
}

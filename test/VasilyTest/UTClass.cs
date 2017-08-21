using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vasily;
using Vasily.Utils;
using VasilyTest.Model;
using Xunit;

namespace VasilyTest
{
    [Trait("SQL语句", "普通")]
    [VasilyInitialize]
    public class UTClass
    {
        [Fact(DisplayName = "查重语句")]

        public void Test_Repeate()
        {
            Assert.Equal(Sql<Test>.CheckRepeate, "SELECT COUNT(*) FROM [tablename] WHERE [StudentId]=@StudentId");
        }

        [Fact(DisplayName = "全部查询语句")]
        public void Test_SelectAll()
        {
            Assert.Equal(Sql<Test>.SelectAll, "SELECT * FROM [tablename]");
        }

        [Fact(DisplayName = "主键查询语句")]
        public void Test_Select()
        {
            Assert.Equal(Sql<Test>.Select, "SELECT * FROM [tablename] WHERE [ID]=@ID");
        }

        [Fact(DisplayName = "更新语句")]
        public void Test_Update()
        {
            Assert.Equal(Sql<Test>.Update, "UPDATE [tablename] SET [Age]=@Age,[Html Title]=@Title,[UpdateTime]=@UpdateTime,[CreateTime]=@CreateTime,[School Description]=@Desc,[StudentId]=@StudentId WHERE [ID]=@ID");
        }
        
        [Fact(DisplayName = "插入语句")]
        public void Test_Insert()
        {
            Assert.Equal(Sql<Test>.Insert, "INSERT INTO [tablename] ([Age],[Html Title],[UpdateTime],[CreateTime],[School Description],[StudentId]) VALUES (@Age,@Title,@UpdateTime,@CreateTime,@Desc,@StudentId)");
        }

        [Fact(DisplayName = "删除语句")]
        public void Test_Delete()
        {
            Assert.Equal(Sql<Test>.Delete, "DELETE [tablename] WHERE [ID]=@ID");
        }
    }
    [Trait("SQL语句", "条件")]
    [VasilyInitialize]
    public class UTCondionClass
    {

        [Fact(DisplayName = "查询语句")]
        public void Test_SelectAll()
        {
            Assert.Equal(Sql<Test>.ConditionSelect, "SELECT * FROM [tablename] WHERE ");
        }


        [Fact(DisplayName = "更新语句")]
        public void Test_Update()
        {
            Assert.Equal(Sql<Test>.ConditionUpdate, "UPDATE [tablename] SET [Age]=@Age,[Html Title]=@Title,[UpdateTime]=@UpdateTime,[CreateTime]=@CreateTime,[School Description]=@Desc,[StudentId]=@StudentId WHERE ");
        }

        [Fact(DisplayName = "插入语句")]
        public void Test_Delete()
        {
            Assert.Equal(Sql<Test>.ConditionDelete, "DELETE [tablename] WHERE ");
        }

    }

    [Trait("SQL语句", "AL")]
    [VasilyInitialize]
    public class ALClass
    {
        [Fact(DisplayName = "根据名字和年龄查备注")]
        public void Test_Al1()
        {
            Assert.Equal(Sql<AlTest>.AL("根据名字和年龄查备注"), "SELECT [Description] FROM [tablename] WHERE [Age]=@Age AND [Name]=@Name");
        }

        [Fact(DisplayName = "查询大于当前StudentId的集合")]
        public void Test_Al2()
        {
            Assert.Equal(Sql<AlTest>.AL("查询大于当前StudentId的集合"), "SELECT [Count] FROM [tablename] WHERE [StudentId]>@StudentId");
        }

        [Fact(DisplayName = "根据学生类型或者分数查询状态集合")]
        public void Test_Al3()
        {
            Assert.Equal(Sql<AlTest>.AL("根据学生类型或者分数查询状态集合"), "SELECT [Status] FROM [tablename] WHERE [Score]=@Score OR [StudentType]=@StudentType");
        }
    }
}

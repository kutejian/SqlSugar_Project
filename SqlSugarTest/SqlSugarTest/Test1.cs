using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugarTest.Test1
{
    public class Test1
    {
        //一对一查询
        public void OneToOne(SqlSugarClient db)
        {
            if (db.DbMaintenance.IsAnyTable("StudentA"))
            db.DbMaintenance.DropTable("StudentA");
            if (db.DbMaintenance.IsAnyTable("SchoolA"))
                db.DbMaintenance.DropTable("SchoolA");
            db.CodeFirst.InitTables<StudentA>();
            db.CodeFirst.InitTables<SchoolA>();
            List<StudentA> studentAs = new List<StudentA>()
            {
                new StudentA()
                {
                    Name = "A1",
                    SchoolA = new SchoolA()
                    {
                        SchoolName = "啊1",
                    }
                },
                new StudentA()
                {
                    Name = "A2",
                    SchoolA = new SchoolA()
                    {
                        SchoolName = "啊2",
                    }
                },
                new StudentA()
                {
                    Name = "A3",
                    SchoolA = new SchoolA()
                    {
                        SchoolName = "啊3",
                    }
                },
                new StudentA()
                {
                    Name = "A4",
                    SchoolA = new SchoolA()
                    {
                        SchoolName = "啊3",
                    }
                },
            };

            var entity = db.InsertNav(studentAs).Include(x=>x.SchoolA).ExecuteCommand();


            //导航+主表过滤  导航属性过滤
            var list = db.Queryable<StudentA>()
                     .Includes(x => x.SchoolA) //填充子对象 （不填充可以不写）
                     .Where(x => x.SchoolA.SchoolName == "啊3")
                     .ToList();


            //导航+主表过滤  只查有导航数据 （新功能：5.1.2.8）
            var list2 = db.Queryable<StudentA>()
                     .Includes(x => x.SchoolA) //填充子对象 （不填充可以不写）
                     .Where(x => SqlFunc.Exists(x.SchoolA.Id))
                     .ToList();

            //导航如果只查一个字段         
            var list3 = db.Queryable<StudentA>()
                         .Where(x => x.SchoolId > 1)  //Where和Select中别名要写一样
                         .Select(x => new {
                             SchoolName = x.SchoolA.SchoolName
                         }).ToList();


        }
       
    }
    //实体
    public class StudentA
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int StudentId { get; set; }
        public string Name { get; set; }
        public int SchoolId { get; set; }

        //用例1：主键模式 StudentA（主表）表中的 SchoolId 和SchoolA（子表）中的主键关联 
        [Navigate(NavigateType.OneToOne, nameof(SchoolId))]//一对一 SchoolId是StudentA类里面的
        public SchoolA SchoolA { get; set; } //不能赋值只能是null
    }
    public class SchoolA
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string SchoolName { get; set; }
    }

    
}

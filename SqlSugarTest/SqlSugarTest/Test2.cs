using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugarTest.Test2
{
    public class Test2
    {
        public void OneToMany(SqlSugarClient db)
        {
            if (db.DbMaintenance.IsAnyTable("StudentA"))
                db.DbMaintenance.DropTable("StudentA");
            if (db.DbMaintenance.IsAnyTable("BookA"))
                db.DbMaintenance.DropTable("BookA");
            db.CodeFirst.InitTables<StudentA>();
            db.CodeFirst.InitTables<BookA>();
            List<StudentA> studentAs = new List<StudentA>()
            {
                new StudentA()
                {
                    Name = "A1",
                    Books = new List<BookA>()
                    {
                        new BookA()
                        {
                            Name="小1"
                        },
                        new BookA()
                        {
                            Name="小2"
                        },
                        new BookA()
                        {
                            Name="小3"
                        },
                        new BookA()
                        {
                            Name="小4"
                        },
                    }
                },
                
            };

            var entity = db.InsertNav(studentAs).Include(x => x.Books).ExecuteCommand();

            //例1：简单用法
            var list = db.Queryable<StudentA>()
            .Includes(x => x.Books)
            .ToList();

            //例2：支持Any和Count 对主表进行过滤 (子对象过滤看下面)
            var list2 = db.Queryable<StudentA>()
            .Includes(x => x.Books)
            .Where(x => x.Books.Any())
            //带条件的
            //.Where(x => x.Books.Any(z=>z.Name=="jack")))
            .ToList();

            //例3: 没有Includes也可以使用过滤
            var list3 = db.Queryable<StudentA>()
            .Where(x => x.Books.Any())//Any中可以加条件 Any(z=>z.BookId==1)
            .ToList();


            //例4 Where子对象进行排序和过滤 (支持WhereIF)
            var list4 = db.Queryable<StudentA>()
              .Includes(x => x.Books.Where(y => y.BookId > 0).OrderByDescending(y => y.BookId).ToList())
              .ToList();

            //例5 主表+子表都过滤
            var list5 = db.Queryable<StudentA>()
            .Includes(x => x.Books.Where(it => it.Name == "jack"))//只过滤子表
            .Where(x => x.Books.Any(z => z.Name == "jack")).ToList();//通过子表过滤主表

            //例6：Select指定字段
            var list6 = db.Queryable<StudentA>()
                       .Includes(x => x.Books.Select(z => new BookA() { Name = z.Name }).ToList()).ToList();
        }
    }
    public class StudentA
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set; }

        //用例1：正常一对多
        [Navigate(NavigateType.OneToMany, nameof(BookA.studenId))]//BookA表中的studenId
        public List<BookA> Books { get; set; }//注意禁止给books手动赋值

    }
    public class BookA
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int BookId { get; set; }
        public string Name { get; set; }
        public int studenId { get; set; }
    }
}

using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugarTest.Test3
{
    internal class Test3
    {
        public void ManyToMany(SqlSugarClient db)
        {

            if (db.DbMaintenance.IsAnyTable("A1"))
                db.DbMaintenance.DropTable("A1");
            if (db.DbMaintenance.IsAnyTable("B1"))
                db.DbMaintenance.DropTable("B1");
            if (db.DbMaintenance.IsAnyTable("ABMapping1"))
                db.DbMaintenance.DropTable("ABMapping1");
            db.CodeFirst.InitTables<A1, B1, ABMapping1>();
            List<A1> studentAs = new List<A1>()
            {
                new A1()
                {
                    Name = "A1",
                    BList = new List<B1>()
                    {
                        new B1()
                        {
                            Name="小1"
                        },
                        new B1()
                        {
                            Name="小2"
                        },
                        new B1()
                        {
                            Name="小3"
                        },
                        new B1()
                        {
                            Name="小4"
                        },
                    }
                },new A1()
                {
                    Name = "A2",
                    BList = new List<B1>()
                    {
                        new B1()
                        {
                            Name="大1"
                        },
                        new B1()
                        {
                            Name="大2"
                        },
                    }
                },

            };

            var entity = db.InsertNav(studentAs).Include(x => x.BList).ExecuteCommand();





            //例1:简单用法 直接填充B的集合，只要配置好特性非常简单
            var list1 = db.Queryable<A1>().Includes(x => x.BList).ToList();



            //例2:支持子对象排序和过滤 (支持WhereIF)
            var list2 = db.Queryable<A1>().Includes(x => x.BList.Where(z => z.Id > 0).ToList()).ToList();

            //例3:支持主表过滤  Any和Count
            var list3 = db.Queryable<A1>().Includes(x => x.BList)
                            .Where(x => x.BList.Any())//Any里面可以加条件 Any(z=>z.xxxx>0)
                            .ToList();
            //例4主表+子表都过滤
            var list4 = db.Queryable<A1>()
               .Includes(x => x.BList.Where(it => it.Name == "jack"))//只过滤子表
               .Where(x => x.BList.Any(z => z.Name == "jack")).ToList();//通过子表过滤主表


            //不使用Includes一样可以过滤              
            var list5 = db.Queryable<A1>()
                            .Where(x => x.BList.Any()) //可以加条件.Where(x=>x.BList.Any(z=>z.xxx==x.yyy))
                            .ToList();

            //多对多后还可用追加字段映射MappingField 如果以前是2个字段关联，现在追加后就成了2+1       
            /* 
               db.Queryable<A1>().Includes(x => x.BList.MappingField(z => z.字段, () => x.字段).ToList())
                             .ToList();
            */
        }
    }
    //实体
    public class ABMapping1
    {
        [SugarColumn(IsPrimaryKey = true)]//中间表可以不是主键
        public int AId { get; set; }
        [SugarColumn(IsPrimaryKey = true)]//中间表可以不是主键
        public int BId { get; set; }
    }
    public class A1
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        [Navigate(typeof(ABMapping1), nameof(ABMapping1.AId), nameof(ABMapping1.BId))]//注意顺序
        public List<B1> BList { get; set; }//只能是null不能赋默认值
    }
    public class B1
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        [Navigate(typeof(ABMapping1), nameof(ABMapping1.BId), nameof(ABMapping1.AId))]//注意顺序
        public List<A1> AList { get; set; }//只能是null不能赋默认值
    }
}

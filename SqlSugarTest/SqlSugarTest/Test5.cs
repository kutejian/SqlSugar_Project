using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugarTest.Test5
{
    internal class Test5
    {
        public void TreeQuery(SqlSugarClient db)
        {
            if (db.DbMaintenance.IsAnyTable("Tree"))
                db.DbMaintenance.DropTable("Tree");
            db.CodeFirst.InitTables<Tree>();
            //创建数据 创建树
            Tree tree = new Tree()
            {
                Id=1,
                Name="一级树",
                ParentId=0,
                Child = new List<Tree>
                {
                    new Tree()
                    {
                        ParentId=1,
                        Id=5,
                        Name="二级目录-1",
                        Child = new List<Tree>
                        {
                            new Tree()
                            {
                                Id=6,
                                Name="三级目录-1",
                                ParentId=2,
                                Child = new List<Tree>
                                { 
                                    new Tree()
                                    {
                                        Id = 7,
                                        ParentId = 6,
                                        Name = "四级目录-1"
                                    }
                                    
                                }
                            }
                        }
                    }
                }
            };
            db.InsertNav<Tree>(tree)
                .Include(c1 => c1.Child)
                .ThenInclude(c2 => c2.Child)
                .ThenInclude(c3 => c3.Child)
                .ThenInclude(c4 => c4.Child).ExecuteCommand();

            var treeList = db.Queryable<Tree>().ToTree(it => it.Child, it => it.ParentId, 0);
            //俩效果一样
            var treeRoot = db.Queryable<Tree>().Where(it => it.Id == 1).ToList();
            db.ThenMapper(treeRoot, item =>
            {
                item.Child = db.Queryable<Tree>().SetContext(x => x.ParentId, () => item.Id, item).ToList();
            });
            //第二层
            db.ThenMapper(treeRoot.SelectMany(it => it.Child), it =>
            {
                it.Child = db.Queryable<Tree>().SetContext(x => x.ParentId, () => it.Id, it).ToList();
            });
            //第三层
            db.ThenMapper(treeRoot.SelectMany(it => it.Child).SelectMany(it => it.Child), it =>
            {
                it.Child = db.Queryable<Tree>().SetContext(x => x.ParentId, () => it.Id, it).ToList();
            });

            //查询所有上级
            var lccist = db.Queryable<Tree>().ToParentList(it => it.ParentId, 5);
            //查找所有下级
            var allchilds2 = db.Queryable<Tree>().ToChildList(it => it.ParentId, 5);

        }
    }
    public class Tree
    {

        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public int Id { get; set; } //关联字段 默认是主键
        public string Name { get; set; }
        public int ParentId { get; set; }//父级字段

        [Navigate(NavigateType.OneToMany, nameof(ParentId))]
        public List<Tree> Child { get; set; }
    }
}

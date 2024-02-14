using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugarTest.Test4
{
    internal class Test4
    {
        public void NONavigatio(SqlSugarClient db)
        {
            //创建数据
            if (db.DbMaintenance.IsAnyTable("StudentA"))
                db.DbMaintenance.DropTable("StudentA");
            if (db.DbMaintenance.IsAnyTable("RoomA"))
                db.DbMaintenance.DropTable("RoomA");
            if (db.DbMaintenance.IsAnyTable("SchoolA"))
                db.DbMaintenance.DropTable("SchoolA");
            if (db.DbMaintenance.IsAnyTable("TeacherA"))
                db.DbMaintenance.DropTable("TeacherA");
            db.CodeFirst.InitTables<StudentA, RoomA, SchoolA, TeacherA>();

            db.Insertable(new RoomA() { RoomId = 1, RoomName = "北大001室", SchoolId = 1 }).ExecuteCommand();
            db.Insertable(new RoomA() { RoomId = 2, RoomName = "北大002室", SchoolId = 1 }).ExecuteCommand();
            db.Insertable(new RoomA() { RoomId = 3, RoomName = "北大003室", SchoolId = 1 }).ExecuteCommand();
            db.Insertable(new RoomA() { RoomId = 4, RoomName = "清华001厅", SchoolId = 2 }).ExecuteCommand();
            db.Insertable(new RoomA() { RoomId = 5, RoomName = "清华002厅", SchoolId = 2 }).ExecuteCommand();
            db.Insertable(new RoomA() { RoomId = 6, RoomName = "清华003厅", SchoolId = 2 }).ExecuteCommand();


            db.Insertable(new SchoolA() { SchoolId = 1, SchoolName = "北大" }).ExecuteCommand();
            db.Insertable(new SchoolA() { SchoolId = 2, SchoolName = "清华" }).ExecuteCommand();

            db.Insertable(new StudentA() { StudentId = 1, SchoolId = 1, Name = "北大jack" }).ExecuteCommand();
            db.Insertable(new StudentA() { StudentId = 2, SchoolId = 1, Name = "北大tom" }).ExecuteCommand();
            db.Insertable(new StudentA() { StudentId = 3, SchoolId = 2, Name = "清华jack" }).ExecuteCommand();
            db.Insertable(new StudentA() { StudentId = 4, SchoolId = 2, Name = "清华tom" }).ExecuteCommand();

            db.Insertable(new TeacherA() { SchoolId = 1, Id = 1, Name = "北大老师01" }).ExecuteCommand();
            db.Insertable(new TeacherA() { SchoolId = 1, Id = 2, Name = "北大老师02" }).ExecuteCommand();

            db.Insertable(new TeacherA() { SchoolId = 2, Id = 3, Name = "清华老师01" }).ExecuteCommand();
            db.Insertable(new TeacherA() { SchoolId = 2, Id = 4, Name = "清华老师02" }).ExecuteCommand();


            var list = db.Queryable<StudentA>().ToList();//这儿也可以联表查询
            db.ThenMapper(list, stu =>
            {

                //俩个效果一样不过 第一个性能高
                stu.SchoolA = db.Queryable<SchoolA>().SetContext(scl => scl.SchoolId, () => stu.SchoolId, stu).FirstOrDefault();
                //stu.SchoolA = db.Queryable<SchoolA>().Where(x=>x.SchoolId==stu.SchoolId).First();
                //可以联查询的
                //stu.xxxx=db.Queryable<SchoolA>().LeftJoin<XXX>().Select(xxxx).SetContext(....).ToList();
            });
            // SetContext不会生成循环操作，高性能  和直接Where性能是不一样的
        }
    }
    public class StudentA
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int StudentId { get; set; }
        public string Name { get; set; }
        public int SchoolId { get; set; }
        [SugarColumn(IsIgnore = true)]
        public SchoolA SchoolA { get; set; }
    }

    public class SchoolA
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int SchoolId { get; set; }
        public string SchoolName { get; set; }
        [SugarColumn(IsIgnore = true)]
        public List<RoomA> RoomList { get; set; }
        [SugarColumn(IsIgnore = true)]
        public List<TeacherA> TeacherList { get; set; }
    }
    public class TeacherA
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int Id { get; set; }
        public int SchoolId { get; set; }
        public string Name { get; set; }
    }
    public class RoomA
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int RoomId { get; set; }
        public string RoomName { get; set; }
        public int SchoolId { get; set; }
    }
}

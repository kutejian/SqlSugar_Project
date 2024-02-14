
using Models;
using SqlSugar;
using SqlSugarTest;
using System.Drawing.Printing;
using System.Security.AccessControl;
using System.Security.Claims;
using System.Xml.Linq;
Console.WriteLine("Hello, World!");



var dath = "C:\\Users\\30765\\Desktop\\SqlSugar_Project\\SqlSugarTest\\Model";
var connstring = "TrustServerCertificate = true; server =.; database = kute; uid = sa; pwd = 123";

ConnectionConfig connectionConfig = new ConnectionConfig()
{
    ConnectionString = connstring,
    IsAutoCloseConnection = true,
    DbType = DbType.SqlServer
};

using (SqlSugarClient db = new SqlSugarClient(connectionConfig))
{
    //.net6以上 string加?
    db.DbFirst.IsCreateAttribute().StringNullable().CreateClassFile(dath, "Models");

    db.Aop.OnLogExecuting = (sql, pars) =>
    {
        Console.WriteLine(sql);//输出sql,查看执行sql 性能无影响
    };

    //第一个导航实例 一对一
    /*    var  test1 = new SqlSugarTest.Test1.Test1();
        test1.OneToOne(db);*/

    //第二个导航实例 一对多
    /*    var test2 = new SqlSugarTest.Test2.Test2();
        test2.OneToMany(db);*/

    //第三个导航实例 多对多 和中间表
    /*      var test3 = new SqlSugarTest.Test3.Test3();
            test3.ManyToMany(db);*/

    //第四个导航实例 在没有使用导航属性的情况下怎么连表查询
    /*    var test4 = new SqlSugarTest.Test4.Test4();
        test4.NONavigatio(db);*/

    //第四个导航实例 在没有使用导航属性的情况下怎么连表查询
       var test5 = new SqlSugarTest.Test5.Test5();
       test5.TreeQuery(db);


    {//新增表
        db.CodeFirst.InitTables(typeof(Test));

        //获取所有表
        var list2 = db.DbMaintenance.GetTableInfoList();
        Console.WriteLine(list2);

        //获取哪个表里的主键
        var list3 = db.DbMaintenance.GetPrimaries(list2[1].Name);
        Console.WriteLine(list3);
    }

    {//新增
        Test test = new Test()
        {
            userName = "kute"
        };
        //返回插入行数
        var en = db.Insertable(test).ExecuteCommand();
        Console.WriteLine(en);

        //多个新增 如果数据更大去查官方文档
        List<Test> tests = new List<Test>(){
        test,test,test,test,test,test, test,test,test,test,test,
    };
        //返回插入行数 11
        var ens = db.Insertable(tests).ExecuteCommand();
        Console.WriteLine(ens);
    }

    {//删除 批量删除
        //单个实体
        db.Deleteable<Test>(new Test() { userId = 1 }).ExecuteCommand();
        //List<实体> (可以不加Where)
        List<Test> listtest = new List<Test>(){
            new Test() { userId = 2 },
            new Test() { userId = 3 }
        };
        db.Deleteable<Test>(listtest).ExecuteCommand(); //批量删除
    }
    
    { //更新
        List<Test> listtest2 = new List<Test>(){
            new Test() { userId = 26 , userName ="www" },
            new Test() { userId = 25 , userName ="w1ww" }
        };
        var result = db.Updateable(listtest2).ExecuteCommand();

        //只更新部分 
        var updateObj = new Test() { userId = 10 }; //主键要有值

        db.Tracking(updateObj);//创建跟踪
        updateObj.userName = "a1" + Guid.NewGuid();//只改修改了name那么只会更新name
        db.Updateable(updateObj).ExecuteCommand();//因为每条记录的列数不一样，批量数据多性能差，不建议用
                                                  //可以清空
        db.ClearTracking();
    }

    //查询
    var list = db.Queryable<Article>().ToList();
    Console.WriteLine(list);

    var exp = Expressionable.Create<Article>();
    exp.OrIF(true , it => it.ArticleId == 3);//.OrIf 是条件成立才会拼接OR
    exp.Or(it => it.ArticleTitle.Contains("E"));//拼接OR
    var list1 = db.Queryable<Article>().Where(exp.ToExpression()).ToList();

    int totalCount = 0;
    var page = db.Queryable<Article>().ToPageList(1, 2, ref totalCount);

    Console.ReadKey();
}

internal class Test
{
    public int userId { get; set;}
    public string userName { get; set;}
}
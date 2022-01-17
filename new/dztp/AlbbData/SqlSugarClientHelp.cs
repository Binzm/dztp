using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlbbData.entity;
using ArchitectureDesignSafety;

namespace AlbbData
{
    public class SqlSugarClientHelp
    {
        SqlSugarClient _db;

        public SqlSugarClientHelp()
        {
            _db = GetInstance();
            CreateDatabaseAndTable(false, 50, typeof(ShopInfoEntity));
            CreateDatabaseAndTable(false, 50, typeof(ShopListInfoEntity));
        }

        //创建SqlSugarClient 
        private SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = Global.mysqlServer,//连接符字串
                DbType = DbType.MySql,
                IsAutoCloseConnection = true
            });

            //添加Sql打印事件，开发中可以删掉这个代码
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                Console.WriteLine(sql);
            };
            return db;
        }

        private void CreateDatabaseAndTable(bool Backup = false, int StringDefaultLength = 50, params Type[] types)
        {
            _db.CodeFirst.SetStringDefaultLength(StringDefaultLength);
            _db.DbMaintenance.CreateDatabase();
            if (Backup)
            {
                _db.CodeFirst.BackupTable().InitTables(types);
            }
            else
            {
                _db.CodeFirst.InitTables(types);
            }
        }

        public object GetDataBySql(string sql)
        {
            return _db.Ado.GetInt(sql);
        }

      


        public SimpleClient<ShopInfoEntity> ShopInfoDb { get { return new SimpleClient<ShopInfoEntity>(_db); } }
        public SimpleClient<ShopListInfoEntity> ShopListInfoDb { get { return new SimpleClient<ShopListInfoEntity>(_db); } }
    }
}

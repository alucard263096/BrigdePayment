using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLink.core
{
    public static class DBFactory
    {
        private static DBInstance dbmgr = null;
        public static DBInstance GetInstance()
        {
            if (dbmgr == null)
            {
                string hosts=ConfigurationManager.AppSettings.Get("applink.mysql.hosts");
                string userid = ConfigurationManager.AppSettings.Get("applink.mysql.userid");
                string password = ConfigurationManager.AppSettings.Get("applink.mysql.password");
                string database = ConfigurationManager.AppSettings.Get("applink.mysql.database");
                Init(hosts,userid,password,database);
            }
            return dbmgr;
        }

        public static void Init(string hosts,string userid,string password,string database )
        {
            //server=localhost;user id=root;database=footercms
            string connectstr = string.Format("server={0};user id={1};password={2};database={3}",
                hosts,userid,password,database);
            dbmgr = new DBInstance("MySqlClient", connectstr);
        }
    }
}

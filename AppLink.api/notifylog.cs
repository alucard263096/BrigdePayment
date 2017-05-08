using AppLink.core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLink.api
{
  public static class NotifylogMgr 
{

        /// <summary>
        /// 更新notifylog，传入对应的表字段
        /// </summary>
        
        /// <param name="api">Get from APIFactory.GetInstance()</param>
        /// <param name="requsetParam">list.add(new AppLink.core.Param("key","value")</param>
        /// <returns>Object[] or Object to change to Dictionary<string,string>[] or Dictionary<string,string></returns>
                       public static Object update(APIInstance api, List<Param> requsetParam){
              return api.CallApi("notifylog/update", requsetParam);
        }

        /// <param name="api">Get from APIFactory.GetInstance()</param>
        /// <param name="requestParam">list.add(new AppLink.core.Param("key","value")</param>
        /// <param name="requestParam">Async Call back to get data</param>
                public static void update(APIInstance api, List<Param> requsetParam,APIInstance.CallbackDelegate callback){
              api.CallApiAsync("notifylog/update", requsetParam, callback);
        }

public static int update(DBInstance dbmgr,List<Param> request,int id=0){
  //id=0为插入新字段
   
              using (DbConnection conn = dbmgr.GetDbConnection())
            {
                conn.Open();
                using (DbTransaction tx = conn.BeginTransaction())
                {

                string sql="";

                if(id==0){
                  id=dbmgr.getNewId("tb_notify_log");
                  sql="insert into tb_notify_log (id,created_user,created_date,updated_user,updated_date,`notify_time`,`log_request`,`log_url` ) values ("+id.ToString()+",-2,now(),-2,now(),@notify_time ,@log_request ,@log_url  )";
                }else{
                  sql="update tb_notify_log set updated_user=-2, updated_date=now() ,`notify_time`=@notify_time ,`log_request`=@log_request ,`log_url`=@log_url  where id="+id.ToString();
                }

                dbmgr.ExecuteNonQuery(tx,sql,request);

                
                

                tx.Commit();
                return id;

              }
            }
            
}

}

}

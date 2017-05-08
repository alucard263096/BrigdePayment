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
  public static class PaymentMgr 
{

        /// <summary>
        /// 获取payment列表，传入对应的搜索条件
        /// </summary>
        
        /// <param name="api">Get from APIFactory.GetInstance()</param>
        /// <param name="requsetParam">list.add(new AppLink.core.Param("key","value")</param>
        /// <returns>Object[] or Object to change to Dictionary<string,string>[] or Dictionary<string,string></returns>
                       public static Object list(APIInstance api, List<Param> requsetParam){
              return api.CallApi("payment/list", requsetParam);
        }

        /// <param name="api">Get from APIFactory.GetInstance()</param>
        /// <param name="requestParam">list.add(new AppLink.core.Param("key","value")</param>
        /// <param name="requestParam">Async Call back to get data</param>
                public static void list(APIInstance api, List<Param> requsetParam,APIInstance.CallbackDelegate callback){
              api.CallApiAsync("payment/list", requsetParam, callback);
        }

public static DataTable list(DBInstance dbmgr,List<Param> searchParam,string orderby=""){

    StringBuilder sql_where=new StringBuilder();


    if(Param.FindContainParamKey(searchParam,"orderno"))
    {
        sql_where.Append(" and r_main.orderno like @orderno ");
    }
    if(Param.FindContainParamKey(searchParam,"time_end"))
    {
        sql_where.Append(" and r_main.time_end like @time_end ");
    }
    if(Param.FindContainParamKey(searchParam,"openid"))
    {
        sql_where.Append(" and r_main.openid like @openid ");
    }
    if(Param.FindContainParamKey(searchParam,"total_fee"))
    {
        sql_where.Append(" and r_main.total_fee=@total_fee ");
    }
    if(Param.FindContainParamKey(searchParam,"trade_type"))
    {
        sql_where.Append(" and r_main.trade_type like @trade_type ");
    }
    if(Param.FindContainParamKey(searchParam,"transaction_id"))
    {
        sql_where.Append(" and r_main.transaction_id like @transaction_id ");
    }
    if(Param.FindContainParamKey(searchParam,"result_code"))
    {
        sql_where.Append(" and r_main.result_code like @result_code ");
    }
    string sql="select  r_main.id  ,r_main.orderno ,r_main.time_end ,r_main.openid ,r_main.total_fee ,r_main.trade_type ,r_main.transaction_id ,r_main.result_code  from  tb_payment r_main  where 1=1 "+sql_where.ToString()+"  "+orderby;
                
                return dbmgr.ExecuteDataTable(sql, searchParam);

}

        /// <summary>
        /// 更新payment，传入对应的表字段
        /// </summary>
        
        /// <param name="api">Get from APIFactory.GetInstance()</param>
        /// <param name="requsetParam">list.add(new AppLink.core.Param("key","value")</param>
        /// <returns>Object[] or Object to change to Dictionary<string,string>[] or Dictionary<string,string></returns>
                       public static Object update(APIInstance api, List<Param> requsetParam){
              return api.CallApi("payment/update", requsetParam);
        }

        /// <param name="api">Get from APIFactory.GetInstance()</param>
        /// <param name="requestParam">list.add(new AppLink.core.Param("key","value")</param>
        /// <param name="requestParam">Async Call back to get data</param>
                public static void update(APIInstance api, List<Param> requsetParam,APIInstance.CallbackDelegate callback){
              api.CallApiAsync("payment/update", requsetParam, callback);
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
                  id=dbmgr.getNewId("tb_payment");
                  sql="insert into tb_payment (id,created_user,created_date,updated_user,updated_date,`orderno`,`time_end`,`openid`,`total_fee`,`trade_type`,`transaction_id`,`result_code` ) values ("+id.ToString()+",-2,now(),-2,now(),@orderno ,@time_end ,@openid ,@total_fee ,@trade_type ,@transaction_id ,@result_code  )";
                }else{
                  sql="update tb_payment set updated_user=-2, updated_date=now() ,`orderno`=@orderno ,`time_end`=@time_end ,`openid`=@openid ,`total_fee`=@total_fee ,`trade_type`=@trade_type ,`transaction_id`=@transaction_id ,`result_code`=@result_code  where id="+id.ToString();
                }

                dbmgr.ExecuteNonQuery(tx,sql,request);

                
                

                tx.Commit();
                return id;

              }
            }
            
}

}

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace AppLink.core
{
    public class Param
    {
        public static List<Param> ConvertDictionaryToList(Dictionary<string,object> dict)
        {
            List<Param> list = new List<Param>();

            foreach (var item in dict)
            {
                list.Add(new Param(item.Key,item.Value));
            }

            return list;
        }
        
        public static bool FindContainParamKey(List<Param> list,string key) 
        {
            foreach (var item in list)
            {
                if (item.name == key)
                {
                    return true;
                }
            }
            return false;
        }

        public static Param GetParam(List<Param> list, string key)
        {
            foreach (var item in list)
            {
                if (item.name == key)
                {
                    return item;
                }
            }
            return null;
        }

        string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        DbType dbType;

        public DbType DbType
        {
            get { return dbType; }
            set { dbType = value; }
        }

        object value;

        public object Value
        {
            get { return this.value; }
            set { this.value = value; }
        }


        /// <summary>
        /// 数据库 值传递
        /// </summary>
        /// <param name="_name">名称，一般前面加@</param>
        /// <param name="_dbtype">数据类型</param>
        /// <param name="_value">值</param>
        public Param(string _name, DbType _dbtype, object _value)
        {
            this.name = _name;
            this.dbType = _dbtype;
            this.value = _value;
        }
        public Param(string _name, object _value)
        {
            this.name = _name;
            this.value = _value;
        }


        public static List<Param> GetDBParamList(string[] name, object[] value)
        {
            List<Param> dnp = new List<Param>();
            for (int i = 0; i < name.Length; i++)
			{
                DbType db;
                if(value[i] is Int32)
                {
                    db=DbType.Int32;
                }
                else if(value[i] is DateTime)
                {
                    db=DbType.DateTime;
                }
                else if (value[i] is decimal)
                {
                    db = DbType.Decimal;
                }
                else if (value[i] is string)
                {
                    db = DbType.String;
                }
                else
                {
                    db =DbType.Object;
                }

                dnp.Add(new Param(name[i], db, value[i]));
			}
            
                return dnp;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;

namespace AppLink.core
{
    public class DBInstance
    {
        private DbProviderFactory _factory;
        private string _connectionString;

        /// <summary>
        /// 数据库对象实例
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="connectionString"></param>
        public DBInstance(string provider, string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException();
            }
            
            _connectionString = connectionString;
            _factory = DbProviderFactories.GetFactory(provider);

        }

        public DBInstance(DbProviderFactory _provider, string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException();
            }


            _connectionString = connectionString;
            _factory = _provider;

        }

        /// <summary>
        /// 获取数据库连接对象，用于做事务T-SQL为主
        /// </summary>
        /// <returns></returns>
        public DbConnection GetDbConnection()
        {
            //string connectionString = ConfigurationManager.AppSettings.Get("connectionString");
            DbConnection conn = _factory.CreateConnection();
            conn.ConnectionString = _connectionString;
            return conn;
        }

        #region validate
        private void Validate(DbTransaction tx, string cmdTxt)
        {
            if (tx == null || tx.Connection == null || string.IsNullOrEmpty(cmdTxt))
            {
                throw new ArgumentNullException();
            }
        }

        private void Validate(string cmdTxt)
        {
            if (string.IsNullOrEmpty(cmdTxt))
            {
                throw new ArgumentNullException();
            }
        }
        #endregion

        #region prepare cmd
        private void PrepareCommand(DbCommand cmd, string cmdTxt, DbTransaction tx,List<Param> parameters)
        {
            cmd.CommandText = cmdTxt;
            cmd.Transaction = tx;
            this.AddParameters(cmd, parameters);
        }

        private void PrepareCommand(DbCommand cmd, string cmdTxt, List<Param> parameters)
        {
            cmd.CommandText = cmdTxt;
            this.AddParameters(cmd, parameters);
        }

        private void AddParameters(DbCommand command, List<Param> parameters)
        {
            if (parameters != null)
            {
                foreach (Param para in parameters)
                {
                    DbParameter param = command.CreateParameter();

                    param.ParameterName = para.Name;
                    param.Value = para.Value;
                    param.Size = Int32.MaxValue;

                    /*if (DbType.Object != para.DbType)
                    {
                        param.DbType = para.DbType;
                    }*/
                    command.Parameters.Add(param);
                }

            }
        }

        #endregion

        #region execute scalar
        public object ExecuteScalar(DbTransaction transaction, string commandText, List<Param> parameters)
        {
            this.Validate(transaction, commandText);
            using (DbCommand cmd = transaction.Connection.CreateCommand())
            {
                this.PrepareCommand(cmd, commandText, transaction, parameters);
                return cmd.ExecuteScalar();
            }
        }

        public object ExecuteScalar(string commandText, List<Param> parameters)
        {
            Validate(commandText);
            //UpdateConnectionStr();
            using (DbConnection conn = this.GetDbConnection())
            {
                using (DbCommand cmd = conn.CreateCommand())
                {
                    this.PrepareCommand(cmd, commandText, parameters);
                    conn.Open();
                    return cmd.ExecuteScalar();
                }
            }
        }

        #endregion

        #region execute nonquery
        public int ExecuteNonQuery(DbTransaction transaction,
            string commandText, List<Param> parameters)
        {
            Validate(transaction, commandText);
            using (DbCommand cmd = transaction.Connection.CreateCommand())
            {
                this.PrepareCommand(cmd, commandText, transaction, parameters);
                return cmd.ExecuteNonQuery();
            }
        }

        public int ExecuteNonQuery(string commandText, List<Param> parameters)
        {
            Validate(commandText);
            using (DbConnection conn = this.GetDbConnection())
            {
                using (DbCommand cmd = conn.CreateCommand())
                {
                    this.PrepareCommand(cmd, commandText, parameters);
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }
        #endregion

        #region execute datatable
        
        public DataTable ExecuteDataTable(string commandText, List<Param> parameters)
        {
            this.Validate(commandText);
            //UpdateConnectionStr();
            DbConnection conn = this.GetDbConnection();
            using (conn)
            {
                using (DbCommand cmd = conn.CreateCommand())
                {
                    this.PrepareCommand(cmd, commandText, parameters);
                    return this.CreateDataTable(cmd);
                }
            }
        }

        public DataTable ExecuteDataTable(DbTransaction transaction,
            string commandText, List<Param> parameters)
        {
            Validate(transaction, commandText);
            using (DbCommand cmd = transaction.Connection.CreateCommand())
            {
                this.PrepareCommand(cmd, commandText, transaction, parameters);
                return this.CreateDataTable(cmd);
            }
        }

        private DataTable CreateDataTable(DbCommand cmd)
        {
            DbDataAdapter adapter = _factory.CreateDataAdapter();
            adapter.SelectCommand = cmd;
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }
        #endregion

        #region ExecuteReader
        
        public IDataReader ExecuteReader(string commandText, List<Param> parameters)
        {
            this.Validate(commandText);
            //UpdateConnectionStr();
            DbConnection conn = this.GetDbConnection();
            using (conn)
            {
                using (DbCommand cmd = conn.CreateCommand())
                {
                    this.PrepareCommand(cmd, commandText, parameters);
                    return cmd.ExecuteReader();
                }
            }
        }
        public IDataReader ExecuteReader(DbTransaction transaction,
            string commandText, List<Param> parameters)
        {
            Validate(transaction, commandText);
            using (DbCommand cmd = transaction.Connection.CreateCommand())
            {
                this.PrepareCommand(cmd, commandText, transaction, parameters);
                return cmd.ExecuteReader();
            }
        }


        #endregion

        public int getNewId(string tablename)
        {
            int id = 0;
            string sql = "select ifnull(max(id),0) from " + tablename + " where id>0";
            DataTable dt = this.ExecuteDataTable(sql, null);
            foreach (DataRow dr in dt.Rows)
            {
                id = Convert.ToInt32(dr[0]);
            }
            return id + 1;
        }


    }
}

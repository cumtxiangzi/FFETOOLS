using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;

namespace FFETOOLS
{
    public class SQLiteHelper
    {
        #region 字段

        /// <summary>
        /// 事务的基类
        /// </summary>
        private DbTransaction DBtrans;
        /// <summary>
        /// 数据库地址
        /// </summary>
        private readonly string DataFile;
        /// <summary>
        /// 数据库密码
        /// </summary>
        private readonly string PassWord;
        /// <summary>
        /// 数据库连接定义
        /// </summary>
        private SQLiteConnection SQLiteConnections;

        #endregion

        #region 构造函数

        /// <summary>
        /// 根据数据库地址初始化
        /// </summary>
        /// <param name="dataFile">数据库地址</param>
        public SQLiteHelper(string dataFile)
        {
            this.DataFile = dataFile ?? throw new ArgumentNullException("dataFile=null");
            this.DataFile = dataFile;
        }

        /// <summary>
        /// 使用密码打开数据库
        /// </summary>
        /// <param name="dataFile">数据库地址</param>
        /// <param name="passWord">数据库密码</param>
        public SQLiteHelper(string dataFile, string passWord)
        {
            this.DataFile = dataFile ?? throw new ArgumentNullException("dataFile is null");
            this.PassWord = passWord ?? throw new ArgumentNullException("PassWord is null");
            this.DataFile = dataFile;
        }

        #endregion

        #region 打开/关闭 数据库

        /// <summary>
        /// 打开数据库连接 
        /// </summary>
        /// <returns>返回true为打开成功，返回false为打开失败</returns>
        public bool Open()
        {
            if (string.IsNullOrWhiteSpace(PassWord))
            {
                SQLiteConnections = OpenConnection(this.DataFile);
            }
            else
            {
                SQLiteConnections = OpenConnection(this.DataFile, PassWord);
            }
            return SQLiteConnections != null;
        }

        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        /// <returns>返回true为关闭成功，返回false为关闭失败</returns>
        public bool Close()
        {
            if (this.SQLiteConnections != null)
            {
                try
                {
                    this.SQLiteConnections.Close();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>  
        /// 打开一个无密码的 SQLite 数据库
        /// </summary>  
        /// <param name="dataFile">数据库文件路径</param>  
        /// <returns> SQLiteConnection类 </returns>  
        private SQLiteConnection OpenConnection(string dataFile)
        {
            if (dataFile == null)
            {
                throw new ArgumentNullException("dataFiledataFile=null");
            }
            bool isExist = File.Exists(dataFile);
            if (!isExist)
            {
                MessageBox.Show("数据库文件为空，请检查文件是否存在");
                return null;
            }
            SQLiteConnection conn = new SQLiteConnection();
            SQLiteConnectionStringBuilder conStr = new SQLiteConnectionStringBuilder
            {
                DataSource = dataFile
            };
            conn.ConnectionString = conStr.ToString();
            conn.Open();
            return conn;
        }

        /// <summary>  
        /// 打开一个有密码的 SQLite 数据库
        /// </summary>  
        /// <param name="dataFile">数据库文件路径</param>  
        /// <param name="password">密码</param>
        /// <returns> SQLiteConnection类 </returns>  
        private SQLiteConnection OpenConnection(string dataFile, string password)
        {
            if (dataFile == null)
            {
                throw new ArgumentNullException("dataFile=null");
            }

            bool isExist = File.Exists(Convert.ToString(dataFile));
            if (!isExist)
            {
                MessageBox.Show("数据库文件为空，请检查文件是否存在");
                return null;
            }

            SQLiteConnection conn = new SQLiteConnection();
            SQLiteConnectionStringBuilder conStr = new SQLiteConnectionStringBuilder
            {
                DataSource = dataFile,
                Password = password,
                Version = 3,
            };
            conn.ConnectionString = conStr.ToString();
            conn.Open();
            return conn;
        }

        #endregion

        #region 创建数据库

        /// <summary>
        /// 创建一个无密码的，默认路径的SQLite数据库
        /// </summary>
        /// <returns>bool表示执行的结果，string表示错误信息，若执行成功，则为Empty</returns>
        public Tuple<bool, string> CreateNotPwdDataBase()
        {
            bool isExist = File.Exists(Convert.ToString(DataFile));
            if (isExist)
            {
                return Tuple.Create(false, "数据库文件已经存在，无需创建");
            }
            try
            {
                SQLiteConnection.CreateFile(DataFile);
                return Tuple.Create(true, string.Empty);
            }
            catch (SQLiteException ex)
            {
                return Tuple.Create(false, ex.Message);
            }
        }

        /// <summary>
        /// 创建一个无密码的，指定路径的SQLite数据库
        /// </summary>
        /// <param name="dataFile"></param>
        /// <returns>bool表示执行的结果，string表示错误信息，若执行成功，则为Empty</returns>
        public Tuple<bool, string> CreateNotPwdDataBase(string dataFile)
        {
            if (string.IsNullOrEmpty(dataFile))
            {
                return Tuple.Create(false, "参数dataFile为空");
            }
            bool isExist = File.Exists(Convert.ToString(dataFile));
            if (isExist)
            {
                return Tuple.Create(false, "数据库文件已经存在，无需创建");
            }
            try
            {
                SQLiteConnection.CreateFile(dataFile);
                return Tuple.Create(true, string.Empty);
            }
            catch (SQLiteException ex)
            {
                return Tuple.Create(false, ex.Message);
            }
        }

        /// <summary>
        /// 创建一个有密码的，默认路径的SQLite数据库 
        /// </summary>
        /// <param name="password">密码</param>
        /// <returns>bool表示执行的结果，string表示错误信息，若执行成功，则为Empty</returns>
        public Tuple<bool, string> CreatePwdDataBase(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return Tuple.Create(false, "参数password为空");
            }
            bool isExist = File.Exists(Convert.ToString(DataFile));
            if (isExist)
            {
                return Tuple.Create(false, "数据库文件已经存在，无需创建");
            }
            try
            {
                SQLiteConnection.CreateFile(DataFile);
                SQLiteConnection conn = new SQLiteConnection();
                SQLiteConnectionStringBuilder conStr = new SQLiteConnectionStringBuilder
                {
                    DataSource = DataFile,
                    Password = password,
                    Version = 3,
                };
                conn.ConnectionString = conStr.ToString();
                conn.Open();
                conn.ChangePassword(password);
                conn.Close();
                return Tuple.Create(true, string.Empty);
            }
            catch (SQLiteException ex)
            {
                return Tuple.Create(false, ex.Message);
            }
        }

        /// <summary>
        /// 创建一个有密码的，指定路径的SQLite数据库
        /// </summary>
        /// <param name="dataFile">文件路径</param>
        /// <param name="password">密码</param>
        /// <returns>bool表示执行的结果，string表示错误信息，若执行成功，则为Empty</returns>
        public Tuple<bool, string> CreatePwdDataBase(string dataFile, string password)
        {
            if (string.IsNullOrEmpty(dataFile))
            {
                return Tuple.Create(false, "参数dataFile为空");
            }
            if (string.IsNullOrEmpty(password))
            {
                return Tuple.Create(false, "参数password为空");
            }
            bool isExist = File.Exists(Convert.ToString(dataFile));
            if (isExist)
            {
                return Tuple.Create(false, "数据库文件已经存在，无需创建");
            }
            try
            {
                SQLiteConnection.CreateFile(dataFile);
                SQLiteConnection conn = new SQLiteConnection();
                SQLiteConnectionStringBuilder conStr = new SQLiteConnectionStringBuilder
                {
                    DataSource = dataFile,
                    Password = password,
                    Version = 3,
                };
                conn.ConnectionString = conStr.ToString();
                conn.Open();
                conn.ChangePassword(password);
                conn.Close();
                return Tuple.Create(true, string.Empty);
            }
            catch (SQLiteException ex)
            {
                return Tuple.Create(false, ex.Message);
            }
        }

        #endregion

        #region 执行SQL

        /// <summary>
        /// 执行SQL, 并返回 SQLiteDataReader 对象结果 
        /// </summary>  
        /// <param name="sql">SQL语句</param>
        /// <param name="paramArr">null 表示无参数</param>
        /// <returns>
        /// 参数1：bool 表示是否执行成功，
        /// 参数2：SQLiteDataReader 表示返回结果，
        /// 参数3：string 表示错误信息，
        /// </returns>  
        public Tuple<bool, SQLiteDataReader, string> ExecuteReader(string sql, SQLiteParameter[] paramArr)
        {
            if (string.IsNullOrEmpty(sql))
            {
                return new Tuple<bool, SQLiteDataReader, string>(false, null, "参数 sql 为空");
            }
            int openState = EnsureConnection();
            if (openState == -1)
            {
                return new Tuple<bool, SQLiteDataReader, string>(false, null, "请先打开或者初始化数据库");
            }
            using (SQLiteCommand cmd = new SQLiteCommand(sql, Connection))
            {
                if (paramArr != null)
                {
                    cmd.Parameters.AddRange(paramArr);
                }
                try
                {
                    SQLiteDataReader dataReader = cmd.ExecuteReader();
                    cmd.Parameters.Clear();
                    return Tuple.Create(true, dataReader, string.Empty);
                }
                catch (SQLiteException ex)
                {
                    return new Tuple<bool, SQLiteDataReader, string>(false, null, ex.Message);
                }
            }
        }

        /// <summary>
        /// 执行查询，并返回 DataSet 对象
        /// </summary>
        /// <param name="sql">SQL查询语句</param>
        /// <param name="paramArr">参数数组</param>
        /// <returns>
        /// 参数1：bool 表示是否执行成功，
        /// 参数2：DataSet 表示返回结果，
        /// 参数3：string 表示错误信息
        /// </returns>
        public Tuple<bool, DataSet, string> ExecuteDataSet(string sql, SQLiteParameter[] paramArr)
        {
            if (string.IsNullOrEmpty(sql))
            {
                return new Tuple<bool, DataSet, string>(false, null, "参数sql为空");
            }
            int openState = EnsureConnection();
            if (openState == -1)
            {
                return new Tuple<bool, DataSet, string>(false, null, "请先打开或者初始化数据库");
            }
            using (SQLiteCommand cmd = new SQLiteCommand(sql, this.Connection))
            {
                if (paramArr != null)
                {
                    cmd.Parameters.AddRange(paramArr);
                }
                try
                {
                    SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter();
                    DataSet dataSet = new DataSet();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataSet);
                    cmd.Parameters.Clear();
                    dataAdapter.Dispose();
                    return Tuple.Create(true, dataSet, string.Empty);
                }
                catch (SQLiteException ex)
                {
                    return new Tuple<bool, DataSet, string>(false, null, ex.Message);
                }
            }
        }

        /// <summary>
        /// 执行SQL查询，并返回 DataSet 对象。
        /// </summary>
        /// <param name="strTable">映射源表的名称</param>
        /// <param name="sql">SQL语句</param>
        /// <param name="paramArr">SQL参数数组</param>
        /// <returns>
        /// 参数1：bool 表示是否执行成功，
        /// 参数2：DataSet 表示返回 DataSet 结果，
        /// 参数3：string 表示错误信息
        /// </returns>
        public Tuple<bool, DataSet, string> ExecuteDataSet(string strTable, string sql, SQLiteParameter[] paramArr)
        {
            if (string.IsNullOrEmpty(sql))
            {
                return new Tuple<bool, DataSet, string>(false, null, "参数sql为空");
            }
            int openState = EnsureConnection();
            if (openState == -1)
            {
                return new Tuple<bool, DataSet, string>(false, null, "请先打开或者初始化数据库");
            }
            using (SQLiteCommand cmd = new SQLiteCommand(sql, this.Connection))
            {
                if (paramArr != null)
                {
                    cmd.Parameters.AddRange(paramArr);
                }
                try
                {
                    SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter();
                    DataSet dataSet = new DataSet();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataSet, strTable);
                    cmd.Parameters.Clear();
                    dataAdapter.Dispose();
                    return Tuple.Create(true, dataSet, string.Empty);
                }
                catch (SQLiteException ex)
                {
                    return new Tuple<bool, DataSet, string>(false, null, ex.Message);
                }
            }
        }

        /// <summary>  
        /// 执行SQL
        /// </summary>  
        /// <param name="sql">SQL语句</param>  
        /// <param name="paramArr">参数数组</param>
        /// <returns>
        /// 参数1：bool 表示是否执行成功，
        /// 参数2：int 表示受影响的行数,
        /// 参数3：string 表示错误信息
        /// </returns>  
        public Tuple<bool, int, string> ExecuteNonQuery(string sql, SQLiteParameter[] paramArr)
        {
            if (string.IsNullOrEmpty(sql))
            {
                return new Tuple<bool, int, string>(false, 0, "参数sql为空");
            }
            int openState = EnsureConnection();
            if (openState == -1)
            {
                return new Tuple<bool, int, string>(false, 0, "请先打开或者初始化数据库");
            }
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, Connection))
                {
                    if (paramArr != null)
                    {
                        foreach (SQLiteParameter p in paramArr)
                        {
                            cmd.Parameters.Add(p);
                        }
                    }
                    int line = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    return Tuple.Create(true, line, string.Empty);
                }
            }
            catch (SQLiteException ex)
            {
                return Tuple.Create(false, 0, ex.Message);
            }
        }

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>
        /// 参数1：bool 表示是否执行成功，
        /// 参数2：int 表示受影响的行数，
        /// 参数3：string 表示错误信息
        /// </returns>
        public Tuple<bool, int, string> ExecuteScalar(string sql)
        {
            if (string.IsNullOrEmpty(sql))
            {
                return new Tuple<bool, int, string>(false, 0, "参数sql为空");
            }
            int openState = EnsureConnection();
            if (openState == -1)
            {
                return new Tuple<bool, int, string>(false, 0, "请先打开或者初始化数据库");
            }
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, Connection))
                {
                    int line = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    return Tuple.Create(true, line, string.Empty);
                }
            }
            catch (SQLiteException ex)
            {
                return Tuple.Create(false, 0, ex.Message);
            }
        }

        /// <summary>  
        /// 执行SQL，返回第一行第一列
        /// </summary>  
        /// <param name="sql">SQL语句</param>  
        /// <param name="paramArr">参数数组</param>  
        /// <returns>
        /// 参数1：bool 表示是否执行成功，
        /// 参数2：object 执行sql的结果：第一行第一列,
        /// 参数3：string 表示错误信息
        /// </returns>  
        public Tuple<bool, object, string> ExecuteScalar(string sql, SQLiteParameter[] paramArr)
        {
            if (string.IsNullOrEmpty(sql))
            {
                return new Tuple<bool, object, string>(false, "参数sql为空", null);
            }
            int openState = EnsureConnection();
            if (openState == -1)
            {
                return new Tuple<bool, object, string>(false, "请先打开或者初始化数据库", null);
            }
            using (SQLiteCommand cmd = new SQLiteCommand(sql, Connection))
            {
                if (paramArr != null)
                {
                    cmd.Parameters.AddRange(paramArr);
                }
                try
                {
                    object reader = cmd.ExecuteScalar();
                    cmd.Parameters.Clear();
                    cmd.Dispose();
                    return Tuple.Create(true, reader, string.Empty);
                }
                catch (SQLiteException ex)
                {
                    return new Tuple<bool, object, string>(false, ex.Message, null);
                }
            }
        }

        #endregion

        #region 增 删 改

        /// <summary>  
        /// 执行 insert into 语句 
        /// </summary>  
        /// <param name="table">表名</param>  
        /// <param name="entity">要插入的数据字典</param>  
        /// <returns>
        /// 参数1：bool 表示是否执行成功，
        /// 参数2：int 表示受影响的行数，
        /// 参数3：string 表示错误信息
        /// </returns>  
        public Tuple<bool, int, string> InsertData(string table, Dictionary<string, object> entity)
        {
            if (string.IsNullOrEmpty(table))
            {
                return new Tuple<bool, int, string>(false, 0, "参数table为空");
            }
            int openState = EnsureConnection();
            if (openState == -1)
            {
                return new Tuple<bool, int, string>(false, 0, "请先打开或者初始化数据库");
            }
            string sql = BuildInsert(table, entity);
            return ExecuteNonQuery(sql, BuildParamArray(entity));
        }

        /// <summary>  
        /// 执行 update 语句
        /// </summary>  
        /// <param name="table">表名</param>  
        /// <param name="entity">要修改的列名和列名的值</param>  
        /// <param name="where">查找符合条件的列</param>  
        /// <param name="whereParams">where条件中参数的值</param>  
        /// <returns>
        /// 参数1：bool 表示是否执行成功，
        /// 参数2：int 表示受影响的行数，
        /// 参数3：string 表示错误信息
        /// </returns>  
        public Tuple<bool, int, string> Update(string table, Dictionary<string, object> entity, string where, SQLiteParameter[] whereParams)
        {
            if (string.IsNullOrEmpty(table))
            {
                return new Tuple<bool, int, string>(false, 0, "参数table为空");
            }
            int openState = EnsureConnection();
            if (openState == -1)
            {
                return new Tuple<bool, int, string>(false, 0, "请先打开或者初始化数据库");
            }
            string sql = BuildUpdate(table, entity);
            SQLiteParameter[] parameter = BuildParamArray(entity);
            if (where != null)
            {
                sql += " where " + where;
                if (whereParams != null)
                {
                    SQLiteParameter[] newArr = new SQLiteParameter[(parameter.Length + whereParams.Length)];
                    Array.Copy(parameter, newArr, parameter.Length);
                    Array.Copy(whereParams, 0, newArr, parameter.Length, whereParams.Length);
                    parameter = newArr;
                }
            }
            return ExecuteNonQuery(sql, parameter);
        }

        /// <summary>  
        /// 执行 delete 语句 
        /// </summary>  
        /// <param name="table">表名</param>  
        /// <param name="where">where条件语句</param>  
        /// <param name="whereParams">where内参数数组</param>  
        /// <returns>
        /// 参数1：bool 表示是否执行成功，
        /// 参数3：int 表示受影响的行数，
        /// 参数2：string 表示错误信息
        /// </returns>  
        public Tuple<bool, int, string> Delete(string table, string where, SQLiteParameter[] whereParams)
        {
            if (string.IsNullOrEmpty(table))
            {
                return new Tuple<bool, int, string>(false, 0, "参数table为空");
            }
            int openState = EnsureConnection();
            if (openState == -1)
            {
                return new Tuple<bool, int, string>(false, 0, "请先打开或者初始化数据库");
            }
            string sql = "delete from " + table + " ";
            if (where != null)
            {
                sql += "where " + where;
            }
            return ExecuteNonQuery(sql, whereParams);
        }

        #endregion

        #region 事务

        /// <summary>
        /// 开始事务
        /// </summary>
        /// <returns>返回true表示成功，返回false表示失败</returns>
        public bool BeginTrain()
        {
            int openState = EnsureConnection();
            if (openState == -1)
            {
                MessageBox.Show("请先打开或者初始化数据库");
                return false;
            }
            DBtrans = SQLiteConnections.BeginTransaction();
            return true;
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public void DBCommit()
        {
            try
            {
                DBtrans.Commit();
            }
            catch (Exception)
            {
                DBtrans.Rollback();
            }
        }

        #endregion

        #region 工具

        /// <summary>  
        /// 读取 或 设置 SQLiteManager 使用的数据库连接  
        /// </summary>  
        public SQLiteConnection Connection
        {
            get
            {
                return SQLiteConnections;
            }
            private set
            {
                SQLiteConnections = value ?? throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 确保数据库是连接状态
        /// </summary>
        /// <returns>
        /// 返回1表示成功打开当前数据库，
        /// 返回2表示数据库处于打开状态，无需再次打开
        /// 返回-1表示SQLiteConnections没有初始化，
        /// </returns>
        private int EnsureConnection()
        {
            if (this.SQLiteConnections == null)
            {
                //throw new Exception("SQLiteManager.Connection=null");
                return -1;
            }
            if (SQLiteConnections.State != ConnectionState.Open)
            {
                SQLiteConnections.Open();
                return 1;
            }
            return 2;
        }

        /// <summary>
        /// 获取数据库文件的路径
        /// </summary>
        /// <returns>文件的路径</returns>
        public string GetDataFile()
        {
            return this.DataFile;
        }

        /// <summary>  
        /// 判断表 table 是否存在  
        /// </summary>  
        /// <param name="table"></param>  
        /// <returns>存在返回true，否则返回false</returns>  
        public bool TableExists(string table)
        {
            if (string.IsNullOrEmpty(table))
            {
                MessageBox.Show("参数table不能为空");
                return false;
            }
            int openState = EnsureConnection();
            if (openState == -1)
            {
                MessageBox.Show("请先打开或者初始化数据库");
                return false;
            }

            Tuple<bool, SQLiteDataReader, string> tuple =
                ExecuteReader("SELECT count(*) as c FROM sqlite_master WHERE type='table' AND name=@tableName ",
                new SQLiteParameter[] { new SQLiteParameter("tableName", table) });

            if (!tuple.Item1)
            {
                return false;
            }
            SQLiteDataReader reader = tuple.Item2;
            if (reader == null)
            {
                return false;
            }
            reader.Read();
            int result = reader.GetInt32(0);
            reader.Close();
            reader.Dispose();
            return result == 1;
        }

        /// <summary>
        /// VACUUM 命令（通过复制主数据库中的内容到一个临时数据库文件，然后清空主数据库，并从副本中重新载入原始的数据库文件）
        /// </summary>
        /// <returns></returns>
        public bool Vacuum()
        {
            try
            {
                using (SQLiteCommand Command = new SQLiteCommand("VACUUM", Connection))
                {
                    Command.ExecuteNonQuery();
                }
                return true;
            }
            catch (System.Data.SQLite.SQLiteException)
            {
                return false;
            }
        }

        /// <summary>
        /// 将 Dictionary 类型数据 转换为 SQLiteParameter[] 类型
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>SQLiteParameter[]</returns>
        private SQLiteParameter[] BuildParamArray(Dictionary<string, object> entity)
        {
            List<SQLiteParameter> list = new List<SQLiteParameter>();
            foreach (string key in entity.Keys)
            {
                list.Add(new SQLiteParameter(key, entity[key]));
            }
            if (list.Count == 0)
            {
                return null;
            }
            return list.ToArray();
        }

        /// <summary>
        /// 将 Dictionary 类型数据 转换为 插入数据 的 SQL语句
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="entity">字典</param>
        /// <returns>SQL语句</returns>
        private string BuildInsert(string table, Dictionary<string, object> entity)
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("insert into ").Append(table);
            buf.Append(" (");
            foreach (string key in entity.Keys)
            {
                buf.Append(key).Append(",");
            }
            buf.Remove(buf.Length - 1, 1); // 移除最后一个,
            buf.Append(") ");
            buf.Append("values(");
            foreach (string key in entity.Keys)
            {
                buf.Append("@").Append(key).Append(","); // 创建一个参数
            }
            buf.Remove(buf.Length - 1, 1);
            buf.Append(") ");

            return buf.ToString();
        }

        /// <summary>
        /// 将 Dictionary 类型数据 转换为 修改数据 的 SQL语句
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="entity">字典</param>
        /// <returns>SQL语句</returns>
        private string BuildUpdate(string table, Dictionary<string, object> entity)
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("update ").Append(table).Append(" set ");
            foreach (string key in entity.Keys)
            {
                buf.Append(key).Append("=").Append("@").Append(key).Append(",");
            }
            buf.Remove(buf.Length - 1, 1);
            buf.Append(" ");
            return buf.ToString();
        }

        #endregion
    }
    public class DbOperator
    {
        private SQLiteConnection _con;

        /// <summary>
        /// 文件路径
        /// </summary>
        public string DbFilePath { get; set; }

        /// <summary>
        /// 旧密码
        /// </summary>
        public string PwdOriginal { get; set; }

        /// <summary>
        /// 新密码
        /// </summary>
        public string PwdNew { get; set; }

        /// <summary>
        /// 修改密码
        /// </summary>
        public void ChangePassword()
        {
            this.ChangePassword(PwdNew);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="newPassword">新密码</param>
        public void ChangePassword(string newPassword)
        {
            string msg = "恭喜，修改密码成功!";
            bool sucess = true;

            _con = new SQLiteConnection();
            _con.ConnectionString = "Data Source=" + this.DbFilePath;
            if (this.PwdOriginal.Length > 0)
            {
                _con.ConnectionString += ";Password=" + this.PwdOriginal;
            }
            try
            {
                _con.Open();
                _con.SetPassword(newPassword);
                _con.ChangePassword(newPassword);//打开加密文件使用 _con.SetPassword(newPassword); 
                _con.Close();
            }
            catch (Exception ex)
            {
                //throw new Exception("无法连接到数据库!" + ex.Message);
                msg = string.Format("修改失败:\n\n({0})!", ex.Message);
                sucess = false;

            }

            if (sucess)
            {
                MessageBox.Show(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }

}

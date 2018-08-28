using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;
using System.Reflection;
using BaseEnvironment;
namespace CommonTool.DBHelper
{
    /// <summary>
    /// DapperCURD帮助类
    /// </summary>
    public class DapperHelper
    {
        private string _connectionKey;
        /// <summary>
        /// 赋值数据库连接字符串
        /// </summary>
        /// <param name="SqlConnectionKey"></param>
        public DapperHelper(string SqlConnectionKey)
        {
            _connectionKey = SqlConnectionKey;
        }


        /// <summary>
        /// 获得默认数据库的链接实例
        /// </summary>
        /// <returns></returns>
        public static DapperHelper CreateInstance()
        {
            return new DapperHelper(BaseApplication.DefaultSqlConnectionKey);
        }

        /// <summary>
        /// 获得实例
        /// </summary>
        /// <param name="SqlConnectionKey"></param>
        /// <returns></returns>
        public static DapperHelper CreateInstance(string SqlConnectionKey)
        {
            return new DapperHelper(SqlConnectionKey);
        }



        /// <summary>
        /// 删除单条数据
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="dicParam"></param>
        /// <returns></returns>
        public int DeleteSigerData(string TableName, Dictionary<string, Object> dicParam)
        {
            string Params = "";
            foreach (var item in dicParam)
            {
                Params += "@" + item.Key + "=" + item.Key + ",";
            }
            Params = Params.Substring(0, Params.Length - 1);
            string sql = string.Format("delete from {0} where {1}", TableName, Params);
            int result;
            using (IDbConnection dbConnection = new SqlConnection(_connectionKey))
            {
                result = dbConnection.Execute(sql, dicParam);
            }
            return result;
        }
        /// <summary>
        /// 删除多条数据
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="dicParam"></param>
        /// <returns></returns>
        public int DeleteManyData(string TableName, string ObjectIdValues)
        {
            string whereStr = "";
            foreach (var item in ObjectIdValues.Split(','))
            {
                whereStr += "'" + item + "',";
            }
            whereStr = whereStr.Substring(0, whereStr.Length - 1);
            string sql = string.Format("delete from {0} where ObjectId in ({1}) ", TableName, whereStr);
            int result;
            using (IDbConnection dbConnection = new SqlConnection(_connectionKey))
            {
                result = dbConnection.Execute(sql);
            }
            return result;
        }


        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="TableName">目标表名</param>
        /// <param name="FiledsName">需要插入的字段名称。例：ProductName,ProductDesc,UserID,CreateTime </param>
        /// <param name="arrayList">需要插入的数据。已集合或是数组的形式</param>
        /// <returns>影响的行数</returns>
        public int InsertBatch(string TableName, string FiledsName, object arrayList)
        {
            string Params = "";
            string[] FiledsArr = FiledsName.Split(',');
            foreach (var item in FiledsArr)
            {
                Params += "@" + item + ",";
            }
            Params = Params.Substring(0, Params.Length - 1);
            string sql = string.Format("Insert into {0}({1}) values({2})", TableName, FiledsName, Params);
            int result;
            using (IDbConnection dbConnection = new SqlConnection(_connectionKey))
            {
                result = dbConnection.Execute(sql, arrayList);
            }
            return result;
        }

        /// <summary>
        /// 批量插入数据,无需特殊处理数据
        /// </summary>
        /// <typeparam name="T">对应表的Model类</typeparam>
        /// <param name="TableName">对应表名</param>
        /// <param name="list">对应需要插入的数据</param>
        /// <returns>影响的行数</returns>
        public int InsertBatch<T>(string TableName, List<T> list)
        {
            string path = "Domain." + TableName + ",Domain";
            Type type = Type.GetType(path);
            object modelObj = Activator.CreateInstance(type);
            string Columns = "";
            string Params = "";
            PropertyInfo[] props = type.GetProperties();
            foreach (var item in props)
            {
                Columns += item.Name + ",";
                Params += "@" + item.Name + ",";
            }
            Columns = Columns.Substring(0, Columns.Length - 1);
            Params = Params.Substring(0, Params.Length - 1);
            string sql = string.Format("insert into " + TableName + "({0}) values ({1})", Columns, Params);
            int result;
            using (IDbConnection dbConnection = new SqlConnection(_connectionKey))
            {
                result = dbConnection.Execute(sql, list);
            }
            return result;
        }

        /// <summary>
        /// 跟新表数据
        /// </summary>
        /// <param name="Data">需要更新的数据</param>
        /// <param name="Param">Where条件数据</param>
        /// <param name="TableName">表名</param>
        /// <returns>影响的行数</returns>
        public int UpdateTable(Dictionary<string, object> Data, Dictionary<string, object> Param, string TableName)
        {
            string sql = "update " + TableName + " set ";
            foreach (var dataValue in Data)
            {
                sql += dataValue.Key + "=" + dataValue.Value + ", ";

            }
            sql = sql.Substring(0, sql.Length - 2);
            if (Param != null)
            {
                sql += " where ";

                foreach (var paramValue in Param)
                {

                    sql += paramValue.Key + "=" + paramValue.Value + "and ";

                }

                sql = sql.Substring(0, sql.Length - 4);
            }

            int result;
            using (IDbConnection dbConnection = new SqlConnection(_connectionKey))
            {
                result = dbConnection.Execute(sql);

            }
            return result;
        }


        /// <summary>
        /// 更新表数据
        /// </summary>
        /// <param name="Data">需要更新的数据</param>
        /// <param name="Param">Where条件数据</param>
        /// <param name="TableName">表名</param>
        /// <returns>影响的行数</returns>
        public int UpdateTable<T>(Dictionary<string, object> Data, Dictionary<string, object> Param, string TableName, T model)
        {
            string sql = "update " + TableName + " set ";
            foreach (var dataValue in Data)
            {
                sql += dataValue.Key + "=@" + dataValue.Key + ", ";

            }
            sql = sql.Substring(0, sql.Length - 2);
            if (Param != null)
            {
                sql += " where ";

                foreach (var paramValue in Param)
                {

                    sql += paramValue.Key + "=@" + paramValue.Key + "and ";

                }

                sql = sql.Substring(0, sql.Length - 4);
            }

            int result;
            using (IDbConnection dbConnection = new SqlConnection(_connectionKey))
            {
                result = dbConnection.Execute(sql, model);

            }
            return result;
        }


        /// <summary>
        /// 简单单表查询(不涉及注外键关系表)
        /// </summary>
        /// <typeparam name="T">对应表的Model</typeparam>
        /// <param name="sql"></param>
        /// <param name="whereObj">dapperQuery参数</param>
        /// <returns></returns>
        public List<T> SimpleQuery<T>(string sql)
        {
            List<T> list = new List<T>();
            using (IDbConnection dbConnection = new SqlConnection(_connectionKey))
            {
                list = (List<T>)dbConnection.Query<T>(sql);
            }
            return list;
        }




        /// <summary>
        /// 简单单表查询(不涉及注外键关系表)
        /// </summary>
        /// <typeparam name="T">对应表的Model</typeparam>
        /// <param name="sql"></param>
        /// <param name="whereObj">dapperQuery参数</param>
        /// <returns></returns>
        public List<T> SimpleQuery<T>(string sql, object whereObj = null)
        {
            List<T> list = new List<T>();
            using (IDbConnection dbConnection = new SqlConnection(_connectionKey))
            {
                list = (List<T>)dbConnection.Query<T>(sql, whereObj);
            }
            return list;
        }

        /// <summary>
        /// 动态获取
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="whereObj"></param>
        /// <returns></returns>
        public List<dynamic> SimpleQuery(string sql, object whereObj = null)
        {
            List<dynamic> list = new List<dynamic>();
            using (IDbConnection dbConnection = new SqlConnection(_connectionKey))
            {
                list = (List<dynamic>)dbConnection.Query<dynamic>(sql, whereObj);
            }
            return list;
        }

        /// <summary>
        /// 一对一联合查询
        /// </summary>
        /// <typeparam name="T">待输出的对象Model</typeparam>
        /// <typeparam name="M">被包含的对象Model</typeparam>
        /// <param name="sql"></param>
        /// <param name="splitOn">分割字段</param>
        /// <returns></returns>
        public List<T> OneToOneQuery<T, M>(string sql, string splitOn)
        {

            List<T> list = new List<T>();
            using (IDbConnection dbConnection = new SqlConnection(_connectionKey))
            {
                list = (List<T>)dbConnection.Query<T, M, T>(sql, (t, m) =>
                {
                    PropertyInfo[] props = typeof(T).GetProperties();
                    foreach (var item in props)
                    {
                        if (item.PropertyType.Name.Equals(m.GetType().Name))
                        {
                            item.SetValue(t, m, null);
                        }
                    }
                    return t;
                }, splitOn: splitOn);
            }
            return list;
        }


        /// <summary>
        /// 一对多联合查询
        /// </summary>
        /// <typeparam name="T">待输出的对象</typeparam>
        /// <typeparam name="M">被包含的对象</typeparam>
        /// <param name="sql"></param>
        /// <param name="splitOn">分割字段</param>
        /// <param name="primaryKeyName">需要做判断重复值的属性值名称，一般为该表的主键名称</param>
        /// <returns></returns>
        public List<T> OneToManyQuery<T, M>(string sql, string splitOn, string primaryKeyName)
        {
            Dictionary<object, T> lookUp = new Dictionary<object, T>();
            Dictionary<object, List<M>> lookUpMList = new Dictionary<object, List<M>>();
            List<T> tList = new List<T>();
            List<M> mList = new List<M>();
            List<T> resultList = new List<T>();
            using (IDbConnection dbConnection = new SqlConnection(_connectionKey))
            {
                tList = dbConnection.Query<T, M, T>(sql, (t, m) =>
                {
                    PropertyInfo[] props = typeof(T).GetProperties();
                    object primaryKeyObj = null;
                    foreach (var item in props)
                    {

                        if (item.Name.Equals(primaryKeyName))
                        {

                            if (lookUp.ContainsKey(item.GetValue(t)))
                            {
                                primaryKeyObj = item.GetValue(t);
                                lookUp.Remove(primaryKeyObj);
                                lookUp.Add(item.GetValue(t), t);
                            }
                            else
                            {
                                primaryKeyObj = item.GetValue(t);
                                lookUp.Add(item.GetValue(t), t);
                            }
                            if (lookUpMList.ContainsKey(item.GetValue(t)))
                            {
                                List<M> tempMList = lookUpMList[item.GetValue(t)];
                                tempMList.Add(m);
                                lookUpMList.Remove(item.GetValue(t));
                                lookUpMList.Add(item.GetValue(t), tempMList);
                            }
                            else
                            {
                                List<M> tempMList = new List<M>();
                                tempMList.Add(m);
                                lookUpMList.Add(item.GetValue(t), tempMList);
                            }
                        }
                        if (item.PropertyType.Name.Equals(mList.GetType().Name))
                        {
                            foreach (var itemLookUpMList in lookUpMList)
                            {
                                if (itemLookUpMList.Key.Equals(primaryKeyObj))
                                {
                                    item.SetValue(t, itemLookUpMList.Value, null);
                                    lookUp.Remove(primaryKeyObj);
                                    lookUp.Add(primaryKeyObj, t);
                                }
                            }
                        }
                    }
                    return t;
                }, splitOn: splitOn).ToList();
            }
            resultList.AddRange(lookUp.Values);
            return resultList;
        }


        /// <summary>
        /// 简单事务
        /// </summary>
        /// <param name="listStr">所需要执行的sql集合</param>
        /// <returns>受影响的行数</returns>
        public int TransAction(List<string> listStr)
        {

            using (IDbConnection dbConnection = new SqlConnection(_connectionKey))
            {
                dbConnection.Open();
                IDbTransaction transaction = dbConnection.BeginTransaction();
                int result = 0;
                try
                {

                    for (int i = 0; i < listStr.Count; i++)
                    {
                        result += dbConnection.Execute(listStr[i], null, transaction);
                    }
                    transaction.Commit();
                }
                catch (Exception exception)
                {

                    transaction.Rollback();

                }

                return result;
            }

        }

    }
}
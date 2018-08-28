using BaseEnvironment;
using CommonTool.DBHelper;
using IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    /// <summary>
    /// Author : 谭振
    /// DateTime : 2017/3/24 16:51:07
    
    /// Description : 
    /// </summary>
    public class BaseService : IBaseService
    {
        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataList"></param>
        /// <returns></returns>
        public int InsertBatch<T>(object dataList, string DbConnectStr = "")
        {
            if (DbConnectStr == "")
            {
                return DapperHelper.CreateInstance(BaseApplication.DefaultSqlConnectionKey).InsertBatch<T>(typeof(T).Name, (List<T>)dataList);
            }
            else
            {
                return DapperHelper.CreateInstance(DbConnectStr).InsertBatch<T>(typeof(T).Name, (List<T>)dataList);
            }

        }
    }
}

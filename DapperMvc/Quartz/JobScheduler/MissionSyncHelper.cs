using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;



    /// <summary>
    /// 定时任务帮助类
    /// </summary>
    public class MissionSyncHelper
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(MissionSyncHelper));
        public MissionSyncHelper()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //

        }

        /// <summary>
        /// 应用程序启动时从新启动已开启的任务
        /// </summary>
        public static void StartPlan()
        {
            try
            {
                string sql = @"SELECT  [MissionCode]
      ,[MissionName]
      ,[SyncTime]
      ,[SyncSuccessTime]
      ,[IsAyncSuccess]
      ,[JobClassName]
      ,[State]
      ,[CronExplain],[SyncSuccessBeginTime],[HostName] FROM [H3Cloud].[dbo].[ZD_SyncTaskConfig] where State=1 and HostName='" + Dns.GetHostName() + "'";
                OThinker.Data.Database.CommandFactory factory = OThinker.H3.WorkSheet.AppUtility.Engine.EngineConfig.CommandFactory;
                OThinker.Data.Database.ICommand command = factory.CreateCommand();
                var dataTable = command.ExecuteDataTable(sql);

                if (dataTable.Rows.Count > 0)
                {
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        ZD_SyncTaskConfig result = new ZD_SyncTaskConfig();
                        result.MissionCode = dataTable.Rows[i]["MissionCode"] + string.Empty;
                        result.MissionName = dataTable.Rows[i]["MissionName"] + string.Empty;
                        result.SyncSuccessTime = Convert.ToDateTime(dataTable.Rows[i]["SyncSuccessTime"]);
                        result.SyncTime = Convert.ToDateTime(dataTable.Rows[i]["SyncTime"]);
                        result.SyncSuccessBeginTime = Convert.ToDateTime(dataTable.Rows[i]["SyncSuccessBeginTime"]);
                        result.IsAyncSuccess = Convert.ToInt32(dataTable.Rows[i]["IsAyncSuccess"]);
                        result.State = 1;
                        result.JobClassName = dataTable.Rows[i]["JobClassName"] + string.Empty;
                        result.CronExplain = dataTable.Rows[i]["CronExplain"] + string.Empty;
                        result.HostName = dataTable.Rows[i]["HostName"] + string.Empty;
                        OThinker.H3.Portal.JobScheduler.Start(result);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.InfoFormat(e.Message.ToString());
                throw;
            }



        }


        /// <summary>
        /// 通过任务编码获得任务信息
        /// </summary>
        /// <param name="missionCode"></param>
        /// <returns></returns>
        public static ZD_SyncTaskConfig GetTaskInfoByCode(string missionCode)
        {

            string sql = @"SELECT  [MissionCode]
      ,[MissionName]
      ,[SyncTime]
      ,[SyncSuccessTime]
      ,[IsAyncSuccess]
      ,[JobClassName]
      ,[State]
      ,[CronExplain],[SyncSuccessBeginTime],[HostName] FROM [H3Cloud].[dbo].[ZD_SyncTaskConfig] where MissionCode='" + missionCode + "'";
            OThinker.Data.Database.CommandFactory factory = OThinker.H3.WorkSheet.AppUtility.Engine.EngineConfig.CommandFactory;
            OThinker.Data.Database.ICommand command = factory.CreateCommand();
            var dataTable = command.ExecuteDataTable(sql);
            ZD_SyncTaskConfig result = new ZD_SyncTaskConfig();
            if (dataTable.Rows.Count > 0)
            {
                result.MissionCode = missionCode;
                result.MissionName = dataTable.Rows[0]["MissionName"] + string.Empty;
                result.SyncSuccessTime = Convert.ToDateTime(dataTable.Rows[0]["SyncSuccessTime"]);
                result.SyncTime = Convert.ToDateTime(dataTable.Rows[0]["SyncTime"]);
                result.SyncSuccessBeginTime = Convert.ToDateTime(dataTable.Rows[0]["SyncSuccessBeginTime"]);
                result.IsAyncSuccess = Convert.ToInt32(dataTable.Rows[0]["IsAyncSuccess"]);
                result.State = Convert.ToInt32(dataTable.Rows[0]["State"]);
                result.JobClassName = dataTable.Rows[0]["JobClassName"] + string.Empty;
                result.CronExplain = dataTable.Rows[0]["CronExplain"] + string.Empty;
                result.HostName = dataTable.Rows[0]["HostName"] + string.Empty;
            }
            else
            {
                result = null;
            }
            return result;
        }

        public static bool AddMissionPlan(ZD_SyncTaskConfig data)
        {
            List<ZD_SyncTaskConfig> list = new List<ZD_SyncTaskConfig>();
            list.Add(data);
            string result = InvestHelper.InsertTableBatch<ZD_SyncTaskConfig>(new JavaScriptSerializer().Serialize(list), "ZD_SyncTaskConfig");
            if (result.IndexOf("true") > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool UpdateMissionPlan(ZD_SyncTaskConfig model)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("MissionCode", "'" + model.MissionCode + "'");
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("MissionName", "'" + model.MissionName + "'");
            data.Add("SyncSuccessTime", "'" + model.SyncSuccessTime + "'");
            data.Add("SyncTime", "'" + model.SyncTime + "'");
            data.Add("JobClassName", "'" + model.JobClassName + "'");
            data.Add("State", model.State);
            data.Add("IsAyncSuccess", model.IsAyncSuccess);
            data.Add("CronExplain", "'" + model.CronExplain + "'");
            data.Add("HostName", "'" + model.HostName + "'");
            data.Add("SyncSuccessBeginTime", "'" + model.SyncSuccessBeginTime + "'");
            string result = InvestHelper.UpdateTable(data, param, "ZD_SyncTaskConfig");
            if (result.IndexOf("true") > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #region 任务执行成功更新状态
        public static bool PlanSuccessUpdate(ZD_SyncTaskConfig model)
        {
            model.IsAyncSuccess = 1;//成功
            model.SyncTime = DateTime.Now;
            model.SyncSuccessTime = DateTime.Now;
            bool flag = UpdateMissionPlan(model);
            if (flag)
            {
                _logger.InfoFormat(model.MissionName + "=============成功执行时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "=============");
                return true;
            }
            else
            {
                _logger.InfoFormat(model.MissionName + "=============更新状态失败=============");
                return false;
            }
        }
        #endregion

        #region 任务执行失败更新状态
        public static bool PlanFailUpdate(ZD_SyncTaskConfig model)
        {
            model.IsAyncSuccess = 0;//失败
            model.SyncTime = DateTime.Now;
            bool flag = UpdateMissionPlan(model);
            if (flag)
            {
                _logger.InfoFormat(model.MissionName + "=============执行失败=============");
                return true;
            }
            else
            {
                _logger.InfoFormat(model.MissionName + "=============更新状态失败=============");
                return false;
            }
        }
        #endregion

    }

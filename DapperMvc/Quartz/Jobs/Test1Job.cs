using log4net;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

    /// <summary>
    /// Test1Job 的摘要说明
    /// </summary>
    public class Test1Job : IBaseJob
    {
        public Test1Job()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }
        private readonly ILog _logger = LogManager.GetLogger(typeof(TestJob));
        public override void JobMethod(ZD_SyncTaskConfig plan)
        {
            _logger.InfoFormat(plan.MissionName + "=============开始执行时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "=============");
        }



        //public void Execute(IJobExecutionContext context)
        //{
        //    IJobDetail jobDetail = context.JobDetail;
        //    ZD_SyncTaskConfig plan = (ZD_SyncTaskConfig)jobDetail.JobDataMap.Get("Plan");
        //    try
        //    {
        //        _logger.InfoFormat(plan.MissionName + "=============开始执行时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "=============");
        //        MissionSyncHelper.PlanSuccessUpdate(plan);
        //    }
        //    catch (Exception e)
        //    {
        //        MissionSyncHelper.PlanFailUpdate(plan);
        //        _logger.InfoFormat(e.Message.ToString());
        //        throw;
        //    }
        //}
    }

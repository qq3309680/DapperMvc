using log4net;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

    /// <summary>
    /// 测试任务
    /// </summary>
    public class TestJob : IJob
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(TestJob));

        public void Execute(IJobExecutionContext context)
        {
            IJobDetail jobDetail = context.JobDetail;
            ZD_SyncTaskConfig plan = (ZD_SyncTaskConfig)jobDetail.JobDataMap.Get("Plan");
            try
            {
                _logger.InfoFormat(plan.MissionName + "=============开始执行时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "=============");
                MissionSyncHelper.PlanSuccessUpdate(plan);
            }
            catch (Exception e)
            {
                MissionSyncHelper.PlanFailUpdate(plan);
                _logger.InfoFormat(e.Message.ToString());
                throw;
            }

        }
    }

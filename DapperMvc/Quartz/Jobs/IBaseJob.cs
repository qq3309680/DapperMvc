using log4net;

using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// IBaseJob 的摘要说明
/// </summary>
public class IBaseJob : IJob
{
    private readonly ILog _logger = LogManager.GetLogger(typeof(IBaseJob));

    public void Execute(IJobExecutionContext context)
    {
        IJobDetail jobDetail = context.JobDetail;
        ZD_SyncTaskConfig plan = (ZD_SyncTaskConfig)jobDetail.JobDataMap.Get("Plan");
        try
        {
            DateTime SyncSuccessBeginTime = DateTime.Now;
            JobMethod(plan);
            plan.SyncSuccessBeginTime = SyncSuccessBeginTime;
            MissionSyncHelper.PlanSuccessUpdate(plan);
        }
        catch (Exception e)
        {
            MissionSyncHelper.PlanFailUpdate(plan);
            _logger.InfoFormat(e.Message.ToString());
            throw;
        }
    }
    /// <summary>
    /// 只需重写该虚拟方法便可
    /// </summary>
    /// <param name="plan"></param>
    public virtual void JobMethod(ZD_SyncTaskConfig plan) { }
}
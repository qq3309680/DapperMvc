using log4net;

using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// JobInterrupt 的摘要说明
/// </summary>
public class JobInterrupt : IInterruptableJob
{
    public JobInterrupt()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }
    private readonly ILog _logger = LogManager.GetLogger(typeof(JobInterrupt));
    private bool interrupted;
    private JobKey jobKey;

    public void Interrupt()
    {
        _logger.InfoFormat("Interrupt---- {0} 执行中断时间 {1}", jobKey, DateTime.Now.ToString());

    }

    public void Execute(IJobExecutionContext context)
    {
        jobKey = context.JobDetail.Key;
        ZD_SyncTaskConfig plan = MissionSyncHelper.GetTaskInfoByCode(jobKey.Name);
        _logger.InfoFormat("Execute---- {0} 执行中断时间 {1}", jobKey, DateTime.Now.ToString());
    }
}
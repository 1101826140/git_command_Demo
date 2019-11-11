/**************************************************************************
*   
*   =================================
*   CLR版本    ：4.0.30319.42000
*   命名空间    ：ConsoleApp
*   文件名称    ：TaskHelper.cs
*   =================================
*   创 建 者     ：sun
*   创建日期    ：2019/11/12 6:29:47 
*   邮箱          ： sunhlp@qq.com
*   个人主站    ： 
*   功能描述    ：
*   使用说明    ：
*   =================================
*   修改者    ：
*   修改日期    ：
*   修改内容    ：
*   =================================
*  
***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp
{
    /// <summary>
    /// Task 辅助类
    /// </summary>
    public class TaskHelper
    {
        /*
         * 
            public ApiResult add(SchedulerData schdata)
        {
            try
            {
                if (schdata == null)
                {
                    return ApiResult.NewErrorJson("参数为空");
                }

                string schedulerValue = schdata.SchedulerValue;
                string schedulerType = schdata.SchedulerType;
                string runtime = schdata.RunTime;

                SchedulerTask task = new SchedulerTask();
                task.Name = schdata.Name;
                task.HandlerName = schdata.HandlerName;
                task.Description = schdata.Description;

                Scheduler scheduler = new Scheduler();
                if (!String.IsNullOrEmpty(schedulerType) && !String.IsNullOrEmpty(schedulerValue))
                {
                    scheduler.SchedulerType = (SchedulerCodeType)Enum.Parse(typeof(SchedulerCodeType), schedulerType);
                    switch (schedulerType)
                    {
                        case "ByMonth":
                            scheduler.ByMonth = schedulerValue;
                            break;
                        case "ByDays":
                            scheduler.ByDays = schedulerValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            scheduler.RunTime = runtime;
                            break;
                        case "ByDay":
                            scheduler.ByDay = schedulerValue;
                            scheduler.RunTime = runtime;
                            break;
                        case "ByHour":
                            scheduler.ByHour = schedulerValue;
                            break;
                        case "ByMinute":
                            scheduler.ByMinute = schedulerValue;
                            break;
                    }
                }
                task.Scheduler = scheduler.ToString();

                task.StartDate = schdata.StartDate;
                task.FinishDate = schdata.FinishDate;

                task.NextRunning = scheduler.NextRunTime();
                task.LastRunning = null;
                task.IsDisabled = false;
                task.IsRunning = false;

                dbcontext.SchedulerTasks.Add(task);
                dbcontext.SaveChanges();

                return ApiResult.NewSuccessJson("添加成功");
            }
            catch (Exception ex)
            {
                return ApiResult.NewErrorJson(ex.Message.ToString());
            }
        } 
         * ----------------------------------------------------
           task = new SchedulerTask()
                    {
                        Name = taskName,
                        HandlerName = "BackDB",
                        Description = description,
                        Scheduler = "freq=daily;interval=1;runtime=01:00",
                        IsRunning = false,
                        IsDisabled = false,
                        StartDate = now,           //从现在起
                        DBID = dbid,
                        LastRunning = null,
                        NextRunning = new DateTime(now.Year, now.Month, now.Day + 1, 1, 0, 0),//当天和第二天中间的凌晨1点开始运行
                        FinishDate = now.AddDays(1),//24小时候该任务结束
                        DisableReason = "",
                    };



             */
        /// <summary>
        /// 获取下一次执行时间
        /// </summary>
        /// <param name="task">Task 实例</param>
        /// <returns>下一次执行时间</returns>
        public static DateTime GetNextRunningDate(SchedulerTask task)
        {
            if (task != null && !string.IsNullOrEmpty(task.Scheduler))
            {
                Scheduler scheduler = new Scheduler(task.Scheduler, Convert.ToDateTime(task.NextRunning));
                DateTime next = scheduler.NextRunTime();
                return next;
            }
            return DateTime.Now;
        }


        public static Hashtable ToHashTable(string scheduler)
        {
            Hashtable ht = new Hashtable();
            string[] array = scheduler.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (array.Length > 0)
            {
                foreach (string str in array)
                {
                    string[] keyValue = str.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    if (keyValue.Length == 2)
                    {
                        ht.Add(keyValue[0], keyValue[1]);
                    }
                    else if (keyValue.Length == 1)
                    {
                        ht.Add(keyValue[0], null);
                    }
                }
            }
            return ht;
        }
    }
}

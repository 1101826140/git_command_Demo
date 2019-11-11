/**************************************************************************
*   
*   =================================
*   CLR版本    ：4.0.30319.42000
*   命名空间    ：ConsoleApp
*   文件名称    ：Scheduler.cs
*   =================================
*   创 建 者     ：sun
*   创建日期    ：2019/11/12 6:27:29 
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class Scheduler
    {
        #region Properties

        /// <summary>
        /// default value now
        /// </summary>
        public DateTime StartDate { get; set; }

        public SchedulerCodeType SchedulerType { get; set; }

        public string ByMinute { get; set; }

        public string ByHour { get; set; }

        public string ByDay { get; set; }

        public string[] ByDays { get; set; }

        public string ByMonth { get; set; }

        public string RunTime { get; set; }

        public Hashtable SchedulerInfo { get; set; }

        #endregion

        public Scheduler()
        {
            this.StartDate = DateTime.Now;
        }

        public Scheduler(string scheduler)
        {
            if (!String.IsNullOrEmpty(scheduler))
            {
                InitSchedulerInfor(scheduler);
            }
        }

        public Scheduler(string scheduler, DateTime currentRunDate)
        {
            if (!string.IsNullOrEmpty(scheduler))
            {
                InitSchedulerInfor(scheduler);
                this.StartDate = currentRunDate;
            }
        }

        public void InitSchedulerInfor(string scheduler)
        {
            this.SchedulerInfo = TaskHelper.ToHashTable(scheduler);

            string freq = this.SchedulerInfo["freq"].ToString();
            switch (freq)
            {
                case "minutely":
                    this.SchedulerType = SchedulerCodeType.ByMinute;
                    if (this.SchedulerInfo["interval"] != null)
                    {
                        this.ByMinute = this.SchedulerInfo["interval"].ToString();
                    }
                    break;
                case "hourly":
                    this.SchedulerType = SchedulerCodeType.ByHour;
                    if (this.SchedulerInfo["interval"] != null)
                    {
                        this.ByHour = this.SchedulerInfo["interval"].ToString();
                    }
                    break;
                case "daily":
                    this.SchedulerType = SchedulerCodeType.ByDay;
                    if (this.SchedulerInfo["interval"] != null)
                    {
                        this.ByDay = this.SchedulerInfo["interval"].ToString();
                        if (this.SchedulerInfo["runtime"] != null)
                        {
                            this.RunTime = this.SchedulerInfo["runtime"].ToString();
                        }
                    }
                    break;
                case "weekly":
                    this.SchedulerType = SchedulerCodeType.ByDays;
                    if (this.SchedulerInfo["byday"] != null)
                    {
                        this.ByDays = this.SchedulerInfo["byday"].ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (this.SchedulerInfo["runtime"] != null)
                        {
                            this.RunTime = this.SchedulerInfo["runtime"].ToString();
                        }
                    }
                    break;
                case "monthly":
                    this.SchedulerType = SchedulerCodeType.ByMonth;
                    if (this.SchedulerInfo["interval"] != null)
                    {
                        this.ByMonth = this.SchedulerInfo["interval"].ToString();
                    }
                    break;
            }
        }

        /// <summary>
        /// 获取下一次的执行时间
        /// </summary>
        /// <returns></returns>
        public DateTime NextRunTime()
        {
            DateTime nextTime = DateTime.MinValue;
            if (SchedulerType == SchedulerCodeType.ByMonth)
            {
                nextTime = this.StartDate.AddDays(Convert.ToDouble(ByMonth) * 30);
                if (nextTime <= DateTime.Now)
                {
                    nextTime = DateTime.Now.AddDays(Convert.ToDouble(ByMonth));
                }
            }
            else if (SchedulerType == SchedulerCodeType.ByDays)
            {
                /*根据今天在集合中的索引，获取下一天的索引，然后获取两天之间间隔的天数,如果获取不到就从集合中的第一天开始*/
                string today = this.StartDate.DayOfWeek.ToString(), next = string.Empty;
                int index = 0, len = this.ByDays.Length;
                for (int i = 0; i < len; i++)
                {
                    if (this.ByDays[i] == today)
                    {
                        index = i + 1;
                        break;
                    }
                }
                if (index >= len)
                {
                    next = this.ByDays[0];
                }
                else
                {
                    next = this.ByDays[index];
                }

                int todayNum = GetDayNum(today), nextDayNum = GetDayNum(next);
                int interval = 0;
                if (nextDayNum > todayNum)
                {
                    interval = nextDayNum - todayNum;
                }
                else
                {
                    interval = 7 - Math.Abs((nextDayNum - todayNum));
                }
                nextTime = this.StartDate.AddDays(interval);
                nextTime = Convert.ToDateTime(String.Format("{0} {1}", nextTime.ToString("yyyy-MM-dd"), this.RunTime));
            }
            else if (this.SchedulerType == SchedulerCodeType.ByDay)
            {
                nextTime = this.StartDate.AddDays(Convert.ToInt32(ByDay));
                if (nextTime < DateTime.Now)
                {
                    nextTime = DateTime.Now.AddDays(Convert.ToInt32(ByDay));
                }
                nextTime = Convert.ToDateTime(String.Format("{0} {1}", nextTime.ToString("yyyy-MM-dd"), this.RunTime));
            }
            else if (this.SchedulerType == SchedulerCodeType.ByHour)
            {
                nextTime = this.StartDate.AddHours(Convert.ToDouble(ByHour));
                if (nextTime <= DateTime.Now)
                {
                    nextTime = DateTime.Now.AddHours(Convert.ToDouble(ByHour));
                }
            }
            else if (this.SchedulerType == SchedulerCodeType.ByMinute)
            {
                nextTime = this.StartDate.AddMinutes(Convert.ToDouble(ByMinute));
                if (nextTime <= DateTime.Now)
                {
                    nextTime = DateTime.Now.AddMinutes(Convert.ToDouble(ByMinute));
                }
            }
            return nextTime;
        }


        /// <summary>
        /// 在传入的时间的基础上获取下一次运行的时间
        /// </summary>
        /// <returns></returns>
        public DateTime NextRunTimeNotComparedNow()

        {
            DateTime nextTime = DateTime.MinValue;
            if (SchedulerType == SchedulerCodeType.ByMonth)
            {
                nextTime = this.StartDate.AddDays(Convert.ToDouble(ByMonth) * 30);
                //if (nextTime <= DateTime.Now)
                //{
                //    nextTime = DateTime.Now.AddDays(Convert.ToDouble(ByMonth));
                //}
            }
            else if (SchedulerType == SchedulerCodeType.ByDays)
            {
                /*根据今天在集合中的索引，获取下一天的索引，然后获取两天之间间隔的天数,如果获取不到就从集合中的第一天开始*/
                string today = this.StartDate.DayOfWeek.ToString(), next = string.Empty;
                int index = 0, len = this.ByDays.Length;
                for (int i = 0; i < len; i++)
                {
                    if (this.ByDays[i] == today)
                    {
                        index = i + 1;
                        break;
                    }
                }
                if (index >= len)
                {
                    next = this.ByDays[0];
                }
                else
                {
                    next = this.ByDays[index];
                }

                int todayNum = GetDayNum(today), nextDayNum = GetDayNum(next);
                int interval = 0;
                if (nextDayNum > todayNum)
                {
                    interval = nextDayNum - todayNum;
                }
                else
                {
                    interval = 7 - Math.Abs((nextDayNum - todayNum));
                }
                nextTime = this.StartDate.AddDays(interval);
                nextTime = Convert.ToDateTime(String.Format("{0} {1}", nextTime.ToString("yyyy-MM-dd"), this.RunTime));
            }
            else if (this.SchedulerType == SchedulerCodeType.ByDay)
            {
                nextTime = this.StartDate.AddDays(Convert.ToInt32(ByDay));
                //if (nextTime < DateTime.Now)
                //{
                //    nextTime = DateTime.Now.AddDays(Convert.ToInt32(ByDay));
                //}
                nextTime = Convert.ToDateTime(String.Format("{0} {1}", nextTime.ToString("yyyy-MM-dd"), this.RunTime));
            }
            else if (this.SchedulerType == SchedulerCodeType.ByHour)
            {
                nextTime = this.StartDate.AddHours(Convert.ToDouble(ByHour));
                //if (nextTime <= DateTime.Now)
                //{
                //    nextTime = DateTime.Now.AddHours(Convert.ToDouble(ByHour));
                //}
            }
            else if (this.SchedulerType == SchedulerCodeType.ByMinute)
            {
                nextTime = this.StartDate.AddMinutes(Convert.ToDouble(ByMinute));
                //if (nextTime <= DateTime.Now)
                //{
                //    nextTime = DateTime.Now.AddMinutes(Convert.ToDouble(ByMinute));
                //}
            }
            return nextTime;
        }
        private int GetDayNum(string dayOfWeek)
        {
            DayOfWeek day = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), dayOfWeek);
            if (day == DayOfWeek.Sunday)
            {
                return 7;
            }
            return Convert.ToInt32(day);
        }

        public override string ToString()
        {
            string result = string.Empty;
            if (this.SchedulerType == SchedulerCodeType.ByMonth)
            {
                result = String.Format("freq={0};interval={1}", "monthly", this.ByMonth);
            }
            else if (this.SchedulerType == SchedulerCodeType.ByDays)
            {
                result = String.Format("freq={0};byday={1};runtime={2}", "weekly", String.Join(",", this.ByDays), this.RunTime);
            }
            else if (this.SchedulerType == SchedulerCodeType.ByDay)
            {
                result = String.Format("freq={0};interval={1};runtime={2}", "daily", this.ByDay, this.RunTime);
            }
            else if (this.SchedulerType == SchedulerCodeType.ByHour)
            {
                result = String.Format("freq={0};interval={1}", "hourly", this.ByHour);
            }
            else if (this.SchedulerType == SchedulerCodeType.ByMinute)
            {
                result = String.Format("freq={0};interval={1}", "minutely", this.ByMinute);
            }
            return result;
        }
    }
}
public enum FreqCodeType
{
    /// <summary>
    /// specify repeating events based on an interval of a second or more
    /// </summary>
    Secondly = 0,

    /// <summary>
    ///  specify repeating events based on an interval of a minute or more
    /// </summary>
    Minutely = 1,

    /// <summary>
    /// specify repeating events based on an interval of an hour or more
    /// </summary>
    Hourly = 2,

    /// <summary>
    /// specify repeating events based on an interval of a day or more
    /// </summary>
    Daily = 3,

    /// <summary>
    /// specify repeating events based on an interval of a week or more
    /// </summary>
    Weekly = 4,

    /// <summary>
    /// specify repeating events based on an interval of a month or more
    /// </summary>
    Monthly = 5,

    /// <summary>
    /// specify repeating events based on an interval of a year or more
    /// </summary>
    Yearly = 6
}

public enum SchedulerCodeType : int
{
    ByMinute = 0,
    ByHour = 1,
    ByDays = 2,
    ByDay = 3,
    ByMonth = 4
}

public enum MonthCodeType
{
    January = 1,
    February = 2,
    March = 3,
    April = 4,
    May = 5,
    June = 6,
    July = 7,
    August = 8,
    September = 9,
    October = 10,
    November = 11,
    December = 12
}

/// <summary>
/// 
/// </summary>
public enum OrdinalCodeType : int
{
    Every = 0,
    First = 1,
    Second = 2,
    Third = 3,
    Fourth = 4,
    Fifth = 5,
    Sixth
}

public enum TaskCodeType : int
{
    Add = 0,
    Delete = 1,
    Edit = 2,
    Download = 3,
    Export = 4,
    Import = 5,
    Email = 6,
    Synchronize = 7,
    System = 8,
    Upload = 9,
    DownloadHTML = 10,
    DownloadFTP = 11,
    DownloadEmail = 12
}

public enum PostingTypeCodeType
{
    Add,
    Edit,
    Delete
}

/// <summary>
/// 扩展的【DayOfWeek】
/// </summary>
public enum DayOfWeekEx : int
{
    /// <summary>
    /// 没有星期几
    /// </summary>
    None = 0,
    /// <summary>
    /// 星期日
    /// </summary>
    Sunday = 1,
    /// <summary>
    /// 星期一
    /// </summary>
    Monday = 2,
    /// <summary>
    /// 星期二
    /// </summary>
    Tuesday = 4,
    /// <summary>
    /// 星期三
    /// </summary>
    Wednesday = 8,
    /// <summary>
    /// 星期四
    /// </summary>
    Thursday = 16,
    /// <summary>
    /// 星期五
    /// </summary>
    Friday = 32,
    /// <summary>
    /// 星期六
    /// </summary>
    Saturday = 64,
}
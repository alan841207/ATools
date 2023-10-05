using Nancy.Json.Simple;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ATools.Quartz
{
    /// <summary>
    /// Quartz 帮助类
    /// </summary>
    public class QuartzHelper
    {
        public QuartzHelper() { }   

        public async Task Run<T>(string corn) where T : IJob
        {
            // 1.创建scheduler的引用
            ISchedulerFactory schedFact = new StdSchedulerFactory();
            IScheduler sched = await schedFact.GetScheduler();

            //2.启动 scheduler
            await sched.Start();

            // 3.创建 job
            IJobDetail job = JobBuilder.Create<T>()
                    .Build();


            //IScheduleBuilder corn = CronScheduleBuilder.CronSchedule("* * * * * ?");

            // 4.创建 trigger
            ITrigger trigger = TriggerBuilder.Create()
                .WithCronSchedule(corn)
                //.WithSchedule(CronScheduleBuilder.CronSchedule("* * * * * ?").WithMisfireHandlingInstructionDoNothing())               
                .Build();


            // 5.使用trigger规划执行任务job
            await sched.ScheduleJob(job, trigger);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="corn">corn表达式</param>
        /// <param name="T">类型</param>
        /// <returns></returns>
        public async Task Run(string corn,Type obj)
        {
            // 1.创建scheduler的引用
            ISchedulerFactory schedFact = new StdSchedulerFactory();
            IScheduler sched = await schedFact.GetScheduler();

            //2.启动 scheduler
            await sched.Start();

            // 3.创建 job
            IJobDetail job = JobBuilder.Create(obj)
                    .Build();


            //IScheduleBuilder corn = CronScheduleBuilder.CronSchedule("* * * * * ?");

            // 4.创建 trigger
            ITrigger trigger = TriggerBuilder.Create()
                .WithCronSchedule(corn)
                //.WithSchedule(CronScheduleBuilder.CronSchedule("* * * * * ?").WithMisfireHandlingInstructionDoNothing())               
                .Build();


            // 5.使用trigger规划执行任务job
            await sched.ScheduleJob(job, trigger);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace AlbbData
{
    public class QuartzTask
    {
        public async void CreateTast(string url)
        {
            try
            {     // 1.创建scheduler的引用
                ISchedulerFactory schedFact = new StdSchedulerFactory();
                IScheduler sched = await schedFact.GetScheduler();

                //2.启动 scheduler
                await sched.Start();

                // 3.创建 job
                //IJobDetail job = JobBuilder.Create<SimpleJob>()
                //        .WithIdentity("job1", "group1")
                //        .Build();
                 IJobDetail job = JobBuilder.Create<SimpleJob>()
                        .WithIdentity(Guid.NewGuid().ToString(), "group1")
                        .Build();

                // 4.创建 trigger
                //ITrigger trigger = TriggerBuilder.Create()
                //    .WithIdentity("trigger1", "group1")
                //    .WithSimpleSchedule(x => x.WithIntervalInSeconds(5).RepeatForever())
                //    .Build();
                 ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity(Guid.NewGuid().ToString(), "group1")
                    .WithSimpleSchedule(x => x.WithIntervalInSeconds(5).RepeatForever())
                    .Build();

                // 5.使用trigger规划执行任务job
                await sched.ScheduleJob(job, trigger);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}

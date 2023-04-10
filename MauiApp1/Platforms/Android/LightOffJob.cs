using Android.App;
using Android.App.Job;
using Android.Content;

// JOBSERVICE IMPLEMENTATION OF SMARTLIGHT AUTO OFF/ON

namespace SmartLight.Platforms.Android
{
    [Service(Name = "com.companyname.mauiapp1.LightOffJob",
         Permission = "android.permission.BIND_JOB_SERVICE")]
    public class LightOffJob : JobService

    {

        
        public override bool OnStartJob(JobParameters jobParams)
        {

            // PERFORM SCHEDULED JOB HERE

            // Have to tell the JobScheduler the work is done. 
            JobFinished(jobParams, false);
            
            // Return true because of the asynchronous work
            return true;
        }

        public override bool OnStopJob(JobParameters jobParams)
        {
            // we don't want to reschedule the job if it is stopped or cancelled.
            return false;
        }

     

        public void ScheduleTest(TimeSpan timeOff)
        {
            // Convert TimeOff to Miliseconds
            
            var jobScheduler = (JobScheduler)Platform.CurrentActivity.GetSystemService(JobSchedulerService);

            var javaClass = Java.Lang.Class.FromType(typeof(LightOffJob));
            var compName = new ComponentName(Platform.CurrentActivity, javaClass);
            var jobInfo = new JobInfo.Builder(1, compName)
                .SetMinimumLatency(5000)
                .SetOverrideDeadline(5000)
                .Build();

            var result = jobScheduler.Schedule(jobInfo);

            if (result != JobScheduler.ResultSuccess)
            {
                // Handle Job scheduling failure
            }
            else
            {
                // Job Worked, 
            }
        }
        
    }

   


    

}

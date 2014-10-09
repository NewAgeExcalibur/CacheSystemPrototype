using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CacheSystemPrototype.UnitTest
{
   /// <summary>
   /// Just to make sure that approximately thread sleep works as exptected
   /// </summary>
    public class ThreadSleepTests
    {
        [Fact]
        public void ThreadSleep_SleepTenMiliSeconds_ItShouldTakeApproximatelyTenMilliSeconds()
        {
            ThreadSleepTester threadSleepTester = new ThreadSleepTester();
            Stopwatch stopwatch=new Stopwatch();

            stopwatch.Start();
            threadSleepTester.Sleep(10);
            stopwatch.Stop();

            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

            Assert.InRange(elapsedMilliseconds, 5, 20);
        }

        [Fact]
        public void ThreadSleep_SleepFiftyMiliSeconds_ItShouldTakeApproximatelyFiftyMilliSeconds()
        {
            ThreadSleepTester threadSleepTester = new ThreadSleepTester();
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            threadSleepTester.Sleep(50);
            stopwatch.Stop();

            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

            Assert.InRange(elapsedMilliseconds, 40, 60);
        }
        
        [Fact]
        public void ThreadSleep_TenTimesSleepTenMiliSeconds_ItShouldTakeApproximatelyTenMilliSecondsEach()
        {
            ThreadSleepTester threadSleepTester = new ThreadSleepTester();

            for (int i = 0; i < 10; i++)
            {
                new Thread(() =>
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Reset();
                    stopwatch.Start();
                    threadSleepTester.Sleep(10);
                    stopwatch.Stop();

                    var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

                    Assert.InRange(elapsedMilliseconds, 5, 20);

                }).Start();
            }

               
            }
           
     }
    
    /// <summary>
    /// a test class to see the behavior of thread.sleep and imported dll 
    /// </summary>
    public class ThreadSleepTester
    {
        [DllImport("winmm.dll")]
        internal static extern uint timeBeginPeriod(uint period);
        [DllImport("winmm.dll")]
        internal static extern uint timeEndPeriod(uint period);
        public void Sleep(int miliSeconds)
        {
            timeBeginPeriod(5);
            Thread.Sleep(miliSeconds);
            timeEndPeriod(5);
        }
    }
}

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace LockPerfTests
{
    public static class LockTestLogic
    {
        static int counter;

        public static void TestLocks(Action<TimeSpan, bool, string> logEntry)
        {
            ILocker[] lockStyles = {
                new NoLock(),
                new InterlockedSpinLock(),
                new MonitorLock(),
                new MutexLock(),
                new SemaphoreSlimLock(),
                new SemaphoreLock(), 
                new ReaderWriterLockSlimLock()
            };

            const int count = 1000000;

            foreach (var ls in lockStyles)
            {
                using (ILocker theLock = ls)
                {
                    for (int mode = 0; mode < 2; mode++)
                    {
                        bool exclusive = (mode == 0);
                        var sw = Stopwatch.StartNew();

                        CountdownEvent startGate = new CountdownEvent(2);
                        Action work = () => {
                            startGate.Signal();
                            for (int n = 0; n < count; n++)
                            {
                                theLock.EnterLock(exclusive);
                                counter++; // "the work"
                                theLock.ExitLock();
                            }
                        };

                        // Block this thread on the results.
                        Task.WaitAll(
                            Task.Run(work), 
                            Task.Run(work));

                        sw.Stop();
                        
                        logEntry?.Invoke(sw.Elapsed, exclusive, theLock.GetType().Name);
                    }
                }
            }
        }

    }
}

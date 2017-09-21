using System;
using System.Threading;

namespace LockPerfTests
{
    public interface ILocker : IDisposable
    {
        void EnterLock(bool exclusive);
        void ExitLock();
    }

    public class NoLock : ILocker
    {
        public void Dispose()
        {
        }

        public void EnterLock(bool exclusive)
        {
        }

        public void ExitLock()
        {
        }
    }

    public class InterlockedSpinLock : ILocker
    {
        volatile int owned;

        public void EnterLock(bool exclusive)
        {
            while (Interlocked.Exchange(ref owned, 1) != 0)
            {
                // Spinning here
            }
        }

        public void ExitLock()
        {
            owned = 0;
        }

        public void Dispose()
        {
        }
    }


    public class MonitorLock : ILocker
    {
        readonly object monitor = new object();

        public void EnterLock(bool exclusive)
        {
            Monitor.Enter(monitor);
        }

        public void ExitLock()
        {
            Monitor.Exit(monitor);
        }

        public void Dispose()
        {
        }
    }

    public class MutexLock : ILocker
    {
        readonly Mutex mutex = new Mutex(false);
        public void EnterLock(bool exclusive)
        {
            mutex.WaitOne();
        }

        public void ExitLock()
        {
            mutex.ReleaseMutex();
        }

        public void Dispose()
        {
            mutex.Dispose();
        }
    }

    public class SemaphoreLock : ILocker
    {
        private Semaphore _lock = new Semaphore(1,1);

        public void Dispose()
        {
            _lock.Dispose();
        }

        public void EnterLock(bool exclusive)
        {
            _lock.WaitOne();
        }

        public void ExitLock()
        {
            _lock.Release();
        }
    }

    public class SemaphoreSlimLock : ILocker
    {
        private SemaphoreSlim _lock = new SemaphoreSlim(1);
        
        public void Dispose()
        {
            _lock.Dispose();            
        }

        public void EnterLock(bool exclusive)
        {
            _lock.Wait();
        }

        public void ExitLock()
        {
            _lock.Release();
        }
    }

    public class ReaderWriterLockSlimLock : ILocker
    {
        private bool holdingWriteLock;
        private ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

        public void EnterLock(bool exclusive)
        {
            holdingWriteLock = exclusive;
            if (exclusive)
            {
                rwLock.EnterWriteLock();
            }
            else
            {
                rwLock.EnterReadLock();
            }
        }

        public void ExitLock()
        {
            if (holdingWriteLock)
            {
                rwLock.ExitWriteLock();
            }
            else
            {
                rwLock.ExitReadLock();
            }
        }

        public void Dispose()
        {
            rwLock.Dispose();
        }
    }

}

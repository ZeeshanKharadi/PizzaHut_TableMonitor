using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace TableMonitor.Class
{
    public static class SingleInstance
    {
        static Mutex mutex;
        public static bool Start(string applicationIdentifier)
        {
            bool isSingleInstance = false;
            mutex = new Mutex(true, applicationIdentifier, out isSingleInstance);
            return isSingleInstance;
        }
        public static void Stop()
        {
            mutex.ReleaseMutex();
        }
    }
}

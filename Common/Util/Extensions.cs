﻿namespace Radar.Common.Util
{
    public static class ThreadExtension
    {
        public static void WaitAll(this IEnumerable<Thread> threads)
        {
            if (threads != null)
            {
                foreach (Thread thread in threads)
                {
                    thread.Join();
                }
            }
        }
    }

    public static class ListExtension
    {

    }

}
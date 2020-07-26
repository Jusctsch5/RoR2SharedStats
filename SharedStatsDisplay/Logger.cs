using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SharedStatsDisplay
{
    public abstract class LogBase
    {
        public abstract void Log(string message);
    }

    public class ConsoleLogger : LogBase
    {
        public override void Log(string message)
        {
            Debug.Log(message);
        }
    }

    [Flags]
    public enum LogTarget
    {
        None =        0x00,
        Networking =  0x01, 
        Recording =   0x02, 
        General =     0x04,
        Init =        0x08,
        All =         0xFF,
    }

    public static class LogHelper
    {
        private static ConsoleLogger logger = new ConsoleLogger();
        private static bool enabled = true;
        //private static LogTarget targetMask = LogTarget.All;
        private static LogTarget targetMask = LogTarget.Init | LogTarget.Recording;

        static bool IsTargetEnabled(LogTarget target)
        {
            return ((target & targetMask) != LogTarget.None);
        }
        public static void Log(LogTarget target, string message)
        {
            string messageWithHeader = "SharedStats: " + message;
            if (enabled == false)
            {
                return;
            }
            if (IsTargetEnabled(target) == false)
            {
                return;
            }

            logger.Log(message);
        }
    }
}

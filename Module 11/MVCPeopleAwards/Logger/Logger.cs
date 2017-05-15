using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPeopleAwards
{
    public static class Logger
    {
        public static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static void LogException(Exception e)
        {
            logger.Error(e.GetType() + ": " + e.Message);
            logger.Error(e.StackTrace);
        }
    }
}
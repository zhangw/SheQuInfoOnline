using System;
using System.Collections.Generic;
using System.Text;

using log4net;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace Wedo.Utility.Log
{
    public enum AppError
    {
        WARN = 0,
        EROR = 1,
        FATL = 2
    }
    /// <summary>
    /// 类名称：Logger
    /// 类说明：按照日志的四个级别写日志。日志目录通过配置文件设定
    /// 作者：  
    /// 完成日期：
    /// </summary>
    public class Logger
    {
        private static readonly log4net.ILog loginfo = log4net.LogManager.GetLogger("ThermoFisher.Logging.Info");
        private static readonly log4net.ILog logdebug = log4net.LogManager.GetLogger("ThermoFisher.Logging.Debug");
        private static readonly log4net.ILog logerror = log4net.LogManager.GetLogger("ThermoFisher.Logging.Error");
        private static readonly log4net.ILog logperf = log4net.LogManager.GetLogger("ThermoFisher.Logging.Perf");

        /// <summary>
        /// 记录程序正常运行的日志
        /// </summary>
        /// <param name="ModuleID">模块编号 （每个BLL类有一个模块编号）</param>
        /// <param name="FuncCode">功能编号</param>
        /// <param name="userid">用户编号</param>
        /// <param name="msg">在异常或错误情况下给出的进一步的说明信息</param>
        /// <param name="ContextInfo">上下文信息（由调用方生成，以名值对方式组织，如userid=123;username=abc）</param>
        public static void LogInfo(string moduleID, string funcCode, int userid, string msg, string contextInfo)
        {
            StringBuilder logMessage = new StringBuilder();
            logMessage.AppendFormat("Time={0}; ModuleID={1}; FuncCode={2}; UserID={3}; ", DateTime.Now.ToString(), moduleID, funcCode, userid);
            if (contextInfo != null && contextInfo.Length > 0)
            {
                logMessage.AppendLine();
                logMessage.Append("ContextInfo=" + contextInfo);
            }
            if (msg != null && msg.Length > 0)
            {
                logMessage.AppendLine();
                logMessage.Append("Message=" + msg);
            }

            if (loginfo.IsInfoEnabled) loginfo.Info(logMessage.ToString());

        }

        /// <summary>
        /// 记调试日志
        /// </summary>
        /// <param name="ModuleID">模块编号</param>
        /// <param name="FuncCode">功能编号</param>
        /// <param name="msg">在异常或错误情况下给出的进一步的说明信息</param>
        /// <param name="ContextInfo">上下文信息（由调用方生成，以名值对方式组织，如userid=123;username=abc）</param>
        public static void LogDebug(string moduleID, string funcCode, string msg, string contextInfo)
        {
            StringBuilder logMessage = new StringBuilder();
            logMessage.AppendFormat("Time={0}; ModuleID={1}; FuncCode:{2}; ", DateTime.Now.ToString(), moduleID, funcCode);
            if (contextInfo != null && contextInfo.Length > 0)
            {
                logMessage.AppendLine();
                logMessage.Append("ContextInfo=" + contextInfo);
            }
            if (msg != null && msg.Length > 0)
            {
                logMessage.AppendLine();
                logMessage.Append("Message=" + msg);
            }

            if (logdebug.IsDebugEnabled) logdebug.Debug(logMessage.ToString());
        }

        /// <summary>
        /// 记错误日志或异常日志
        /// </summary>
        /// <param name="moduleID">模块编号</param>
        /// <param name="funcCode">功能编号</param>
        /// <param name="Level">日志级别，3级（WARN，EROR，FATL）</param>
        /// <param name="errCode">错误码，对于没有明确错误码的场合需要给一个系统级的通用错误码</param>
        /// <param name="ex">发生的异常</param>
        /// <param name="msg"></param>
        /// <param name="contextInfo"></param>
        public static void LogError(string moduleID, string funcCode,  Exception ex, string msg, string contextInfo)
        {
            LogError(moduleID, funcCode, ex.Message, msg, contextInfo);
        }

        /// <summary>
        /// 记错误日志或异常日志
        /// </summary>
        /// <param name="moduleID">模块编号</param>
        /// <param name="funcCode">功能编号</param>
        /// <param name="Level">日志级别，3级（WARN，EROR，FATL）</param>
        /// <param name="errCode">错误码，对于没有明确错误码的场合需要给一个系统级的通用错误码</param>
        /// <param name="ex">发生的异常</param>
        /// <param name="msg"></param>
        /// <param name="contextInfo"></param>
        public static void LogError(string moduleID, string funcCode,string ex, string msg, string contextInfo)
        {
             AppError level= AppError.EROR;
             int errCode = 0;
            StringBuilder logMessage = new StringBuilder();
            logMessage.AppendFormat("Time={0}; LogLevel={1}; ModuleID={2}; FuncCode={3}; ErrorCode={4}; ", DateTime.Now.ToString(), level.ToString(), moduleID, funcCode, errCode);
            if (contextInfo != null && contextInfo.Length > 0)
            {
                logMessage.AppendLine();
                logMessage.Append("ContextInfo=" + contextInfo);
            }

            if (msg != null && msg.Length > 0)
            {
                logMessage.AppendLine();
                logMessage.Append("Message=" + msg);
            }

            if (ex != null)
            {
                logMessage.AppendLine();
                logMessage.AppendLine("Exception=" + ex);
            }

            if (logerror.IsErrorEnabled) logerror.Error(logMessage.ToString());

        }

        /// <summary>
        /// 记性能日志
        /// </summary>
        /// <param name="StartTime">开始时间</param>
        /// <param name="ModuleID">模块编号</param>
        /// <param name="FuncCode">功能编号</param>
        /// <param name="ContextInfo">上下文信息（由调用方生成，以名值对方式组织，如userid=123;username=abc）</param>
        public static void LogPerf(DateTime startTime, string moduleID, string funcCode, string contextInfo)
        {
            DateTime endTime = DateTime.Now;
            TimeSpan ts = endTime.Subtract(startTime);

            StringBuilder logMessage = new StringBuilder();
            logMessage.AppendFormat("StartTime={0}; EndTime={1}; Interval={2}(ms); ModuleID={3}; FuncCode={4}; ", startTime, DateTime.Now.ToString(), ts.TotalMilliseconds.ToString(), moduleID, funcCode);
            if (contextInfo != null && contextInfo.Length > 0)
            {
                logMessage.AppendLine();
                logMessage.Append("ContextInfo=" + contextInfo);
            }

            if (logperf.IsWarnEnabled) logperf.Warn(logMessage.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logmsg"></param>
        /// <param name="loggerName"></param>
        public static void Log(string logmsg, string loggerName)
        {
            log4net.LogManager.GetLogger(loggerName).Info(logmsg);
        }

        public static ILog GetLogger(string loggerName)
        {
            return log4net.LogManager.GetLogger(loggerName);
        }
    }
}

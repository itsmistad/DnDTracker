using DnDTracker.Web.Configuration;
using DnDTracker.Web.Objects;
using DnDTracker.Web.Persisters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DnDTracker.Web.Logging
{
    public class Log
    {
        private static void WithTag(string tag, string message, MethodBase caller, Exception ex)
        {
            var formattedTag = $"{tag.Trim().ToUpper()}";
            var createDate = DateTime.Now.ToString("s");
            var originTypeName = caller.DeclaringType?.Name;
            var originMethodName = caller.Name;
            var disallowedTypeNameWords = new string[]
            {
                "Singleton", "DynamoDbPersister", "EnvironmentConfig", "InvokeMethod"
            };
            var disallowedMemberNameWords = new string[]
            {

            };

            var log = $"[{createDate}]\t[{formattedTag}]\t{message} ({originTypeName}.{originMethodName})";
            if (formattedTag == "DEBUG" && Singleton.Get<EnvironmentConfig>()?.Current != Environments.Local)
                return;

            System.Diagnostics.Debug.WriteLine(log);

            foreach (var t in disallowedTypeNameWords)
                if (originTypeName.Contains(t))
                    return;
            foreach (var t in disallowedMemberNameWords)
                if (originMethodName.Contains(t))
                    return;

            if (bool.TryParse(Singleton.Get<AppConfig>()[ConfigKeys.System.PersistLogs], out var persistLogs) && persistLogs)
            {
                var dynamo = Singleton.Get<DynamoDbPersister>();
                dynamo?.Save(new LogObject(formattedTag, message, caller, ex));
            }
        }

        public static void Info(string message) => WithTag("info", message, new StackTrace().GetFrame(1).GetMethod(), null);
        public static void Error(string message, Exception ex = null) => WithTag("error", message, new StackTrace().GetFrame(1).GetMethod(), ex);
        public static void Warn(string message, Exception ex = null) => WithTag("warn", message, new StackTrace().GetFrame(1).GetMethod(), ex);
        public static void Debug(string message, Exception ex = null) => WithTag("debug", message, new StackTrace().GetFrame(1).GetMethod(), ex);
    }
}

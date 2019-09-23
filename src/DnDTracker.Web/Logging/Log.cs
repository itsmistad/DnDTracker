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
        /// <summary>
        /// Forces a persisting attempt regardless of any blocking conditions.
        /// This is only intended for use within test cases.
        /// </summary>
        public static bool ForcePersist = false;

        private static void WithTag(string tag, string message, MethodBase caller, Exception ex)
        {
            var formattedTag = $"{tag.Trim().ToUpper()}";
            var createDate = DateTime.Now.ToString("s");
            var originTypeName = caller.DeclaringType?.Name ?? "";
            var originMethodName = caller.Name ?? "";
            var disallowedTypeNameWords = new []
            {
                "Singleton", "DynamoDbPersister", "EnvironmentConfig"
            };
            var disallowedMemberNameWords = new string[]
            {

            };

            var log = $"[{createDate}]\t[{formattedTag}]\t{message} ({originTypeName}.{originMethodName})";
            var envConfig = Singleton.Get<EnvironmentConfig>();
            if (formattedTag == "DEBUG" && (envConfig?.Current ?? Environments.Local) != Environments.Local)
                return;

            System.Diagnostics.Debug.WriteLine(log);
            Console.WriteLine(log);

            // Don't persist logs while running tests.
            if (Assembly.GetEntryAssembly().GetName().Name.ToLower().Contains("test") && !ForcePersist)
                return;

            foreach (var t in disallowedTypeNameWords)
                if (originTypeName.Contains(t))
                    return;
            foreach (var t in disallowedMemberNameWords)
                if (originMethodName.Contains(t))
                    return;

            if (ForcePersist || (bool.TryParse(Singleton.Get<AppConfig>()[ConfigKeys.System.PersistLogs], out var persistLogs) && persistLogs))
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

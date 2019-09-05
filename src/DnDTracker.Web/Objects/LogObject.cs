using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DnDTracker.Web.Objects
{
    [DynamoDBTable("DnDTracker.Logs")]
    public class LogObject : AbstractObject
    {
        [DynamoDBProperty]
        public string Tag { get; set; }
        [DynamoDBProperty]
        public string Message { get; set; }
        [DynamoDBProperty]
        public string TypeName { get; set; }
        [DynamoDBProperty]
        public string MethodName { get; set; }
        [DynamoDBProperty]
        public string StackTrace { get; set; }

        public LogObject() : base() { }

        public LogObject(string tag, string message, MethodBase caller, Exception exception)
        {
            Tag = tag;
            Message = message;
            TypeName = caller.DeclaringType.Name;
            MethodName = caller.Name;
            StackTrace = exception?.StackTrace ?? "";
        }
    }
}

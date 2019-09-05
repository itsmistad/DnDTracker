using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnDTracker.Web.Objects
{
    public interface IObject
    {
        Guid Guid { get; set; }
        string CreateDate { get; set; }
        void FromDocument(Document document);
    }
}

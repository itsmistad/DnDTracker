using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using DnDTracker.Web.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnDTracker.Web.Objects
{
    public abstract class AbstractObject : IObject
    {
        /// <summary>
        /// The unique identifier for an IObject.
        /// </summary>
        [DynamoDBHashKey]
        public Guid Guid { get; set; }
        /// <summary>
        /// The date of creation for an IObject.
        /// </summary>
        [DynamoDBProperty]
        public string CreateDate { get; set; }

        /// <summary>
        /// The default base constructor of an IObject. Be sure to include ": base ()" with your object's constructor.
        /// </summary>
        public AbstractObject()
        {
            Guid = Guid.NewGuid();
            CreateDate = DateTime.Now.ToString("s");
        }

        /// <summary>
        /// The conversion method when reading from dynamo.
        /// You do not need to override this if the object is write-only. Be sure to call base.FromDocument();
        /// </summary>
        /// <param name="document">The dynamo document "row".</param>
        public virtual void FromDocument(Document document)
        {
            Guid = document.TryGetValue("Guid", out var entry) ?
                entry.AsGuid() :
                Guid.NewGuid();
            CreateDate = (document.TryGetValue("CreateDate", out entry) ?
                Convert.ToDateTime(entry.AsString()) :
                DateTime.Now).ToString("s");
        }
    }
}

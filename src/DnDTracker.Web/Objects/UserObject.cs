using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnDTracker.Web.Objects
{
    public class UserObject : AbstractObject
    {
        [DynamoDBProperty]
        public string Email { get; set; }
        [DynamoDBProperty]
        public string FullName { get; set; }
        [DynamoDBProperty]
        public string FirstName { get; set; }
        [DynamoDBProperty]
        public string LastName { get; set; }
        [DynamoDBProperty]
        public string ImageUrl { get; set; }

        public UserObject() : base() { }

        public UserObject(string email, string fullName, string firstName, string lastName, string imageUrl)
        {
            Email = email;
            FullName = fullName;
            FirstName = firstName;
            LastName = lastName;
            ImageUrl = imageUrl;
        }

        public override void FromDocument(Document document)
        {
            base.FromDocument(document);

            Email = document.TryGetValue("Email", out var entry) ?
                entry.AsString() :
                "";
            FullName = document.TryGetValue("FullName", out entry) ?
                entry.AsString() :
                "";
            FirstName = document.TryGetValue("FirstName", out entry) ?
                entry.AsString() :
                "";
            LastName = document.TryGetValue("LastName", out entry) ?
                entry.AsString() :
                "";
            ImageUrl = document.TryGetValue("ImageUrl", out entry) ?
                entry.AsString() :
                "";
        }
    }
}

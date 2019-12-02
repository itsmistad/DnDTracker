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
        /// <summary>
        /// !! DO NOT MODIFY !!
        /// The user's Google email address.
        /// </summary>
        [DynamoDBProperty]
        public string Email { get; set; }
        /// <summary>
        /// The user's full name.
        /// </summary>
        [DynamoDBProperty]
        public string FullName { get; set; }
        /// <summary>
        /// The user's first name.
        /// </summary>
        [DynamoDBProperty]
        public string FirstName { get; set; }
        /// <summary>
        /// The user's last name.
        /// </summary>
        [DynamoDBProperty]
        public string LastName { get; set; }
        /// <summary>
        /// The URL to the user's profile image.
        /// </summary>
        [DynamoDBProperty]
        public string ImageUrl { get; set; }
        [DynamoDBProperty]
        public List<Guid> CharacterGuids { get; set; }

        public UserObject() : base() { }

        public UserObject(string email, string fullName, string firstName, string lastName, string imageUrl)
        {
            Email = email;
            FullName = fullName;
            FirstName = firstName;
            LastName = lastName;
            ImageUrl = imageUrl;
            CharacterGuids = new List<Guid>();
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

            CharacterGuids = new List<Guid>();
            if (document.TryGetValue("CharacterGuids", out entry))
            {
                foreach (var p in entry.AsListOfPrimitive())
                {
                    CharacterGuids.Add(p.AsGuid());
                }
            }
        }
    }
}

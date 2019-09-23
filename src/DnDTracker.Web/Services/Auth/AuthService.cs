using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using DnDTracker.Web.Configuration;
using DnDTracker.Web.Logging;
using DnDTracker.Web.Models;
using DnDTracker.Web.Objects;
using DnDTracker.Web.Persisters;
using DnDTracker.Web.Services.Session;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnDTracker.Web.Services.Auth
{
    public class AuthService
    {
        public object Authenticate(Controller controller, AuthUserModel authUser)
        {
            // 1. Check if the user's id token is valid through Google's Auth API.
            bool tokenIsValid = false;
            try
            {
                var payload = Task.Run(async () => await GoogleJsonWebSignature.ValidateAsync(authUser.Token)).Result;
                tokenIsValid = payload != null && payload.Email == authUser.Email && (string)payload.Audience == Singleton.Get<AppConfig>()[ConfigKeys.System.Google.SignInId];
            }
            catch (Exception) { }

            if (!tokenIsValid)
                return new
                {
                    response = "err",
                    message = "Bad id_token. Please try again."
                };

            // 2. Check if the user is already in the UserObject table.
            var persister = Singleton.Get<DynamoDbPersister>();
            var expression = new Expression() // Create the AWS Expression for our Scan query.
            {
                ExpressionStatement = "Email = :email",
                ExpressionAttributeValues = new Dictionary<string, DynamoDBEntry>()
                {
                    { ":email", authUser.Email }
                }
            };
            var results = persister.Scan<UserObject>(expression); // Look for any user with the Email value of authUser.Email
            Log.Debug($"Authenticating {authUser.Email} returned {results.Count} results from the table.");
            UserObject user;
            string message;

            // 3. If the user exists...
            if (results.Any())
            {
                user = results.First();
                Log.Debug($"Found existing user with guid {user.Guid}");

                message = "Successfully signed in.";
            }
            // 4. If the user doesn't exist, save the new UserObject.
            else
            {
                user = new UserObject(authUser.Email, authUser.FullName, authUser.FirstName, authUser.LastName, authUser.ImageUrl);
                persister.Save(user);
                Log.Debug($"Saved new user: {authUser.Email} [{user.Guid}]");

                message = "Successfully registered.";
            }

            /* 
             * Each of these is accessible from
             *  - any .cshtml file by using:
             *      @session.Get("<key>", <default value>, HttpContextAccessor)
             *  - any Controller by using:
             *      session.Get("<key>", <default value>, null, controller);
             *      
             *  Where "session" is:
             *      var session = Singleton.Get<SessionService>();
             *  
             *  These are the variables needed for showing login information outside of the Login view.
             */
            // 5. Save the user's data to the AppState to be retrieved later by the Controller or View.
            var session = Singleton.Get<SessionService>();
            session.Set("UserGuid", user.Guid.ToString(), null, controller); // Use this when checking if the player is logged in.
            session.Set("UserEmail", user.Email, null, controller);
            session.Set("UserFirstName", user.FirstName, null, controller);
            session.Set("UserImageUrl", user.ImageUrl, null, controller);

            return new
            {
                response = "ok",
                message
            };
        }
    }
}

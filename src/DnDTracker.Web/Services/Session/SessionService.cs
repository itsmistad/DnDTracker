using DnDTracker.Web.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnDTracker.Web.Services.Session
{
    public class SessionService
    {
        /// <summary>
        /// Retrieves a variable as a JToken from the session's AppState through an accessor or controller.
        /// Allows for JToken navigation.
        /// Atleast an accessor or a controller is needed.
        /// </summary>
        /// <param name="key">The key of the variable.</param>
        /// <param name="accessor">The context accessor provided by the calling View.</param>
        /// <param name="controller">The calling controller instance.</param>
        public JToken GetJson(string key, IHttpContextAccessor accessor = null, Controller controller = null)
        {
            var session = GetSession(accessor, controller);
            var state = GetCurrentAppState(session);
            if (state.TryGetValue(key, out var val))
                return val;
            return null;
        }

        /// <summary>
        /// Retrieves a variable from the session's AppState through an accessor or controller.
        /// Allows for complex variable typing and conversion.
        /// Atleast an accessor or a controller is needed.
        /// </summary>
        /// <typeparam name="T">The type to convert the variable to.</typeparam>
        /// <param name="key">The key of the variable.</param>
        /// <param name="defaultVal">The default value to return if the variable does not exist.</param>
        /// <param name="accessor">The context accessor provided by the calling View.</param>
        /// <param name="controller">The calling controller instance.</param>
        public T Get<T>(string key, T defaultVal = default, IHttpContextAccessor accessor = null, Controller controller = null)
        {
            var session = GetSession(accessor, controller);
            var state = GetCurrentAppState(session);
            try
            {
                if (state.TryGetValue(key, out var val))
                {
                    Log.Debug($"Key found in AppState. Key: {key} | Value: {val.ToString()}");
                    return val.ToObject<T>();
                }
                else
                    Log.Debug($"Tried to find unexistent variable in AppState: {key}");
            }
            catch (Exception ex)
            {
                Log.Error($"Something went wrong trying to retrieve and convert ({typeof(T).Name}) a variable ({key}) from the AppState. {ex.Message}", ex);
            }

            return defaultVal;
        }

        /// <summary>
        /// Saves a key-value pair as a variable in the session's AppState through the context accessor or controller.
        /// Atleast an accessor or a controller is needed.
        /// </summary>
        /// <param name="key">The key of the variable.</param>
        /// <param name="value">The value of the variable.</param>
        /// <param name="accessor">The context accessor provided by the calling View.</param>
        /// <param name="controller">The calling constructor instance.</param>
        public void Set(string key, object value, IHttpContextAccessor accessor = null, Controller controller = null)
        {
            var session = GetSession(accessor, controller);
            var state = GetCurrentAppState(session);

            if (state.ContainsKey(key))
                state[key] = JToken.FromObject(value);
            else
                state.Add(key, JToken.FromObject(value));

            Log.Debug($"AppState modified: {state.ToString()}");

            session.SetString("AppState", state.ToString(Formatting.None));
        }

        private JObject GetCurrentAppState(ISession session)
        {
            try
            {
                var appState = session.GetString("AppState") ?? "{}";
                return JObject.Parse(appState);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to parse the AppState. {ex.Message}", ex);
            }

            return new JObject();
        }

        private ISession GetSession(IHttpContextAccessor accessor = null, Controller controller = null)
        {
            ISession session;
            if (accessor != null)
                session = accessor.HttpContext.Session;
            else if (controller != null)
                session = controller.HttpContext.Session;
            else
                throw new ArgumentException("Tried accessing an ISession without an HttpContextAccessor or Controller.");

            return session;
        }
    }
}

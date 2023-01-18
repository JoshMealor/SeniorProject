namespace SeniorProject.Models.DataLayer
{
    public static class SessionExtensions
    {
        //Serialize the value passed in to a string object and assign that string to the session storage of the current session passed in to the key passed in.
        public static void SetObject<T>(this ISession session,string key, T value)
        {
            session.SetString(key, Newtonsoft.Json.JsonConvert.SerializeObject(value));
        }
        //Deserailize the object from the string value retrieved from the current session passed in to the key passed in
        public static T GetObject<T>(this ISession session, string key)
        {
            var valueJson = session.GetString(key);
            if (string.IsNullOrEmpty(valueJson))
            {
                return default(T);
            }
            else
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(valueJson);
            }
        }
    }
}

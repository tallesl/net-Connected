namespace ConnectedLibrary
{
    using System;
    using System.Configuration;
    using System.Data.Common;
    using System.Globalization;

    /// <summary>
    /// Issues tests commands to SMTP, RDBMS and Redis servers.
    /// </summary>
    public static partial class Connected
    {
        /// <summary>
        /// Issues a SELECT 1 to a RDBMS server thus testing its connection.
        /// </summary>
        /// <param name="name">Connection string name</param>
        /// <returns>True if the server responded with success, false otherwise</returns>
        public static bool Rdbms(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            var cs = ConfigurationManager.ConnectionStrings[name];
            if (cs == null)
                throw new ConfigurationErrorsException(string.Format(CultureInfo.InvariantCulture,
                    "Couldn't find a \"{0}\" connection string.", name));

            return Rdbms(cs);
        }

        /// <summary>
        /// Issues a SELECT 1 to a RDBMS server thus testing its connection.
        /// </summary>
        /// <param name="cs">Connection string</param>
        /// <returns>True if the server responded with success, false otherwise</returns>
        public static bool Rdbms(ConnectionStringSettings cs)
        {
            if (cs == null)
                throw new ArgumentNullException("cs");

            try
            {
                using (var conn = DbProviderFactories.GetFactory(cs.ProviderName).CreateConnection())
                {
                    conn.ConnectionString = cs.ConnectionString;
                    conn.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}

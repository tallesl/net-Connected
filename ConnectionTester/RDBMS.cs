namespace ConnectionTests
{
    using ConnectionStringReading.Exceptions;
    using QckQuery;
    using System;
    using System.Configuration;

    /// <summary>
    /// Issues tests commands to SMTP, RDBMS and Redis servers.
    /// </summary>
    public static partial class ConnectionTester
    {
        /// <summary>
        /// Issues a SELECT 1 to a RDBMS server thus testing its connection.
        /// </summary>
        public static class RDBMS
        {
            /// <summary>
            /// Returns true if the RDBMS server responded with success, false otherwise.
            /// </summary>
            /// <param name="connectionStringName">Name of the connection string to use</param>
            /// <returns>True if the RDBMS server responded with success, false otherwise</returns>
            /// <exception cref="ArgumentNullException">If connectionStringName is null</exception>
            /// <exception cref="NoSuchConnectionStringException">
            /// A connection string with the provided name doesn't exist
            /// </exception>
            /// <exception cref="EmptyConnectionStringException">An empty connection string is found</exception>
            /// <exception cref="EmptyProviderNameException">An empty provider name is found</exception>
            public static bool IsOk(string connectionStringName)
            {
                return IsOk(new QuickQuery(connectionStringName));
            }

            /// <summary>
            /// Returns true if the RDBMS server responded with success, false otherwise.
            /// </summary>
            /// <param name="connectionString">Connection string to use</param>
            /// <returns>True if the RDBMS server responded with success, false otherwise</returns>
            /// <exception cref="ArgumentNullException">If connectionStringName is null</exception>
            /// <exception cref="EmptyConnectionStringException">An empty connection string is found</exception>
            /// <exception cref="EmptyProviderNameException">An empty provider name is found</exception>
            public static bool IsOk(ConnectionStringSettings connectionString)
            {
                return IsOk(new QuickQuery(connectionString));
            }

            private static bool IsOk(QuickQuery query)
            {
                try
                {
                    return query.SelectSingle<int>("SELECT 1") == 1;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}

namespace ConnectionTests
{
    using SocketThat;
    using System.Net.Sockets;

    public static partial class ConnectionTester
    {
        /// <summary>
        /// Issues a PING to a Redis server thus testing its connection.
        /// </summary>
        public static class Redis
        {
            private const string _lineTerminator = "\r\n";

            /// <summary>
            /// Returns true if the Redis server responded with success, false otherwise.
            /// Assumes localhost as host and 6379 as port.
            /// </summary>
            /// <returns>True if the Redis server responded with success, false otherwise.</returns>
            public static bool IsOk()
            {
                return IsOk("localhost", 6379);
            }

            /// <summary>
            /// Returns true if the Redis server responded with success, false otherwise.
            /// Assumes localhost as host and 6379 as port.
            /// </summary>
            /// <param name="auth">Redis password</param>
            /// <returns>True if the Redis server responded with success, false otherwise.</returns>
            public static bool IsOk(string auth)
            {
                return IsOk("localhost", 6379, auth);
            }

            /// <summary>
            /// Returns true if the Redis server responded with success, false otherwise.
            /// </summary>
            /// <param name="host">Redis server host</param>
            /// <param name="port">Redis server port</param>
            /// <returns>True if the Redis server responded with success, false otherwise.</returns>
            public static bool IsOk(string host, int port)
            {
                return IsOk(host, port, null);
            }

            /// <summary>
            /// Returns true if the Redis server responded with success, false otherwise.
            /// </summary>
            /// <param name="host">Redis server host</param>
            /// <param name="port">Redis server port</param>
            /// <param name="auth">Redis password</param>
            /// <returns>True if the Redis server responded with success, false otherwise.</returns>
            public static bool IsOk(string host, int port, string auth)
            {
                try
                {
                    using (var socket = new ConnectedSocket(host, port))
                    {
                        if (auth != null)
                        {
                            socket.Send("AUTH " + auth + _lineTerminator);
                            if (socket.Receive() != ("+OK" + _lineTerminator)) return false;
                        }

                        socket.Send("PING" + _lineTerminator);
                        return socket.Receive() == ("+PONG" + _lineTerminator);
                    }
                }
                catch (SocketException)
                {
                    return false;
                }
            }
        }
    }
}

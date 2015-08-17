namespace ConnectionTests
{
    using SocketThat;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Net;
    using System.Net.Configuration;
    using System.Net.Sockets;

    public static partial class ConnectionTester
    {
        /// <summary>
        /// Issues a HELO to a SMTP server thus testing its connection.
        /// </summary>
        public static class SMTP
        {
            private const string _lineTerminator = "\r\n";

            private static readonly int[] _validReplyCodes = {
                // Non standard for "success"
                200,

                // Service ready
                220,

                // Requested mail action okay, completed
                250
            };

            /// <summary>
            /// Returns true if the SMTP server responded with success, false otherwise.
            /// Reads the SMTP server settings (host and port) from &lt;smtp&gt; in config.
            /// </summary>
            /// <returns>True if the SMTP server responded with success, false otherwise.</returns>
            /// <exception cref="ConfigurationErrorsException">If there's no SMTP settings in .config</exception>
            public static bool IsOk()
            {
                var cfg = ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection;
                if (cfg == null) throw new ConfigurationErrorsException("There's not SMTP configuration in .config!");
                return IsOk(cfg.Network.Host, cfg.Network.Port);
            }

            /// <summary>
            /// Returns true if the SMTP server responded with success, false otherwise.
            /// </summary>
            /// <param name="host">SMTP server host</param>
            /// <param name="port">SMTP server port</param>
            /// <returns>True if the SMTP server responded with success, false otherwise.</returns>
            public static bool IsOk(string host, int port)
            {
                try
                {
                    using (var socket = new ConnectedSocket(host, port))
                    {
                        // getting local hostname
                        var localhost = Dns.GetHostName();

                        // formatting HELO
                        var data = string.Format("HELO {0}{1}", localhost, _lineTerminator);

                        // fire in the hole!
                        socket.Send(data);

                        // getting an answer
                        var received = socket.Receive();

                        // sanitizing
                        var answers = SanitizeAnswer(received);

                        // checking answer's reply codes and returning it
                        return answers.Any(a => ValidReplyCode(a, localhost));
                    }
                }
                catch (SocketException)
                {
                    // something bad happened
                    return false;
                }
            }

            private static IEnumerable<string> SanitizeAnswer(string answer)
            {
                var lines = answer.Split(new [] { _lineTerminator }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines) yield return line.Trim().ToLower();
            }

            private static bool ValidReplyCode(string answer, string hostname)
            {
                var answerCode = answer.Split(new[] { ' ' }).First();
                return _validReplyCodes.Any(validCode => validCode.ToString() == answerCode);
            }
        }
    }
}

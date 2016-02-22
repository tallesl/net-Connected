namespace ConnectedLibrary
{
    using SocketLibrary;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Net;
    using System.Net.Configuration;
    using System.Net.Sockets;

    public static partial class Connected
    {
        private const string _smtpLineTerminator = "\r\n";

        private static readonly int[] _smtpValidReplyCodes = {
            // Non standard for "success"
            200,

            // Service ready
            220,

            // Requested mail action okay, completed
            250
        };

        /// <summary>
        /// Issues a HELO to a SMTP server thus testing its connection.
        /// Reads the SMTP server settings (host and port) from &lt;smtp&gt; in config.
        /// </summary>
        /// <returns>True if the SMTP server responded with success, false otherwise.</returns>
        /// <exception cref="ConfigurationErrorsException">If there's no SMTP settings in .config</exception>
        public static bool Smtp()
        {
            var cfg = ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection;

            if (cfg == null)
                throw new ConfigurationErrorsException("There's not SMTP configuration in .config!");
            else
                return Smtp(cfg.Network.Host, cfg.Network.Port);
        }

        /// <summary>
        /// Issues a HELO to a SMTP server thus testing its connection.
        /// </summary>
        /// <param name="endpoint">SMTP endpoint</param>
        /// <returns>True if the SMTP server responded with success, false otherwise.</returns>
        public static bool Smtp(EndPoint endpoint)
        {
            return Smtp(new ConnectedSocket(endpoint));
        }

        /// <summary>
        /// Issues a HELO to a SMTP server thus testing its connection.
        /// </summary>
        /// <param name="host">SMTP server host</param>
        /// <param name="port">SMTP server port</param>
        /// <returns>True if the SMTP server responded with success, false otherwise.</returns>
        public static bool Smtp(string host, int port)
        {
            return Smtp(new ConnectedSocket(host, port));
        }

        private static bool Smtp(ConnectedSocket socket)
        {
            try
            {
                using (socket)
                {
                    // getting local hostname
                    var localhost = Dns.GetHostName();

                    // formatting HELO
                    var data = string.Format("HELO {0}{1}", localhost, _smtpLineTerminator);

                    // fire in the hole!
                    socket.Send(data);

                    // getting an answer
                    var received = socket.Receive();

                    // sanitizing
                    var answers = SanitizeSmtpAnswer(received);

                    // checking answer's reply codes and returning it
                    return answers.Any(a => ValidSmtpReplyCode(a, localhost));
                }
            }
            catch (SocketException)
            {
                // something bad happened
                return false;
            }
        }

        private static IEnumerable<string> SanitizeSmtpAnswer(string answer)
        {
            return answer
                .Split(new [] { _smtpLineTerminator }, StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim().ToLower());
        }

        private static bool ValidSmtpReplyCode(string answer, string hostname)
        {
            var answerCode = answer.Split(new[] { ' ' }).First();
            return _smtpValidReplyCodes.Any(validCode => validCode.ToString() == answerCode);
        }
    }
}

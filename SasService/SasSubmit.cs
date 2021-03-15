using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SasService
{
    public class SasSubmit
    {
        private string server;
        private string host;
        private string port;
        private SasServer activeSession;

        public SasSubmit(string server, string host, string port)
        {
            this.server=server;
            this.host=host;
            this.port=port;

            activeSession = new SasServer();
            activeSession.Host = host;
            activeSession.Port = port;
            activeSession.Name = server;

            
        }

        public SasServer ActiveSession
        {
            get { return activeSession; }
            set
            {
                activeSession = SasServer.ServerConfig(this.server, this.host, this.port);
            }
        }

        public List<string> RunCode(string code)
        {            
            var log = new List<string>();

            try
            {
                activeSession.Connect();
                var langServ = activeSession.Workspace.LanguageService;

                langServ.Submit(code);
                Console.WriteLine("Running SAS Code...");

                SAS.LanguageServiceCarriageControl lscc;
                SAS.LanguageServiceLineType lslt;
                var cr = default(Array);
                var lt = default(Array);
                var logLines = default(Array);

                langServ.FlushLogLines(100000, out cr, out lt, out logLines);

                for (int i = 0; i < logLines.GetLength(0); i++)
                {
                    log.Add(logLines.GetValue(i).ToString());
                }

                Console.WriteLine("Closing session...");
                activeSession.Close();
            }
            catch(Exception ee)
            {
                log.Add(ee.Message);
                log.Add(ee.StackTrace);
            }

            return log;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Configuration;
using System.IO;

namespace SasService
{

    public class SasServer
    {
        /// <summary>
        /// Name of the server ("SASApp")
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Node name of the server
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// Port (number), such as 8591
        /// </summary>
        public string Port { get; set; }

        public string UserId { get; set; }


        // Use the ObjectKeeper, which keeps track of SAS Workspaces
        // We need this so that the OLE DB provider can find the workspace to
        // connect to if/when the user opens a data set to view
        internal static SASObjectManager.ObjectKeeper objectKeeper =
            new SASObjectManager.ObjectKeeper();

        /// <summary>
        /// Property for the SAS Workspace connection.
        /// Will connect if needed.
        /// </summary>
        public SAS.Workspace Workspace
        {
            get
            {
                if (_workspace == null)
                    Connect();

                if (_workspace != null)
                    return _workspace;
                else
                    throw new Exception("Could not connect to SAS Workspace");
            }
        }

        /// <summary>
        /// Is a Workspace connected?
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return _workspace != null;
            }
        }

        /// <summary>
        /// Close the Workspace if connected
        /// </summary>
        public void Close()
        {
            if (IsConnected) _workspace.Close();

            // clear out the ObjectKeeper
            objectKeeper.RemoveAllObjects();

            _workspace = null;
        }

        // Get the configuration for workspace server
        public static SasServer ServerConfig(string server, string host, string port)
        {
            SasServer s = new SasServer
            {
                Name = server,
                Host = host,
                Port = port
            };
            return s;
        }


        /// <summary>
        /// Connect to a SAS Workspace
        /// </summary>
        public void Connect()
        {
            if (_workspace != null)
                try
                {
                    Close();
                }
                catch { }
                finally
                {
                    _workspace = null;
                }


            // Connect to SAS server
            SASObjectManager.IObjectFactory2 obObjectFactory = new SASObjectManager.ObjectFactoryMulti2();
            SASObjectManager.ServerDef obServer = new SASObjectManager.ServerDef();
            obServer.MachineDNSName = Host;
            obServer.Protocol = SASObjectManager.Protocols.ProtocolBridge;
            obServer.Port = Convert.ToInt32(Port);
            obServer.ClassIdentifier = "440196d4-90f0-11d0-9f41-00a024bb830c";

            obServer.BridgeSecurityPackage = "Negotiate";
            obServer.BridgeSecurityPackageList = "Kerberos";

            _workspace = (SAS.Workspace)obObjectFactory.CreateObjectByServer(Name, true, obServer, null, null);

            objectKeeper.AddObject(1, Name, _workspace);

        }


        private SAS.Workspace _workspace = null;


    }
}


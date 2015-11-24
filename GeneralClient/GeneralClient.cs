/////////////////////////////////////////////////////////////////////////
// Client1.cs - CommService client sends and receives messages         //
// ver 2.1                                                             //
// Jim Fawcett, CSE681 - Software Modeling and Analysis, Project #4    //
/////////////////////////////////////////////////////////////////////////
//developed by XINGHE LU
/*
 * Additions to C# Console Wizard generated code:
 * - Added using System.Threading
 * - Added reference to ICommService, Sender, Receiver, Utilities
 *
 * Note:
 * - in this incantation the client has Sender and now has Receiver to
 *   retrieve Server echo-back messages.
 * - If you provide command line arguments they should be ordered as:
 *   remotePort, remoteAddress, localPort, localAddress
 */
/*
 * Maintenance History:
 * --------------------
 * ver 2.1 : 29 Oct 2015
 * - fixed bug in processCommandLine(...)
 * - added rcvr.shutdown() and sndr.shutDown() 
 * ver 2.0 : 20 Oct 2015
 * - replaced almost all functionality with a Sender instance
 * - added Receiver to retrieve Server echo messages.
 * - added verbose mode to support debugging and learning
 * - to see more detail about what is going on in Sender and Receiver
 *   set Utilities.verbose = true
 * ver 1.0 : 18 Oct 2015
 * - first release
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using static System.Console;
using System.Diagnostics;

namespace Project4
{
    using Util = Utilities;

    ///////////////////////////////////////////////////////////////////////
    // Client class sends and receives messages in this version
    // - commandline format: /L http://localhost:8085/CommService 
    //                       /R http://localhost:8080/CommService
    //   Either one or both may be ommitted

    class GeneralClient
    {
        string localUrl { get; set; } = "http://localhost:8083/CommService";
        string remoteUrl { get; set; } = "http://localhost:8080/CommService";

        //----< retrieve urls from the CommandLine if there are any >--------

        public void processCommandLine(string[] args)
        {
            if (args.Length == 0)
                return;
            localUrl = Util.processCommandLineForLocal(args, localUrl);
            remoteUrl = Util.processCommandLineForRemote(args, remoteUrl);
        }
        static void Main(string[] args)
        {
            Console.Write("\n  starting General Client for requirement #4");
            Console.Write("\n =============================\n");

            Console.Title = "GeneralClient for Requirement#4";

            GeneralClient clnt = new GeneralClient();
            clnt.processCommandLine(args);
            Console.Title = "General Client";

            string localPort = Util.urlPort(clnt.localUrl);
            string localAddr = Util.urlAddress(clnt.localUrl);
            Receiver rcvr = new Receiver(localPort, localAddr);
            if (rcvr.StartService())
            {
                rcvr.doService(rcvr.defaultServiceAction());
            }

            Sender sndr = new Sender(clnt.localUrl);  // Sender needs localUrl for start message

            Message msg = new Message();
            msg.fromUrl = clnt.localUrl;
            msg.toUrl = clnt.remoteUrl;

            Console.Write("\n  sender's url is {0}", msg.fromUrl);
            Console.Write("\n  attempting to connect to {0}\n", msg.toUrl);

            if (!sndr.Connect(msg.toUrl))
            {
                Console.Write("\n  could not connect in {0} attempts", sndr.MaxConnectAttempts);
                sndr.shutdown();
                rcvr.shutDown();
                return;
            }

            XmlDocument doc = new XmlDocument();
            doc.Load("generalClienttest.xml");
            XmlNodeList xmlnode = doc.GetElementsByTagName("Message");
            Stopwatch watch = new Stopwatch();

            watch.Start();//start watch
            while (true)//read and send messages
            {
                msg = new Message();
                msg.fromUrl = clnt.localUrl;
                msg.toUrl = clnt.remoteUrl;
                for (int i = 0; i < xmlnode.Count; i++)
                {
                    msg.content = "<?xml version=\"1.0\" encoding=\"utf - 8\" standalone=\"yes\"?>" + xmlnode[i].OuterXml;
                    Console.Write("\n  sending {0}\n", msg.content);
                    if (!sndr.sendMessage(msg))
                        return;
                    Thread.Sleep(100);
                }
                break;
            }
            msg.content = "done";
            sndr.sendMessage(msg);

            // Wait for user to press a key to quit.
            // Ensures that client has gotten all server replies.
            watch.Stop();
			string time = watch.ElapsedMilliseconds.ToString();
            Console.Write("Elapsed time: " + time + " millionsecondns\n\n");//print time elapsed
            Util.waitForUser();

            // shut down this client's Receiver and Sender by sending close messages
            rcvr.shutDown();
            sndr.shutdown();

            Console.Write("\n\n");
            
        }
    }
}

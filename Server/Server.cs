/////////////////////////////////////////////////////////////////////////
// Server.cs - CommService server                                      //
// ver 2.2                                                             //
// Jim Fawcett, CSE681 - Software Modeling and Analysis, Project #4    //
/////////////////////////////////////////////////////////////////////////
/*
 * Additions to C# Console Wizard generated code:
 * - Added reference to ICommService, Sender, Receiver, Utilities
 *
 * Note:
 * - This server now receives and then sends back received messages.
 */
/*
 * Plans:
 * - Add message decoding and NoSqlDb calls in performanceServiceAction.
 * - Provide requirements testing in requirementsServiceAction, perhaps
 *   used in a console client application separate from Performance 
 *   Testing GUI.
 */
/*
 * Maintenance History:
 * --------------------
 * ver 2.3 : 29 Oct 2015
 * - added handling of special messages: 
 *   "connection start message", "done", "closeServer"
 * ver 2.2 : 25 Oct 2015
 * - minor changes to display
 * ver 2.1 : 24 Oct 2015
 * - added Sender so Server can echo back messages it receives
 * - added verbose mode to support debugging and learning
 * - to see more detail about what is going on in Sender and Receiver
 *   set Utilities.verbose = true
 * ver 2.0 : 20 Oct 2015
 * - Defined Receiver and used that to replace almost all of the
 *   original Server's functionality.
 * ver 1.0 : 18 Oct 2015
 * - first release
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.Xml;
using System.Xml.Linq;


namespace Project4
{
  using Util = Utilities;

  class Server
  {
    string address { get; set; } = "localhost";
    string port { get; set; } = "8080";
	private DBEngine<int, DBElement<int, string>> db = new DBEngine<int, DBElement<int, string>>();
    //private DBEngine<int, DBElement<int, List<int>>> keysFromQuery = new DBEngine<int, DBElement<int, List<int>>>();

        //----< quick way to grab ports and addresses from commandline >-----
	public int editValue(XDocument message)
	{
		DBElement<int, string> temp = new DBElement<int, string>();
        XElement element = message.Element("Message").Element("Key");
        int key = Int32.Parse(element.Value);
		if(db.containsKey(key)){
			db.getValue(key, out temp);
			ItemEditor<int, string> editItem = new ItemEditor<int, string>(temp);
			element = message.Element("Message").Element("EditType");
			string str = element.Value;
			switch(str){
				case "nameEdit":
					element = message.Element("Message").Element("NewName");
					editItem.nameEdit(element.Value);
					break;
				case "descrEdit":
					element = message.Element("Message").Element("NewDescr");
					editItem.descrEdit(element.Value);
					break;
				case "timeUpdate":
					editItem.dateTimeEdit();
					break;
				case "addRelationship":
					element = message.Element("Message").Element("ChildKey");
					editItem.addRelationship(Int32.Parse(element.Value));
					break;
				case "deleteRelationship":
					element = message.Element("Message").Element("ChildKey");
					editItem.deleteRelationship(Int32.Parse(element.Value));
					break;
				case "payloadEdit":
					element = message.Element("Message").Element("NewPayload");
					editItem.payloadEdit(element.Value);
					break;
				case "replaceWithInstance":
					DBElement<int, string> elemNew = new DBElement<int, string>();
					editItem.replaceWithInstance(out elemNew);
					temp = null;
					break;
				default:
					Write("Invalid editing type.");
					break;
			}
		}
		else
			key = -1;
		return key;
	}
	
	public int deleteData(XDocument message)
	{
		XElement element = message.Element("Message").Element("Key");
		int key = Int32.Parse(element.Value);
		db.delete(key);
		return key;
	}
	
    public int addValue(XDocument message)//method used to do addition of key/value pairs
	{
	  XElement element = message.Element("Message").Element("Name");
	  DBElement<int, string> elem = new DBElement<int, string>();
      elem.name = element.Value;//add name
	  element = message.Element("Message").Element("descr");//add description
      elem.descr = element.Value;
      elem.timeStamp = DateTime.Now;//add time
	  var p = from x in 
                message.Elements("Message")
                .Elements("Children").Descendants()
              select x;
	  foreach (var child in p)//add children's keys
		elem.children.Add(Int32.Parse(child.Value));
	  element = message.Element("Message").Element("Payload");//add payload
      elem.payload = element.Value;
      //elem.showElement();
	  element = message.Element("Message").Element("Key");//add description
	  int key = Int32.Parse(element.Value);
      db.insert(key, elem);
	  //Write("\n\n Show key/value pairs in data base:\n");
      //db.showDB();
      return key;
	}
    
    public void showDB()
	{
		db.showDB();
	}
	
    public void ProcessCommandLine(string[] args)
    {
      if (args.Length > 0)
      {
        port = args[0];
      }
      if (args.Length > 1)
      {
        address = args[1];
      }
    }
    static void Main(string[] args)
    {
      Util.verbose = false;
      Server srvr = new Server();
      srvr.ProcessCommandLine(args);

      Console.Title = "Server";
      Console.Write(String.Format("\n  Starting CommService server listening on port {0}", srvr.port));
      Console.Write("\n ====================================================\n");

      Sender sndr = new Sender(Util.makeUrl(srvr.address, srvr.port));
      //Sender sndr = new Sender();
      Receiver rcvr = new Receiver(srvr.port, srvr.address);
      //srvr.addValue(2);
            
		// - serviceAction defines what the server does with received messages
		// - This serviceAction just announces incoming messages and echos them
		//   back to the sender.  
		// - Note that demonstrates sender routing works if you run more than
		//   one client.

      Action serviceAction = () =>
      {
        Message msg = null;
		int key;
		
        while (true)
        {
          msg = rcvr.getMessage();   // note use of non-service method to deQ messages
		  Message testMsg = new Message();
          Console.Write("\n  Received message:");
          Console.Write("\n  sender is {0}", msg.fromUrl);
		  Console.Write("\n  Content is: {0}", msg.content);
          if (msg.content.StartsWith("<"))//handling regular message
          {
               XDocument xml = XDocument.Parse(msg.content);
			   XElement element = xml.Element("Message").Element("Type");
			   string type = element.Value;
			   switch(type){
					case "Add":
						key = srvr.addValue(xml);
						testMsg.content = "Value with key "+ key +" has been added";
						break;
					case "Delete":
						key = srvr.deleteData(xml);
						testMsg.content = "Value with key "+ key +" has been deleted";
						break;
					case "Edit":
						key = srvr.editValue(xml);
						if(key<0)
							testMsg.content = "Value cannot be found";
						else
							testMsg.content = "Value with key "+ key +" has be edited";
                          break;
					default:
						testMsg.content = "Invalid request.";
						break;
			   }
          }

          if (msg.content == "connection start message")
          {
            continue; // don't send back start message
          }
          if (msg.content == "done")
          {
            Console.Write("\n  client has finished\n");
            continue;
          }
          if (msg.content == "closeServer")
          {
            Console.Write("received closeServer");
            break;
          }
          msg.content = "received " + msg.content + " from " + msg.fromUrl;

          // swap urls for outgoing message
          Util.swapUrls(ref msg);

#if (TEST_WPFCLIENT)
              /////////////////////////////////////////////////
              // The statements below support testing the
              // WpfClient as it receives a stream of messages
              // - for each message received the Server
              //   sends back 1000 messages
              //
				testMsg.toUrl = msg.toUrl;
				testMsg.fromUrl = msg.fromUrl;
				Console.Write("\n  sending reply: {0}", testMsg.content);
				WriteLine();
				sndr.sendMessage(testMsg);
              
#else
          /////////////////////////////////////////////////
          // Use the statement below for normal operation
          sndr.sendMessage(msg);
#endif
        }
      };

      if (rcvr.StartService())
      {
        rcvr.doService(serviceAction); // This serviceAction is asynchronous,
      }                                // so the call doesn't block.
      Util.waitForUser(); 
	  
	  srvr.showDB();
    }
  }
}

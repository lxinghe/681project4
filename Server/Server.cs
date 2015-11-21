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
	private DBEngine<string, DBElement<int, string>> dbString = new DBEngine<string, DBElement<int, string>>();
    //private DBEngine<int, DBElement<int, List<int>>> keysFromQuery = new DBEngine<int, DBElement<int, List<int>>>();

        
	public string query(XDocument message)	{//method used to handle queries
		string reply;
		List<int> children = new List<int>();
		XElement element = message.Element("Message").Element("QueryType");
		Query<int, int, string> query1 = new Query<int, int, string>(db);
		switch(element.Value){
				case "the value of specified key":
					element = message.Element("Message").Element("Key");
				    DBElement<int, string> elem = new DBElement<int, string>();
				    query1.checkValueByKey(Int32.Parse(element.Value), out elem);
					reply = ("The value of specified key " + element.Value + " is\n" + elem.showElement<int, string>());
					break;
				case "the children of specified key":
					element = message.Element("Message").Element("Key");
					children = query1.childrenByKey(Int32.Parse(element.Value));
					reply = ("The children of specified key " + element.Value + " is\n");
					reply = this.addChildToStr(children, reply);
					break;
				case "the keys share a same pattern":
					element = message.Element("Message").Element("Pattern");
					Query<string, int, string> queryString = new Query<string, int, string>(dbString);
					List<string> keyString = new List<string>();
					keyString = queryString.keysWithPattern(dbString,element.Value);
					reply = ("The keys share a same pattern \"" + element.Value + "\" is\n");
					foreach(var key in keyString)
						reply += (String.Format("{0}\n", key.ToString()));
					break;
				case "the keys share same pattern in their metadata":
					element = message.Element("Message").Element("Pattern");
					children = query1.keysSameMdataPattern(element.Value);
					reply = ("The keys share same pattern " + element.Value + " is\n");
					reply = this.addChildToStr(children, reply);
					break;
				case "the keys of value created in the same time interval":
					List<DateTime> dts = new List<DateTime>();
					dts = this.getDTS(message);
					children = query1.keysSameTinterval(dts[0], dts[1]);
					reply = ("The keys of value created in the same time interval between " + dts[0].ToString() + " and " + dts[1]).ToString()+"\n";
					reply = this.addChildToStr(children, reply);
					break;
				default:
                    reply = ("Invalid editing type.");
					break;
			}
		return reply;
	}
	
	public List<DateTime> getDTS(XDocument mesg){
		List<DateTime> dts = new List<DateTime>();
		int year, month, date, hour, min, sec;
		XElement element = mesg.Element("Message").Element("Year1");
		year = Int32.Parse(element.Value);
		element = mesg.Element("Message").Element("Month1");
		month = Int32.Parse(element.Value);
		element = mesg.Element("Message").Element("Date1");
		date = Int32.Parse(element.Value);
		element = mesg.Element("Message").Element("Hour1");
		hour = Int32.Parse(element.Value);
		element = mesg.Element("Message").Element("Min1");
		min = Int32.Parse(element.Value);
		element = mesg.Element("Message").Element("Sec1");
		sec = Int32.Parse(element.Value);
		DateTime dt1 = new DateTime(year, month, date, hour, min, sec);
		dts.Add(dt1);//add datetime #1
		element = mesg.Element("Message").Element("Year2");
		year = Int32.Parse(element.Value);
		element = mesg.Element("Message").Element("Month2");
		month = Int32.Parse(element.Value);
		element = mesg.Element("Message").Element("Date2");
		date = Int32.Parse(element.Value);
		element = mesg.Element("Message").Element("Hour2");
		hour = Int32.Parse(element.Value);
		element = mesg.Element("Message").Element("Min2");
		min = Int32.Parse(element.Value);
		element = mesg.Element("Message").Element("Sec2");
		sec = Int32.Parse(element.Value);
		DateTime dt2 = new DateTime(year, month, date, hour, min, sec);
		dts.Add(dt2);//add datetime #1
		return dts;
	}
	
	public string addChildToStr(List<int> list, string str){
		string astr = str;
		List<int> children = new List<int>();
        children = list;
		foreach (var child in children)
          astr += (String.Format("{0}\n", child.ToString()));
		return astr;
	}
	
	public void recoverDB(XDocument message){//method used to recover a db from a xml file
		XElement element = message.Element("Message").Element("File");
		LoadXML fromxml = new LoadXML(db, element.Value);
		fromxml.WriteToDBEngine();
	}
	
	public void persistDB(XDocument message){//method used to persist a database to xml file
		XElement element = message.Element("Message").Element("File");
		PersistToXML toxml  = new PersistToXML(db);
		toxml.writeXML(element.Value);
		//toxml.displayXML();
		toxml.cleanDB();
	}
	
	public int editValue(XDocument message){//method used to do value edition
		DBElement<int, string> temp = new DBElement<int, string>();
        XElement element = message.Element("Message").Element("Key");
        int key = Int32.Parse(element.Value);
		if(db.containsKey(key)){
			db.getValue(key, out temp);
			ItemEditor<int, string> editItem = new ItemEditor<int, string>(temp);
			element = message.Element("Message").Element("EditType");
			switch(element.Value){
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
	
	public int deleteData(XDocument message)//method used to do value deletion
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
	  element = message.Element("Message").Element("KeyType");
	  int key = -1;
	  if(element.Value.Equals("int")){
		  element = message.Element("Message").Element("Key");
		  key = Int32.Parse(element.Value);
		  db.insert(key, elem);
	  }
	  else{
		element = message.Element("Message").Element("Key");
		dbString.insert(element.Value, elem);
	  }
      return key;
	}
    
    public void showDB()
	{
		db.showDB();
	}
	//----< quick way to grab ports and addresses from commandline >-----
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
			   switch(element.Value){
					case "Add":
						key = srvr.addValue(xml);
						if(key>=0)
							testMsg.content = "Value with key "+ key +" has been added";
						else{
							element = xml.Element("Message").Element("Key");
							testMsg.content = "Value with key "+ element.Value +" has been added";
						}
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
					case "ToXML":
						srvr.persistDB(xml);
						testMsg.content = "The content of database has been converted to XML and saved in Test.xml";
						break;
					case "RecoverDB":
						srvr.recoverDB(xml);
						testMsg.content = "The database has been recover from a XML file";
						break;
					case "Query":
						testMsg.content = srvr.query(xml);
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
				sndr.sendMessage(testMsg);//sending replies 
              
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

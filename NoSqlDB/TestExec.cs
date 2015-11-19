/*
Programmer: Xinghe Lu
Date 10/01/2015
Purpose: Used to test every requirement
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
  class TestExec
  {
    private DBEngine<int, DBElement<int, string>> db = new DBEngine<int, DBElement<int, string>>();
	private DBEngine<string, DBElement<int, string>> dbString = new DBEngine<string, DBElement<int, string>>();
	private DBEngine<int, DBElement<int, List<int>>> keysFromQuery = new DBEngine<int, DBElement<int, List<int>>>();

        void TestR2()
    {
      "Demonstrating Requirement #2".title();
      DBElement<int, string> elem = new DBElement<int, string>();
      elem.name = "element";
      elem.descr = "test element";
      elem.timeStamp = DateTime.Now;
      elem.children.AddRange(new List<int>{ 1, 2, 3 });
      elem.payload = "elem's payload";
      elem.showElement();
      db.insert(1, elem);
	  Write("\n\n Show key/value pairs in data base:\n");
      db.showDB();
      WriteLine();
    }
	
    void TestR3()//addition and deletion of key/value pairs
    {
      "Demonstrating Requirement #3".title();
	  WriteLine();
	  
	  Write("\n --- Test addition of key/value pairs Start---");
	  WriteLine();
	  DBElement<int, string> elem1 = new DBElement<int, string>();
      elem1.name = "element#1";//add a new key/value pairs
      elem1.descr = "test element#1";
      elem1.timeStamp = DateTime.Now;
      elem1.children.AddRange(new List<int>{ 6, 8 });
      elem1.payload = "elem#1's payload";
      elem1.showElement();
      db.insert(2, elem1);
	  Write("\n\n Show key/value pairs in data base:\n");
      db.showDB();
      WriteLine();
	  Write("\n --- Test addition of key/value pairs End---");
	  WriteLine();
	  
	  Write("\n --- Test deletion of key/value pairs Start---");
	  WriteLine();
	  db.delete(1);//delete an existing key/value pairs
	  Write("\n\n Show key/value pairs in data base:\n");
      db.showDB();
      WriteLine();
      db.delete(100);//try to delete a key/value pairs that doesn't exist
	  Write("\n --- Test deletion of key/value pairs End---");
      WriteLine();
    }
	
    void TestR4(){//support editing of value including the addition and/or deletion of relationships, 
                  //editing text metadata and replacing an existing value's instance with a new instance

            "Demonstrating Requirement #4".title();

        DBElement<int, string> temp = new DBElement<int, string>();
        ItemEditor<int, string> editItem;
		
        if (db.containsKey(2)){
            db.getValue(2, out temp);
			Write("\n\n --- value before modified---\n");
			temp.showElement();
            editItem = new ItemEditor<int, string>(temp);
			editItem.nameEdit("newName!!");//edit the name of the value with key 2
			editItem.descrEdit("new description!!");//edit description
			editItem.dateTimeEdit();//update timeStamp
			editItem.addRelationship(18);//add relationship
			editItem.deleteRelationship(6);//delete relationship
			editItem.payloadEdit("new payload!!");//modify payload
			
			DBElement<int, string> elemNew = new DBElement<int, string>();
			editItem.replaceWithInstance(out elemNew);// replace an existing value's instance with a new instance
			temp = null;
			Write("\n\n --- value after modified---\n");
			elemNew.showElement();
            editItem = null;
        }

        else
			Write("Value not found!");
		
      //Write("\n\n Show key/value pairs in data base:\n");
	  //db.showDB();
      WriteLine();
      WriteLine();
    }
	
    void TestR5()
    {
      "Demonstrating Requirement #5".title();
	  DBElement<int, string> elem2 = new DBElement<int, string>();
      elem2.name = "element#2";//add a new key/value pairs
      elem2.descr = "test element#2";
      elem2.timeStamp = DateTime.Now;
      elem2.children.AddRange(new List<int>{ 16, 48 });
      elem2.payload = "elem#2's payload";
      db.insert(7, elem2);
	 
	  PersistToXML toxml  = new PersistToXML(db);
	  toxml.writeXML("Test.xml");
	  toxml.displayXML();
	  toxml.cleanDB();
	  
	  Write("\n --- Test read XML file Start---");
      LoadXML fromxml = new LoadXML(db, "ReadFile.xml");
	  fromxml.WriteToDBEngine();
	  
	  Write("\n\n Show key/value pairs in data base:\n");
      db.showDB();
	  
	  Write("\n --- Test read XML file End---");
      WriteLine();
    }
	
    void TestR6()
    {
      "Demonstrating Requirement #6".title();
	  Write("\n\n  press any key to stop scheduled save\n");
	  Scheduler ts = new Scheduler(db);
	  ts.schedular.Enabled = true;
	  Console.ReadKey();
      WriteLine();
    }
	
    void TestR7()
    {
      "Demonstrating Requirement #7".title();
	  
	  Write("\n\n --- Query the value of specified key Start---\n");
	  Query<int, int, string> query = new Query<int, int, string>(db);
	  DBElement<int, string> elemR7 = new DBElement<int, string>();
	  query.checkValueByKey(2, out elemR7);
	  elemR7.showElement();
	  Write("\n\n --- Query the value of specified key End---\n");
      WriteLine();
	  /*****************************************************************/
	  Write("\n\n --- Query the children of specified key Start---\n");
	  Write("Children of value with key #2");
	  List<int> children = new List<int>();
	  children = query.childrenByKey(2);
	  foreach(var child in children)
	  Write("\n"+child);
	  Write("\n\n --- Query the children of specified key End---\n");
	  /*****************************************************************/
	  Write("\n\n --- Query the keys share a same pattern Start---\n");
	  DBElement<int, string> elemR701 = new DBElement<int, string>();
      elemR701.name = "r701";
      elemR701.descr = "su cs";
      elemR701.timeStamp = DateTime.Now;
      elemR701.children.AddRange(new List<int>{ 1, 2, 3 });
      elemR701.payload = "cs";
      dbString.insert("SU cs dep", elemR701);
	  DBElement<int, string> elemR702 = new DBElement<int, string>();
      elemR702.name = "r702";
      elemR702.descr = "su math";
      elemR702.timeStamp = DateTime.Now;
      elemR702.children.AddRange(new List<int>{ 4, 5, 2 });
      elemR702.payload = "math";
      dbString.insert("SU math dep", elemR702);
	  DBElement<int, string> elemR703 = new DBElement<int, string>();
      elemR703.name = "r703";
      elemR703.descr = "cornell math";
      elemR703.timeStamp = DateTime.Now;
      elemR703.children.AddRange(new List<int>{ 4, 5, 2 });
      elemR703.payload = "music";
      dbString.insert("CORNELL music dep", elemR703);
	  IEnumerable<string> keys = dbString.Keys();
	  Write("All keys in this database:");
	  foreach(var key in keys)
		Write("\n"+key);
	  Query<string, int, string> queryString = new Query<string, int, string>(dbString);
	  List<string> keyString = new List<string>();
	  keyString = queryString.keysWithPattern(dbString,"SU");
	  Write("\n\nKeys with pattern SU:");
	  foreach(var key in keyString)
	  	 Write("\n"+key);
	  Write("\n\n --- Query the keys share a same pattern End---\n");
	  /*****************************************************************/
	  Write("\n\n --- Query the keys share a same pattern Start---\n");
	  db.showDB();
	  Write("\n\nKeys with pattern ‘description’ in there metadata(name & descr):");
	  List<int> keyslist = new List<int>();
	  keyslist = query.keysSameMdataPattern("description");
	  foreach(var key in keyslist)
	  	 Write("\n"+key);
	  Write("\n\n --- Query the keys share a same pattern End---\n");
	  /*****************************************************************/
	  Write("\n\n --- Query the keys of value created in the same time interval Start---\n");
	  DBElement<int, string> temp = new DBElement<int, string>();
	  db.getValue(7, out temp);
	  temp.timeStamp = new DateTime(1991, 4, 15, 8, 9, 05);
	  db.getValue(82, out temp);
	  temp.timeStamp = new DateTime(1994, 9, 7, 10, 8, 55);
	  db.showDB();
	  DateTime dt1 = new DateTime(1990, 4, 15, 5, 9, 05);
	  DateTime dt2 = new DateTime(1995, 9, 7, 5, 8, 05);
	  Write("\n\nkeys of value created after 1990/4/15 05:09:05 but before 1995/9/7 05:08:05:");
	  keyslist = query.keysSameTinterval(dt1, dt2);
	  foreach(var key in keyslist)
	  	 Write("\n"+key);
	  Write("\n\n --- Query the keys of value created in the same time interval Start---\n");
    }
	
    void TestR8()
    {
      "Demonstrating Requirement #8".title();
	  Query<int, int, string> query = new Query<int, int, string>(db);
	  List<int> keyslist = new List<int>();
	  DBElement<int, string> temp = new DBElement<int, string>();
	  db.getValue(7, out temp);
	  temp.timeStamp = new DateTime(1991, 4, 15, 8, 9, 05);
	  db.getValue(82, out temp);
	  temp.timeStamp = new DateTime(1994, 9, 7, 10, 8, 55);
	  db.showDB();
	  DateTime dt1 = new DateTime(1990, 4, 15, 5, 9, 05);
	  DateTime dt2 = new DateTime(1995, 9, 7, 5, 8, 05);
	  Write("\n\nQuery: keys of value created after 1990/4/15 05:09:05 but before 1995/9/7 05:08:05:");
	  keyslist = query.keysSameTinterval(dt1, dt2);
	  
		 
	  QueryKeysDB<int> queryStore = new QueryKeysDB<int>(keysFromQuery, keyslist);
	  queryStore.storeKeys(1, "key set #1", "keys of value created after 1990/4/15 05:09:05 but before 1995/9/7 05:08:05");
	  
	  DBElement<int, List<int>> temp2 = new DBElement<int, List<int>>();
	  
	  keysFromQuery.getValue(1, out temp2);
	  List<int> keyslist2 = new List<int>();
	  keyslist2 = temp2.payload;
	  Write("\n\nRead keys returned from the query in the new database\n");
	  foreach(var key in keyslist2)
	  	 Write("\n"+key);
	  
      WriteLine();
    }
	
	void TestR9()
    {
		"Demonstrating Requirement #9".title();
		XDocument xml = new XDocument();
		xml.Declaration = new XDeclaration("1.0", "utf-8", "yes");
		XElement project2 = new XElement("project2");
		xml.Add(project2);//root
		/*************DBElement**************************/
		XElement DBElement = new XElement("DBElement");
		project2.Add(DBElement);
		XElement Reference = new XElement("Reference", "UtilityExtensions");
		DBElement.Add(Reference);
		/*************DBElementTest**************************/
		XElement DBElementTest = new XElement("DBElementTest");
		project2.Add(DBElementTest);
		XElement Reference1 = new XElement("Reference", "DBExtensions");
		DBElementTest.Add(Reference1);
		XElement Reference2 = new XElement("Reference", "DBElement");
		DBElementTest.Add(Reference);
		DBElementTest.Add(Reference);
		/*************DBEngine**************************/
		XElement DBEngine = new XElement("DBEngine");
		project2.Add(DBEngine);
		DBEngine.Add(Reference);
		DBEngine.Add(Reference2);
		/*************DBEngine**************************/
		XElement DBEngineTest = new XElement("DBEngineTest");
		project2.Add(DBEngineTest);
		DBEngineTest.Add(Reference2);
		DBEngineTest.Add(Reference1);
		DBEngineTest.Add(Reference);
		XElement Reference3 = new XElement("Reference", "DBEngine");
		DBEngineTest.Add(Reference3);
		/*************DBExtensions**************************/
		XElement DBExtensions = new XElement("DBExtensions");
		project2.Add(DBExtensions);
		DBExtensions.Add(Reference2);
		DBExtensions.Add(Reference);
		DBExtensions.Add(Reference3);
		/*************Display**************************/
		XElement Display = new XElement("Display");
		project2.Add(Display);
		Display.Add(Reference2);
		Display.Add(Reference);
		Display.Add(Reference3);
		Display.Add(Reference1);
		/*************ItemEditor**************************/
		XElement ItemEditor = new XElement("ItemEditor");
		project2.Add(ItemEditor);
		ItemEditor.Add(Reference2);
		/*************LoadXML**************************/
		XElement LoadXML = new XElement("LoadXML");
		project2.Add(LoadXML);
		LoadXML.Add(Reference3);
		LoadXML.Add(Reference2);
		/*************PayloadWrapper**************************/
		XElement PayloadWrapper = new XElement("PayloadWrapper");
		project2.Add(PayloadWrapper);
		PayloadWrapper.Add(Reference2);
		PayloadWrapper.Add(Reference);
		PayloadWrapper.Add(Reference1);
		/*************PersistToXML**************************/
		XElement PersistToXML = new XElement("PersistToXML");
		project2.Add(PersistToXML);
		PersistToXML.Add(Reference3);
		PersistToXML.Add(Reference2);
		/*************Query**************************/
		XElement Query = new XElement("Query");
		project2.Add(Query);
		Query.Add(Reference3);
		Query.Add(Reference2);
		/*************QueryKeysDB**************************/
		XElement QueryKeysDB = new XElement("QueryKeysDB");
		project2.Add(QueryKeysDB);
		XElement Reference4 = new XElement("Reference", "PersistToXML");
		QueryKeysDB.Add(Reference3);
		QueryKeysDB.Add(Reference2);
		QueryKeysDB.Add(Reference4);
		/*************Scheduler**************************/
		XElement Scheduler = new XElement("Scheduler");
		project2.Add(Scheduler);
		Scheduler.Add(Reference3);
		Scheduler.Add(Reference2);
		Scheduler.Add(Reference4);
		/*************TestExec**************************/
		XElement TestExec = new XElement("TestExec");
		project2.Add(TestExec);
		TestExec.Add(Reference2);
		TestExec.Add(Reference3);
		XElement Reference5 = new XElement("Reference", "Display");
		TestExec.Add(Reference5);
		XElement Reference6 = new XElement("Reference", "ItemEditor");
		TestExec.Add(Reference6);
		XElement Reference7 = new XElement("Reference", "LoadXML");
		TestExec.Add(Reference7);
		TestExec.Add(Reference4);
		XElement Reference8 = new XElement("Reference", "Query");
		TestExec.Add(Reference8);
		XElement Reference9 = new XElement("Reference", "QueryKeysDB");
		TestExec.Add(Reference9);
		XElement Reference10 = new XElement("Reference", "Scheduler");
		TestExec.Add(Reference10);
		TestExec.Add(Reference);
		/*************TestExec**************************/
		XElement UtilityExtensions = new XElement("UtilityExtensions");
		project2.Add(UtilityExtensions);
		
		
		xml.Save("Content.xml");
		Write("\n{0}\n", xml.Declaration);
			Write(xml.ToString());
			Write("\n\n");
	}
	
    static void Main(string[] args)
    {
      TestExec exec = new TestExec();
      "Demonstrating Project#2 Requirements".title('=');
      WriteLine();
      exec.TestR2();
      exec.TestR3();
      exec.TestR4();
      exec.TestR5();
      exec.TestR6();
      exec.TestR7();
      exec.TestR8();
	  exec.TestR9();
      Write("\n\n");
    }
	
  }

}

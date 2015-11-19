/*
Programmer: Xinghe Lu
Course: CIS681
Date: 10/01/2015
Purpose: this program will store key/value pairs in a dictionary to a XML file.
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
    
	public class PersistToXML 
	{
		private DBEngine<int, DBElement<int, string>> db1;
		XDocument xml = new XDocument();
		
		public PersistToXML(DBEngine<int, DBElement<int, string>> db0)
		{
			db1 = new DBEngine<int, DBElement<int, string>>();
			db1 = db0;
		}
		
		public void writeXML(string path)
		{	
			if(db1.emptyDictionary())
				Write("\n\nThe database is empty.\n");
		
			else{//if the dictionary is not empty store key/value pairs into a XML file
			
					DBElement<int, string> temp = new DBElement<int, string>();
					IEnumerable<int> keys = db1.Keys();
					
					xml.Declaration = new XDeclaration("1.0", "utf-8", "yes");
					
					XElement noSqlDb = new XElement("noSqlDb");
					xml.Add(noSqlDb);
					
					XElement keytype = new XElement("keytype", "int");
					XElement payloadtype = new XElement("payloadtype", "string");
					noSqlDb.Add(keytype);
					noSqlDb.Add(payloadtype);
					
					foreach(var i in keys)
					{
						db1.getValue(2, out temp);
						XElement key = new XElement("key", ("key"+i));
						noSqlDb.Add(key);//noSqlDb's children
						XElement element = new XElement("element");
						noSqlDb.Add(element);//noSqlDb's children 
						
						XElement name = new XElement("name", temp.name);//name of the element
						element.Add(name);
						XElement descr = new XElement("descr", temp.descr);//description of the element
						element.Add(descr);
						XElement timeStamp = new XElement("timeStamp", temp.timeStamp);//timeStamp of the element
						element.Add(timeStamp);
						
						if(temp.children.Count!=0)//children of the element
						{
							XElement children = new XElement("children");
							element.Add(children);
							foreach(var j in temp.children)
							{
								XElement childkey = new XElement("key", ("key"+j));
								children.Add(childkey);
							}
						}
						
						XElement payload = new XElement("payload", temp.payload);//payload of the element
						element.Add(payload);
					}
					
					
					xml.Save(path);
				}
		}
		
		public void cleanDB()
		{
			db1.emptyDBEngine();//empty DBEngine
		}
		
		public void displayXML()
		{
			Write("\n{0}\n", xml.Declaration);
			Write(xml.ToString());
			Write("\n\n");
		}
	}
	
	/*class Program
	{
		static void Main(string[] args)
		{
		}
	}*/
}

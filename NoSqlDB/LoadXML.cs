/*
Programmer: Xinghe Lu
course: CIS681
Date: 10/01/2015
Purpose: This program will read a XML which contains the key/value pairs to database

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using static System.Console;

namespace Project4
{
    public class LoadXML
    {
		private XDocument xml;
		private DBEngine<int, DBElement<int, string>> db;
		private DBElement<int, string> temp;
		
        public LoadXML(DBEngine<int, DBElement<int, string>> database, string path)
		{
			db = database;
			xml = XDocument.Load(path);//read xml
			Write(xml.ToString()); 
		}
		
		public void WriteToDBEngine()
		{
			List<int> keyList = new List<int>();
			var q = from x in 
                xml.Elements("noSqlDb")
                .Elements("key")
              select x;

		    foreach (var elem in q)
			{
				int key = Int32.Parse(elem.Value.Substring(3));//read all keys and store them to a list
				keyList.Add(key);
			}
			
			int i = 0;
			
			var p = from x in 
                xml.Elements("noSqlDb")
                .Elements("element")
              select x;

		    foreach (var elem in p)
			{
				temp = new DBElement<int, string>();  
                temp.name = (elem as XElement).Descendants("name").First().Value;//read and store name to element
                temp.descr = (elem as XElement).Descendants("descr").First().Value;//read and store description to element
                temp.timeStamp = Convert.ToDateTime((elem as XElement).Descendants("timeStamp").First().Value);//read and store timeStamp to element
                temp.payload = (elem as XElement).Descendants("payload").First().Value;//read payload and store it to element
				
				var a = from x in
                            (elem as XElement).Elements("children").Descendants()
                                    select x;
				foreach (var child in a)//store key/value pairs into database
                {
                    temp.children.Add(Int32.Parse(child.Value.Substring(3)));
                }
				
				db.insert(keyList[i], temp);
				i++;
			}
		}
    }
	
	/*class Program
	{
		static void Main(string[] args)
		{
		}
	}*/
}

/*
Programmer: Xinghe Lu
course: CIS681
Date: 10/01/2015
Purpose: This program accepts a positive time interval or number of writes after which the database contents are persisted.

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Timers;

namespace Project4
{
    public class Scheduler
    {
		
		public Timer schedular { get; set; } = new Timer();

		public Scheduler(DBEngine<int, DBElement<int, string>> db)
		{
			
			schedular.Interval = 1000; // save interval is 1 second
			schedular.AutoReset = true;
			schedular.Elapsed += (object source, ElapsedEventArgs e) =>
			{
				PersistToXML toxml = new PersistToXML(db);
				Console.Write("\n  myDBXml.xml was saved at {0}", e.SignalTime);
				toxml.writeXML("myDBXml.xml");
			};
		}

		
	}
	
	/*class Program
	{
		static void Main(string[] args)
		{
		}
	}*/
    
}

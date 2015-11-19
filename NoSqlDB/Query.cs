/*
Programmer: Xinghe Lu
course: CIS681
Date: 10/01/2015
Purpose: This program support queries for 1. the value of a specified key
										  2. the children of a specified key
										  3. the set of all keys matching a specified pattern which defaults to all keys
										  4. all keys that contain a specified string in their metadata section
										  5. all keys that contain values written within a specified time-date interval. 

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace Project4
{
    public class Query<Key1, Key2, Data>
    {
		private DBEngine<Key1, DBElement<Key2, Data>> db;
		private DBElement<Key2, Data> temp = new DBElement<Key2, Data>();
		
        public Query(DBEngine<Key1, DBElement<Key2, Data>> database)
		{
			db = new DBEngine<Key1, DBElement<Key2, Data>>();
			db = database;
		}
		
		public bool checkValueByKey(Key1 key, out DBElement<Key2, Data> val)//the value of a specified key
		{
			return db.getValue(key, out val);
		}
		
		public List<Key2> childrenByKey(Key1 key)//the children of a specified key
		{
				db.getValue(key, out temp);
				return temp.children;
		}
		
		public List<string> keysWithPattern(DBEngine<string, DBElement<Key2, Data>> db1, string pattern)//the set of all keys matching a specified pattern which defaults to all keys
		{
			IEnumerable<string> keys = db1.Keys();
			List<string> keylist = new List<string>();
			
			foreach(var key in keys)
			{
				if(key.Contains(pattern))
					keylist.Add(key);
			}
			
			return keylist;
		}
		
		public List<Key1> keysSameMdataPattern(string pattern)//all keys that contain a specified string in their metadata section
		{
			IEnumerable<Key1> keys = db.Keys();
			List<Key1> keylist = new List<Key1>();
			
			foreach(var key in keys)
			{
				db.getValue(key, out temp);
				if(temp.name.Contains(pattern)||temp.descr.Contains(pattern))
					keylist.Add(key);
			}
		
			return keylist;
		}
		
		public List<Key1> keysSameTinterval(DateTime t1, DateTime t2)//all keys that contain values written within a specified time-date interval.
		{
			List<Key1> keylist = new List<Key1>();
			IEnumerable<Key1> keys = db.Keys();
			
			foreach(var key in keys)
			{
				db.getValue(key, out temp);
				if((DateTime.Compare(temp.timeStamp,t1)>=0)&&(DateTime.Compare(temp.timeStamp,t2)<=0))
					keylist.Add(key);
			}
			
			return keylist;
		}
    }

    /*class Program
    {
        static void Main(string[] args)
        {
        }
    }*/
}

/*

Programmer: Xinghe Lu
Date: 10/01/2015
Course: CIS681
purpose: this program helps to modify the value in database

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;


namespace Project4
{
    public class ItemEditor<Key, Data>
    {
        private DBElement<Key, Data> element;
		
		public ItemEditor(DBElement<Key, Data> val){//let element points to the value's instance 

            element = new DBElement<Key, Data>();
            element = val;
        }

        public void nameEdit(string newName){//modify name 
	
			element.name = newName;
		}
		
		public void descrEdit(string newDescr){//modify description of the value
			
			element.descr = newDescr;
		}
		
		public void dateTimeEdit(){//update timeStamp
			
			element.timeStamp = DateTime.Now;
		}
		
		public void addRelationship(Key key){//add a relationship
			
			element.children.Add(key);
		}
		
		public void deleteRelationship(Key key){//delete a relationship
			
			bool removed = element.children.Remove(key);
			if(!removed)
				Write("\n\nChild not found!\n\n");
		}
		
		public void payloadEdit(Data newPayload){//modify payload
			
			element.payload = newPayload;
		}
		
		public void replaceWithInstance(out DBElement<Key, Data> newElem){
			newElem = element;
		}
    }

    /*class Program
    {
        static void Main(string[] args)
        {
        }
    }*/


}

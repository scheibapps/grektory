using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;

namespace GrektoryApp.Objects
{
    class DB
    {
        private string name;
        private string val;
        private string public_key;
        private string private_key;

        public DB(string name, string val, string public_key, string private_key)
        {
            this.name = name;
            this.val = val;
            this.public_key = public_key;
            this.private_key = private_key;
        }

        public string Name
        {
            get { return name; }
            set { name = value;}
        }

        public string Val
        {
            get { return val; }
            set { val = value;}
        }

        public string Public_key
        {
            get { return public_key; }
            set { public_key = value; }
        }

        public string Private_key
        {
            get { return private_key; }
            set { private_key = val; }
        }

        public string toString()
        {
            return Name + " " + Val + " " + Public_key + " " + Private_key;
        }

        public Boolean validate(string public_key, string private_key)
        {
            if(public_key == Public_key && private_key == Private_key){
                return true;
            } else{
                return false;
            }
        }
    }
}

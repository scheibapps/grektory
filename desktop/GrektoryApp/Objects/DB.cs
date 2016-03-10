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
        private string Name;
        private string Val;
        private string Public_key;
        private string Private_key;

        public DB(string name, string val, string public_key, string private_key)
        {
            this.Name = name;
            this.Val = val;
            this.Public_key = public_key;
            this.Private_key = private_key;
        }

        public string name
        {
            get { return Name; }
            set { Name = value;}
        }

        public string val
        {
            get { return Val; }
            set { Val = value;}
        }

        public string public_key
        {
            get { return Public_key; }
            set { Public_key = value; }
        }

        public string private_key
        {
            get { return Private_key; }
            set { Private_key = value; }
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

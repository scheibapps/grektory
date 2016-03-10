using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrektoryApp.Objects
{
    class Member
    {
        private string Name;
        private string Phone;
        private string Email;

        public Member(string name, string phone, string email)
        {
            this.Name = name;
            this.Phone = phone;
            this.Email = email;
        }

        public string name
        {
            get { return Name; }
            set { Name = value; }
        }

        public string phone
        {
            get { return Phone; }
            set { Phone = value; }
        }

        public string email
        {
            get { return Email; }
            set { Email = value; }
        }
    }
}

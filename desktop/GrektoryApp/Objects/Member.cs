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
        private Boolean Alumni;

        public Member(string name, string phone, string email, Boolean alumni)
        {
            this.Name = name;
            this.Phone = phone;
            this.Email = email;
            this.Alumni = alumni;
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

        public Boolean alumni
        {
            get { return Alumni; }
            set { Alumni = value; }
        }
    }
}

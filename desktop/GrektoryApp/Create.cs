using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Windows.Forms;
using GrektoryApp.Objects;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.IO;

namespace GrektoryApp
{
    public partial class Create : Form
    {
        private DB db;
        private Main main;
        public Create(Main main)
        {
            this.main = main;
            InitializeComponent();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            Regex r = new Regex("^[a-zA-Z0-9 ]*$");
            if (txtDBName.Text.Length > 0 && r.IsMatch(txtDBName.Text))
            {
                string[] keys = generateKey(new Random());
                db = new DB(txtDBName.Text, keys[0], keys[1], keys[2]);
                txtPublic.Text = keys[1];
                txtPrivate.Text = keys[2];
                Console.WriteLine(db.private_key);
                txtDBName.Enabled = false;
                btnGenerate.Enabled = false;
                btnCreate.Enabled = true;
            }
            else
            {
                MessageBox.Show("Database name must contain only contain alphanumberic characters.");
            }
        }

        public string[] generateKey(Random rand)
        {
            string[] key_values = new string[3];
            for (int i = 0; i < 32; i++)
            {
                int type = rand.Next(10,100);
                if (type % 3 == 0)
                {
                    key_values[0] += Convert.ToChar(rand.Next(1000, 10000) % 10 + 48);
                }
                else if (type%2 == 0)
                {
                    key_values[0] += Convert.ToChar(rand.Next(1000, 10000) % 26 + 65);
                }
                else 
                {
                    key_values[0] += Convert.ToChar(rand.Next(1000, 10000) % 26 + 97);
                }
                
            }
            for (int i = 0; i < 8; i++)
            {
                key_values[1] += Convert.ToChar(rand.Next(1000, 10000) % 74 + 48);
                key_values[2] += Convert.ToChar(rand.Next(1000, 10000) % 74 + 48);
            }
            return key_values;
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            updateDBS();
            createClass();
            main.txtPublic.Text = db.public_key;
            main.txtPrivate.Text = db.private_key;
            this.Close();
        }

        private void updateDBS()
        {
            string stream = JsonConvert.SerializeObject(db);
            Console.Write(stream);
            byte[] bytes = Encoding.UTF8.GetBytes(stream);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://grektory.herokuapp.com/parse/classes/DBS");
            request.Method = "POST";
            request.Headers["X-Parse-Application-Id"] = "hehueu8y283yu3hlj14k3h4j1";
            request.ContentType = "application/json";
            request.ContentLength = bytes.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(bytes, 0, bytes.Length);
            dataStream.Close();
            request.Abort();
        }

        private void createClass()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://grektory.herokuapp.com/parse/classes/" + db.val);
            request.Method = "POST";
            request.Headers["X-Parse-Application-Id"] = "hehueu8y283yu3hlj14k3h4j1";
            request.GetResponse();
            request.Abort();
        }
    }
}

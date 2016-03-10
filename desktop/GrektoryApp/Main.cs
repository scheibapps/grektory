using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading.Tasks;
using System.Net.Http;
using System.Windows.Forms;
using System.Net;
using Newtonsoft.Json.Linq;
using GrektoryApp.Objects;
using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace GrektoryApp
{
    public partial class Main : Form
    {
        private List<DB> databases = new List<DB>();
        private DB saveDb;

        public Main()
        {
            InitializeComponent();
            txtPublic.Text = "0i394j56";
            txtPrivate.Text = "jn45j6k5";
            grid.Hide();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            progressBar.PerformStep();
            var results = requestJArray("DBS");
            foreach (JObject result in results.Children<JObject>())
            {
                foreach (JProperty data in result.Properties())
                {
                    foreach (JObject dbs in JArray.Parse(data.Value.ToString()).Children<JObject>())
                    {
                        dynamic dB = JObject.Parse(dbs.ToString());
                        string name = dB.name;
                        string val = dB.val;
                        string public_key = dB.public_key;
                        string private_key = dB.private_key;
                        DB db = new DB(name, val, public_key, private_key);
                        databases.Add(db);
                        progressBar.PerformStep();
                    }
                }
            }
            Boolean validate = false;
            foreach (DB database in databases)
            {
                if (database.validate(txtPublic.Text, txtPrivate.Text)){
                    validate = true;
                    progressBar.PerformStep();
                    saveDb = database;
                    loadDatabase(database);
                }
            }
            if (!validate)
            {
                MessageBox.Show("No directory with public key: "+txtPublic.Text+" and private key: "+txtPrivate.Text+" exists.");
                progressBar.Value = 0;
            }
        }

        private void btnSaveAll_Click(object sender, EventArgs e)
        {
            saveDatabase();
        }

        private void loadDatabase(DB database)
        {
            var results = requestJArray(database.Val);
            foreach (JObject result in results.Children<JObject>())
            {
                foreach (JProperty data in result.Properties())
                {
                    foreach (JObject members in JArray.Parse(data.Value.ToString()).Children<JObject>())
                    {
                        dynamic member = JObject.Parse(members.ToString());
                        string id = member.objectId;
                        string name = member.name;
                        string phone = member.phone;
                        string email = member.email;
                        int index = grid.Rows.Add();
                        grid.Rows[index].Cells[0].Value = id;
                        grid.Rows[index].Cells[1].Value = name;
                        grid.Rows[index].Cells[2].Value = email;
                        grid.Rows[index].Cells[3].Value = phone;
                        grid.Rows[index].Cells[4].Value = false;
                        progressBar.PerformStep();
                    }
                }
            }
            grid.Refresh();
            btnSaveAll.Enabled = true;
            btnSave.Enabled = true;
            btnDelete.Enabled = true;
            btnLoad.Enabled = false;
            grid.Show();
            progressBar.Value = 0;
        }

        private void saveDatabase()
        {
            for(int i = 0; i < grid.Rows.Count-1; i++)
            {
                if(progressBar.Value == 100)
                {
                    progressBar.Value = 0;
                } else
                {
                    progressBar.PerformStep();
                }
                saveRecord(grid.Rows[i]);
                
            }
            progressBar.Value = 0;
            
        }

        private void saveRecord(DataGridViewRow row)
        {
            Member member = new Member(row.Cells[1].Value as string, row.Cells[3].Value as string, row.Cells[2].Value as string);
            if (row.Cells[0].Value != null)
            {
                string stream = JsonConvert.SerializeObject(member);
                Console.Write(stream);
                byte[] bytes = Encoding.UTF8.GetBytes(stream);
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://grektory.herokuapp.com/parse/classes/" + saveDb.Val + "/" + row.Cells[0].Value as string);
                request.Method = "PUT";
                request.Headers["X-Parse-Application-Id"] = "hehueu8y283yu3hlj14k3h4j1";
                request.ContentType = "application/json";
                request.ContentLength = bytes.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(bytes, 0, bytes.Length);
                dataStream.Close();
                request.GetResponse();
                request.Abort();
            }
            else
            {
                string stream = JsonConvert.SerializeObject(member);
                byte[] bytes = Encoding.UTF8.GetBytes(stream);
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://grektory.herokuapp.com/parse/classes/" + saveDb.Val);
                request.Method = "POST";
                request.Headers["X-Parse-Application-Id"] = "hehueu8y283yu3hlj14k3h4j1";
                request.ContentType = "application/json";
                request.ContentLength = bytes.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(bytes, 0, bytes.Length);
                dataStream.Close();
                request.GetResponse();
                request.Abort();
            }
        }

        private void deleteRecord(DataGridViewRow row)
        {
            if(row.Cells[0].Value != null)
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://grektory.herokuapp.com/parse/classes/" + saveDb.Val + "/" + row.Cells[0].Value as string);
                request.Method = "DELETE";
                request.Headers["X-Parse-Application-Id"] = "hehueu8y283yu3hlj14k3h4j1";
                request.ContentType = "application/json";
                request.ContentLength = bytes.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(bytes, 0, bytes.Length);
                dataStream.Close();
                request.GetResponse();
                request.Abort();
            }
        }

        private JArray requestJArray(string key)
        {
            progressBar.PerformStep();
            WebRequest request = WebRequest.Create("https://grektory.herokuapp.com/parse/classes/" + key+ "?limit=1000&order=name");
            request.Method = "GET";
            request.Headers["X-Parse-Application-Id"] = "hehueu8y283yu3hlj14k3h4j1";
            request.ContentType = "application/json";
            WebResponse wr = request.GetResponse();
            Stream receiveStream = wr.GetResponseStream();
            StreamReader reader = new StreamReader(receiveStream, Encoding.UTF8);
            string content = reader.ReadToEnd();
            var json = "[" + content + "]"; // change this to array
            var results = JArray.Parse(json); // parse as array
            request.Abort();
            return results;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection rowCollection = grid.SelectedRows;
            for(int i = 0; i < rowCollection.Count; i++)
            {
                saveRecord(rowCollection[i]);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

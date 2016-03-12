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
            grid.Hide();
        }

        /**BUTTON CLICKS*/
        private void btnLoad_Click(object sender, EventArgs e)
        {
            grid.Rows.Clear();
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
                    lblTitle.Text = database.name;
                    progressBar.PerformStep();
                    saveDb = database;
                    loadDatabase(database);
                    break;
                }
            }
            if (!validate)
            {
                MessageBox.Show("No directory with public key: "+txtPublic.Text+" and private key: "+txtPrivate.Text+" exists.");
                btnLoad.Enabled = true;
                progressBar.Value = 0;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection rowCollection = grid.SelectedRows;
            for (int i = 0; i < rowCollection.Count; i++)
            {
                saveRecord(rowCollection[i]);
            }
        }

        private void btnSaveAll_Click(object sender, EventArgs e)
        {
            saveDatabase();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection rowCollection = grid.SelectedRows;
            for (int i = 0; i < rowCollection.Count; i++)
            {
                deleteRecord(rowCollection[i]);
            }
        }

        /**MENU CLICKS*/
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    
        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Create create = new Create(this);
            create.ShowDialog();
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = ".csv";
            dialog.Filter = ".csv|*csv";
            dialog.Title = "Save directory to CSV file";
            dialog.ShowDialog();
            if (dialog.FileName != "")
            {
                StreamWriter writer = new StreamWriter(dialog.OpenFile());
                writer.WriteLine(txtPublic.Text);
                writer.WriteLine(txtPrivate.Text);
                for (int i = 0; i < grid.Rows.Count - 1; i++)
                {
                    writer.WriteLine(grid.Rows[i].Cells[0].Value + "," + grid.Rows[i].Cells[1].Value + "," + grid.Rows[i].Cells[2].Value + "," + grid.Rows[i].Cells[3].Value + "," + grid.Rows[i].Cells[4].Value);
                }
                writer.Close();
            }
        }

        private void restoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "(*.csv)|*csv";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Stream stream = dialog.OpenFile();
                StreamReader streamReader = new StreamReader(stream);
                string line;
                txtPublic.Text = streamReader.ReadLine();
                txtPrivate.Text = streamReader.ReadLine();
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
                    if (database.validate(txtPublic.Text, txtPrivate.Text))
                    {
                        validate = true;
                        lblTitle.Text = database.name;
                        saveDb = database;
                        break;
                    }
                }
                if (!validate)
                {
                    MessageBox.Show("This directory has been removed and is not available for restore.");
                    btnLoad.Enabled = true;
                    progressBar.Value = 0;
                    return;
                }
                while ((line = streamReader.ReadLine()) != null)
                {
                    string[] newRow = line.Split(',');
                    int index = grid.Rows.Add();
                    grid.Rows[index].Cells[0].Value = newRow[0];
                    grid.Rows[index].Cells[1].Value = newRow[1];
                    grid.Rows[index].Cells[2].Value = newRow[2];
                    grid.Rows[index].Cells[3].Value = newRow[3];
                    grid.Rows[index].Cells[4].Value = Convert.ToBoolean(newRow[4]);
                }
                btnSaveAll.Enabled = true;
                btnSave.Enabled = true;
                btnDelete.Enabled = true;
                importToolStripMenuItem.Enabled = true;
                exportToolStripMenuItem.Enabled = true;
                createToolStripMenuItem.Enabled = false;
                restoreToolStripMenuItem.Enabled = false;
                txtPublic.Enabled = false;
                txtPrivate.Enabled = false;
                btnLoad.Enabled = false;
                grid.Show();
                progressBar.Value = 0;
            }
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "(*.csv)|*csv";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Stream stream = dialog.OpenFile();
                StreamReader streamReader = new StreamReader(stream);
                List<string> lines = new List<string>();
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    lines.Add(line);
                }
                Import import = new Import(this, lines);
                import.ShowDialog();
            }
        }

        /**HELPER FUNCTION*/
        private void loadDatabase(DB database)
        {
            var results = requestJArray(database.val);
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
                        bool alumni;
                        try
                        {
                            alumni = Convert.ToBoolean(member.alumni);
                        } catch (Exception e)
                        {
                            alumni = false;
                        }
                        int index = grid.Rows.Add();
                        grid.Rows[index].Cells[0].Value = id;
                        grid.Rows[index].Cells[1].Value = name;
                        grid.Rows[index].Cells[2].Value = email;
                        grid.Rows[index].Cells[3].Value = phone;
                        grid.Rows[index].Cells[4].Value = alumni;
                        progressBar.PerformStep();
                    }
                }
            }
            grid.Refresh();
            btnSaveAll.Enabled = true;
            btnSave.Enabled = true;
            btnDelete.Enabled = true;
            importToolStripMenuItem.Enabled = true;
            exportToolStripMenuItem.Enabled = true;
            createToolStripMenuItem.Enabled = false;
            restoreToolStripMenuItem.Enabled = false;
            txtPublic.Enabled = false;
            txtPrivate.Enabled = false;
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
            Member member = new Member(row.Cells[1].Value as string, row.Cells[3].Value as string, row.Cells[2].Value as string, Convert.ToBoolean(row.Cells[4].Value));
            if (row.Cells[0].Value != null)
            {
                try {
                    string stream = JsonConvert.SerializeObject(member);
                    Console.Write(stream);
                    byte[] bytes = Encoding.UTF8.GetBytes(stream);
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://grektory.herokuapp.com/parse/classes/" + saveDb.val + "/" + row.Cells[0].Value as string);
                    request.Method = "PUT";
                    request.Headers["X-Parse-Application-Id"] = "hehueu8y283yu3hlj14k3h4j1";
                    request.ContentType = "application/json";
                    request.ContentLength = bytes.Length;
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(bytes, 0, bytes.Length);
                    dataStream.Close();
                    request.GetResponse();
                    request.Abort();
                } catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            else
            {
                string stream = JsonConvert.SerializeObject(member);
                byte[] bytes = Encoding.UTF8.GetBytes(stream);
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://grektory.herokuapp.com/parse/classes/" + saveDb.val);
                request.Method = "POST";
                request.Headers["X-Parse-Application-Id"] = "hehueu8y283yu3hlj14k3h4j1";
                request.ContentType = "application/json";
                request.ContentLength = bytes.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(bytes, 0, bytes.Length);
                dataStream.Close();
                WebResponse response = request.GetResponse();
                string response_location = response.Headers["Location"].ToString();
                char[] response_char = response_location.ToCharArray();
                string objectId = "";
                for(int i = 0; i < 10; i++)
                {
                    objectId += response_char[response_char.Length - (10 - i)];
                }
                row.Cells[0].Value = objectId;
                request.Abort();
            }
        }

        private void deleteRecord(DataGridViewRow row)
        {
            if(row.Cells[0].Value != null)
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://grektory.herokuapp.com/parse/classes/" + saveDb.val + "/" + row.Cells[0].Value as string);
                grid.Rows.Remove(row);
                request.Method = "DELETE";
                request.Headers["X-Parse-Application-Id"] = "hehueu8y283yu3hlj14k3h4j1";
                request.ContentType = "application/json";
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
    }
}

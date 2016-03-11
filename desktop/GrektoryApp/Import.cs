using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GrektoryApp.Objects;

namespace GrektoryApp
{
    public partial class Import : Form
    {
        Main main;
        List<string> lines;
        DataGridViewRow headerRow;
        public Import(Main main, List<string> lines)
        {
            this.main = main;
            this.lines = lines;
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Import_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                string[] line = lines[i].Split(',');
                int index = grid.Rows.Add();
                grid.Rows[index].Cells[0].Value = (line.Length > 0) ? line[0] : null;
                grid.Rows[index].Cells[1].Value = (line.Length > 1) ? line[1] : null;
                grid.Rows[index].Cells[2].Value = (line.Length > 2) ? line[2] : null;
                if(line.Length > 3 && line[3].Equals("TRUE")){
                    grid.Rows[index].Cells[3].Value = true;
                }
            }
            grid.Refresh();
        }

        private void chkHeaders_CheckedChanged(object sender, EventArgs e)
        {
            if (chkHeaders.Checked)
            {
                headerRow = grid.Rows[0];
                grid.Rows.RemoveAt(0);
            }
            else
            {
                grid.Rows.Insert(0, headerRow);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            for(int i = 0; i < grid.Rows.Count-1; i++)
            {
                int index = main.grid.Rows.Add();
                main.grid.Rows[index].Cells[1].Value = grid.Rows[i].Cells[0].Value;
                main.grid.Rows[index].Cells[2].Value = grid.Rows[i].Cells[1].Value;
                main.grid.Rows[index].Cells[3].Value = grid.Rows[i].Cells[2].Value;
                main.grid.Rows[index].Cells[4].Value = grid.Rows[i].Cells[3].Value;
            }
            this.Close();
        }
    }
}

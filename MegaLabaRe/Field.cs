using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MegaLabaRe
{
    public partial class Field : Form
    {
        int[,] arr = new int[10,10];
        public Field(ref int[,] arr)
        {
            InitializeComponent();
            arr = this.arr;
            int c = 'A';
            dataGridView1.Height = 325;
            dataGridView1.Width = 335;
            for (int i = 0; i < 10; i++)
            {
                dataGridView1.Columns[i].Width = 30;
                dataGridView1.Columns[i].HeaderText = ((char)c++).ToString();
                dataGridView1.Rows.Add();
                dataGridView1.Rows[i].Height = 30;
            }
            dataGridView1.RowHeadersWidth = 30;
            this.dataGridView1.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            dataGridView1.AutoResizeRowHeadersWidth(
        DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
        }

        private void unmarkButton_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
            {
                int x = cell.ColumnIndex;
                int y = cell.RowIndex;
                dataGridView1.ClearSelection();
                dataGridView1.Rows[y].Cells[x].Style.BackColor = Color.Red;
                arr[y, x] = 0;
            }
        }

        private void markbutton_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
            {
                int x = cell.ColumnIndex;
                int y = cell.RowIndex;
                dataGridView1.ClearSelection();
                dataGridView1.Rows[y].Cells[x].Style.BackColor = Color.Red;
                arr[y, x] = 1;
            }
        }

        private void setButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

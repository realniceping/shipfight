using System.Windows.Forms;

namespace MegaLabaRe
{
    public partial class Form1 : Form
    {
        private static Guid TypeGuid => Guid.Parse("65D44511-BFD4-45EF-B5A5-A044B358C364");
        public static Type objectType = Type.GetTypeFromCLSID(TypeGuid)
                ?? throw new ArgumentException($"Сборка с идентификатором {TypeGuid} не обнаружена");
        dynamic @object = Activator.CreateInstance(objectType);
        const int client = 1;
        string[] cord;
        public Form1()
        {
            InitializeComponent();
            SetGrid(dataGridView1);
            SetGrid(dataGridView2);
        }

        private void SetGrid(DataGridView dataGridView1)
        {
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

        private void dataGridView2_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {

        }

        private void DolbitsaVServerTurn()
        {
            string responce = @object.TurnRequest(client);
            button1.Enabled = false;
            SetField.Enabled = false;
            while (responce == "-1 -1 0")
            {
                responce = @object.TurnRequest(client);
                Thread.Sleep(100);
            }
            button1.Enabled = true;
            SetField.Enabled = true;
            cord = responce.Split(' ');
            dataGridView2.Rows[Convert.ToInt32(cord[1])].Cells[Convert.ToInt32(cord[0])].Style.BackColor = Color.Orange;
            //MessageBox.Show("YOUR TURN");
        }
        private void DolbitsaVServerReady()
        {
            bool status = @object.Ready(client);
            button1.Enabled = false;
            SetField.Enabled = false;
            while (status == false)
            {
                status = @object.Ready(client);
                Thread.Sleep(100);
            }
            button1.Enabled = true;
            SetField.Enabled = true;
            MessageBox.Show("YOUR TURN");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int x = dataGridView1.CurrentCell.ColumnIndex;
            int y = dataGridView1.CurrentCell.RowIndex;
            dataGridView1.ClearSelection();
            int result = @object.PointShootEventHandler(y, x, client);
            if (result == 1)
            {
                dataGridView1.Rows[y].Cells[x].Style.BackColor = Color.Red;
            }
            if (result == -1)
            {
                dataGridView1.Rows[y].Cells[x].Style.BackColor = Color.Black;
            }
            if (result == 2)
            {
                dataGridView1.Rows[y].Cells[x].Style.BackColor = Color.Red;
                MessageBox.Show("Win");
            }
            
            new Thread(DolbitsaVServerTurn) { IsBackground = true}.Start();
        }

        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            dataGridView2.ClearSelection();
        }

        private void resetDataGrid(DataGridView dataGridView1)
        {
            dataGridView1.SelectAll();
            foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
            {
                cell.Style.BackColor = Color.White;
            }
            dataGridView1.ClearSelection();
        }

        private void SetField_Click(object sender, EventArgs e)
        {
            resetDataGrid(dataGridView1);
            resetDataGrid(dataGridView2);
            int[,] arr = new int[10,10];
            (new Field(ref arr)).ShowDialog();
            @object.setField(arr, client);
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (arr[i, j] != 0)
                    {
                        dataGridView2.Rows[i].Cells[j].Style.BackColor = Color.Black;
                    }
                }
            }
            
            new Thread(DolbitsaVServerReady) { IsBackground=true}.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
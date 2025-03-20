using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TableMonitor.Forms
{
    public partial class AddPrinter : Form
    {
        public AddPrinter()
        {
            InitializeComponent();
        }

        /// <summary>
        /// This event function will treggerd when printer window load.
        /// Purpose of this function is to load DATA in grid and hide printerId.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddPrinter_Load(object sender, EventArgs e)
        {
            loadDataInGrid();
            printerId.Visible = false;
        }

        /// <summary>
        /// This Button event is use to close window of Printer Configuration.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// This Button event is use to Add printer data in the data base.
        /// First its check all field values are or not null || empty then insert query will treggerd and store values in database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Printer_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(TxtPrinterName.Text) && !string.IsNullOrEmpty(TxtPrintStatus.Text) && !string.IsNullOrEmpty(TxtTransacionType.Text))
                {
                    var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString);
                    connection.Open();
                    var printer_name = TxtPrinterName.Text;
                    var printer_status = Convert.ToBoolean(Convert.ToInt32(TxtPrintStatus.Text));
                    var transaction_type = TxtTransacionType.Text;
                    var printer_floor = TextPrinterFloor.Text;
                    var pos_terminal = TextPosTerminal.Text;
                    var cmd = new SqlCommand("insert into ItemWisePrinterConfiguration (PrinterName ,PrinterStatus, PoolId, Floor, Terminal) values('" + printer_name + "', '" + printer_status + "','" + transaction_type + "','" + printer_floor + "','" + pos_terminal + "')", connection);
                    var result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MessageBox.Show("Data Inserted Successfully.");
                        loadDataInGrid();
                    }
                    else
                    {
                        MessageBox.Show("Data not Inserted.");
                    }
                    connection.Close();
                }
                else
                {
                    MessageBox.Show("Please Fill all Fields and then press Add Printer button", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        ///This function is use to populate printer data in gridview and display..
        /// </summary>
        private void loadDataInGrid()
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString);
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM ItemWisePrinterConfiguration", connection);
            DataSet ds = new DataSet();
            da.Fill(ds, "ItemWisePrinterConfiguration");
            dataGridView1.DataSource = ds.Tables["ItemWisePrinterConfiguration"].DefaultView;
        }
        /// <summary>
        /// This Button event is use to Detete printer data in the data base.
        /// First you have to select gridview row which data you whant to delete when all textbox values are populate then click delete button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Delete_Printer_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(printerId.Text))
                {
                    var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString);
                    connection.Open();
                    var cmd = new SqlCommand("Delete ItemWisePrinterConfiguration where Id=@PrinterId", connection);
                    cmd.Parameters.Add("@PrinterId", SqlDbType.Int).Value = printerId.Text;
                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MessageBox.Show("Printer Configuration has been deleted successfully.");
                        loadDataInGrid();
                    }
                    else
                    {
                        MessageBox.Show("Unable to delete.");
                    }
                    connection.Close();
                }
                else
                {
                    MessageBox.Show("Please select row in gridview which data you want to Delete", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// This Button event is use to update printer data in the database.
        /// First you have to select gridview row which data you whant to update when all textbox values are populate then change your data and click update button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrinterUpdatebtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(TxtPrinterName.Text) && !string.IsNullOrEmpty(TxtPrintStatus.Text) && !string.IsNullOrEmpty(TxtTransacionType.Text))
                {
                    SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString);
                    connection.Open();

                    SqlCommand cmd = new SqlCommand("UPDATE ItemWisePrinterConfiguration SET  PrinterName = @PrinterName, PrinterStatus = @PrinterStatus, PoolId = @PoolId, Floor = @Floor, Terminal = @Terminal where Id=@PrinterId", connection);
                    cmd.Parameters.Add("@PrinterId", SqlDbType.Int).Value = Convert.ToInt32(printerId.Text);
                    cmd.Parameters.Add("@PrinterName", SqlDbType.VarChar).Value = TxtPrinterName.Text;
                    cmd.Parameters.Add("@PrinterStatus", SqlDbType.VarChar).Value = TxtPrintStatus.Text;
                    cmd.Parameters.Add("@PoolId", SqlDbType.VarChar).Value = TxtTransacionType.Text;
                    cmd.Parameters.Add("@Floor", SqlDbType.VarChar).Value = TextPrinterFloor.Text;
                    cmd.Parameters.Add("@Terminal", SqlDbType.VarChar).Value = TextPosTerminal.Text;
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Printer Configuration has been updated successfully.");
                    loadDataInGrid();
                    connection.Close();
                }
                else
                {
                    MessageBox.Show("Please select row in gridview which data you want to update", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// This gridview event function is use to display data to user from database 
        /// and enable user to select data which user want to modify.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = this.dataGridView1.Rows[e.RowIndex];
                printerId.Text = row.Cells["Id"].Value.ToString();
                TxtPrinterName.Text = row.Cells["PrinterName"].Value.ToString();
                var status = row.Cells["PrinterStatus"].Value.ToString();
                if (status == "True")
                {
                    TxtPrintStatus.Text = "1";
                }
                else
                {
                    TxtPrintStatus.Text = "0";
                }
                TxtTransacionType.Text = row.Cells["PoolId"].Value.ToString();
                TextPrinterFloor.Text = row.Cells["Floor"].Value.ToString();
                TextPosTerminal.Text = row.Cells["Terminal"].Value.ToString();
            }
        }

    }
}
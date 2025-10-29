using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OOADCafeShopManagement.Models;


namespace OOADCafeShopManagement
{
    public partial class frmDashboard : Form
    {

        private Dashboard model;
        //Config for moving form
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();


        public frmDashboard()
        {
            InitializeComponent();

            //Drag From with panel
            MakeDraggable(pnlDashboard);
            MakeDraggable(pnlMenu);
            MakeDraggable(pnlSepBar);

            //Default - Last 7 days
            dtpStartDate.Value = DateTime.Today.AddDays(-7);
            dtpEndDate.Value = DateTime.Today;
            btnLast7Days.Select();

            model = new Dashboard();
            LoadData();



        }

        //Private Methods
        private void LoadData()
        {
            var refreshData = model.LoadData(dtpStartDate.Value, dtpEndDate.Value);
            if (refreshData == true)
            {
                //Update Labels
                lblNumberOrder.Text = model.NumberOrders.ToString();
                lblTotalRevenue.Text = "$" + model.TotalRevenue.ToString("N2");
                lblTotalProfit.Text = "$" + model.TotalProfit.ToString("N2");

                lblNumberCustomer.Text = model.NumberCustomers.ToString();
                lblNumberSupplier.Text = model.NumberSuppliers.ToString();
                lblNumberProduct.Text = model.NumberProducts.ToString();

                chartGrossRevenue.DataSource = model.GrossRevenueList;
                chartGrossRevenue.Series[0].XValueMember = "Date";
                chartGrossRevenue.Series[0].YValueMembers = "TotalAmount";
                chartGrossRevenue.DataBind();

                chartTopProducts.DataSource = model.TopProductsList;
                chartTopProducts.Series[0].XValueMember = "Key";
                chartTopProducts.Series[0].YValueMembers = "Value";
                chartTopProducts.DataBind();

                dgvUnderstock.DataSource = model.UnderstockList;
                Console.WriteLine("Data loaded successfully.");

            }
            else
            {
                Console.WriteLine("Failed to load data.");
            }
        }

        private void MakeDraggable(Control control)
        {
            control.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(Handle, 0xA1, 0x2, 0);
                }
            };
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        private void btnBack_Click(object sender, EventArgs e)
        {
            frmMainDashboard mainDashboard = new frmMainDashboard();
            mainDashboard.Show();
            this.Hide();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void chartTopProducts_Click(object sender, EventArgs e)
        {

        }
    }
}

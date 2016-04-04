using System;
using System.Windows.Forms;
using Trek.ProductMonitor.Events;
using Trek.ProductMonitor.Services;

namespace Trek.ProductMonitor
{
    public partial class Main : Form
    {
        private readonly IProductUpdateService _productUpdateService;

        public Main(IProductUpdateService productUpdateService)
        {  
            InitializeComponent();
            
            _productUpdateService = productUpdateService;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadVendors();
            WatchForUpdates();
        }

        private void WatchForUpdates()
        {
            _productUpdateService.ProductsUpdated += OnProductUpdated;
            _productUpdateService.WatchForUpdates();
        }

        public void OnProductUpdated(object sender, ProductUpdateEventArgs args)
        {
            if (args == null) { return; }

            this.Invoke((MethodInvoker)delegate
            {
                productUpdatesDataGrid.Rows.Insert(0, args.Item.Vendor, args.Item.Product, args.Item.Description, args.Item.Price);
                TrimExcessRows();
            });

        }

        public void LoadVendors()
        {
            vendorListBox.DataSource =  _productUpdateService.GetVendors().Result;
            vendorListBox.DisplayMember = "Label";
            vendorListBox.ValueMember = "Code";
            vendorListBox.SelectedIndex = -1;
        }

        private void TrimExcessRows()
        {
            if (productUpdatesDataGrid.Rows.Count > 50)
            {
                for (var i = productUpdatesDataGrid.Rows.Count - 1; i > 50; i--)
                {
                    productUpdatesDataGrid.Rows.RemoveAt(i);
                }
            }
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}

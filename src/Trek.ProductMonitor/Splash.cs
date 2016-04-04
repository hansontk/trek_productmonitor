using System;
using System.Threading;
using System.Windows.Forms;
using Trek.ProductMonitor.Services;

namespace Trek.ProductMonitor
{
    public partial class Splash : Form
    {
        private readonly IProductUpdateService _productUpdateService;
        private readonly Main _mainForm;

        public Splash(Main mainForm, IProductUpdateService productUpdateService)
        {
            _productUpdateService = productUpdateService;
            _mainForm = mainForm;

            InitializeComponent();
        }

        private void Splash_Load(object sender, EventArgs e)
        {
            LoadVendors();
        }

        public void LoadVendors()
        {
            _productUpdateService.GetVendors().ContinueWith((o) =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(2)); // Ensure we're displayed for at least a little while
                this.Invoke((MethodInvoker) delegate
                {
                    _mainForm.Show();
                    this.Hide();
                });
            });
        }
    }
}

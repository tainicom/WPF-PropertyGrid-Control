using System.Windows.Forms;

namespace WindowsFormsIntegration
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();

      var host = new System.Windows.Forms.Integration.ElementHost { Dock = DockStyle.Fill };

      var wpg = new tainicom.WpfPropertyGrid.PropertyGrid
      {
        Layout = new tainicom.WpfPropertyGrid.Design.CategorizedLayout()
      };
      host.Child = wpg;

      wpg.SelectedObject = new BusinessObject();
      
      this.Controls.Add(host);
    }
  }
}

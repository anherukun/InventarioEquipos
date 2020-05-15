using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SharedViews.UControls
{
    /// <summary>
    /// Lógica de interacción para CatalogControl.xaml
    /// </summary>
    public partial class CatalogControl : UserControl
    {
        public CatalogControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public string Title { get { return title.Text; } set { title.Text = value; } }
        public string Subtitle { get { return subtitle.Text; } set { subtitle.Text = value; } }
    }
}

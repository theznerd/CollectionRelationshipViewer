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
using MahApps.Metro.Controls;
using MahApps.Metro;
using Caliburn.Micro;

namespace CollectionRelationshipViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class AppView : MetroWindow
    {
        public AppView()
        {
            InitializeComponent();

            //// Load the three dynamic views - it's terrible to do this
            //// in code behind, but honestly... it's a pretty simple 
            //// application... so maybe I can be a bit lazy.

            // The about view does not need to be called here since we'll
            // be doing that properly in the view itself.

            // Settings View
            SettingsView sv = new SettingsView();
            SettingsViewModel svm = new SettingsViewModel();
            ViewModelBinder.Bind(svm, sv, null);
            Settings.Tag = sv;

            // Devices View
            DevicesView dv = new DevicesView();
            DevicesViewModel dvm = new DevicesViewModel();
            ViewModelBinder.Bind(dvm, dv, null);
            Devices.Tag = dv;

            // Users View
            UsersView uv = new UsersView();
            UsersViewModel uvm = new UsersViewModel();
            ViewModelBinder.Bind(uvm, uv, null);
            Users.Tag = uv;
        }
    }
}
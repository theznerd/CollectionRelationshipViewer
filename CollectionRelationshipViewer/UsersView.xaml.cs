using Syncfusion.UI.Xaml.Diagram;
using Syncfusion.UI.Xaml.Diagram.Layout;
using System;
using System.Windows;
using System.Windows.Controls;

namespace CollectionRelationshipViewer
{
    /// <summary>
    /// Interaction logic for DevicesView.xaml
    /// </summary>
    public partial class UsersView : UserControl
    {
        public UsersView()
        {
            InitializeComponent();
            UFD.Tool = Tool.ZoomPan;
            UFD.Constraints = GraphConstraints.Default & ~GraphConstraints.ContextMenu;
        }

        private void RefreshView(object sender, EventArgs e)
        {
            UFD.Page.UpdateLayout();
            (UFD.LayoutManager.Layout as DirectedTreeLayout).UpdateLayout();
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            IGraphInfo graphinfo = UFD.Info as IGraphInfo;
            graphinfo.Commands.Zoom.Execute(new ZoomPositionParamenter()
            {
                ZoomFactor = 0.5,
                ZoomCommand = ZoomCommand.ZoomIn
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IGraphInfo graphinfo = UFD.Info as IGraphInfo;
            graphinfo.Commands.Reset.Execute(new ResetParameter() { Reset = Reset.Zoom });
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            IGraphInfo graphinfo = UFD.Info as IGraphInfo;
            graphinfo.Commands.Zoom.Execute(new ZoomPositionParamenter()
            {
                ZoomFactor = 0.5,
                ZoomCommand = ZoomCommand.ZoomOut
            });
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = (UsersViewModel)this.DataContext;
            vm.RefreshView += RefreshView;
        }
    }
}
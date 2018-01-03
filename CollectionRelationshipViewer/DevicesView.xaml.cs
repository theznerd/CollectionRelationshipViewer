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
    public partial class DevicesView : UserControl
    {
        //// Lots of code behind here that I'm not proud of
        //// but honestly it's about the only way you can
        //// really do some of this with the Syncfusion stuff
        public DevicesView()
        {
            InitializeComponent();
            SFD.Tool = Tool.ZoomPan; // sets the tool to zoompan (so that you can't select objects)
            SFD.Constraints = GraphConstraints.Default & ~GraphConstraints.ContextMenu; // removes the context menu on right click
        }

        // Refresh view button causes the layout to be "updated"
        private void RefreshView(object sender, EventArgs e)
        {
            SFD.Page.UpdateLayout();
            (SFD.LayoutManager.Layout as DirectedTreeLayout).UpdateLayout();
        }

        // Zoom In click zooms the view in
        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            IGraphInfo graphinfo = SFD.Info as IGraphInfo;
            graphinfo.Commands.Zoom.Execute(new ZoomPositionParamenter()
            {
                ZoomFactor = 0.5,
                ZoomCommand = ZoomCommand.ZoomIn
            });
        }

        // This resets the zoom level to 1
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IGraphInfo graphinfo = SFD.Info as IGraphInfo;
            graphinfo.Commands.Reset.Execute(new ResetParameter() { Reset = Reset.Zoom });
        }

        // This zooms the view out
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            IGraphInfo graphinfo = SFD.Info as IGraphInfo;
            graphinfo.Commands.Zoom.Execute(new ZoomPositionParamenter()
            {
                ZoomFactor = 0.5,
                ZoomCommand = ZoomCommand.ZoomOut
            });
        }

        // When the control is loaded, link refresh view to update the layout
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
                var vm = (DevicesViewModel)this.DataContext;
                vm.RefreshView += RefreshView;
        }
    }
}
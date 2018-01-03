using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace CollectionRelationshipViewer
{
    /// <summary>
    /// This is borrowed from punker76 (maintainer of MahApps Metro theme
    /// that is used in this app). It allows for custom icons to be set
    /// on the hamburger menu.
    /// https://github.com/punker76/code-samples/blob/master/MahAppsMetroHamburgerMenu/HamburgerMenuApp.V3/CustomHamburgerMenuIconItem.cs
    /// </summary>
    public class CustomHamburgerMenuIconItem : HamburgerMenuIconItem
    {
        public static readonly DependencyProperty ToolTipProperty
            = DependencyProperty.Register("ToolTip",
                typeof(object),
                typeof(CustomHamburgerMenuIconItem),
                new PropertyMetadata(null));

        public object ToolTip
        {
            get { return (object)GetValue(ToolTipProperty); }
            set { SetValue(ToolTipProperty, value); }
        }
    }
}
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Caliburn.Micro;
using Syncfusion.UI.Xaml.Diagram;
using Syncfusion.UI.Xaml.Diagram.Layout;
using System.Windows.Media;
using System.Windows;
using System.Collections.Generic;

namespace CollectionRelationshipViewer
{
    /// <summary>
    /// The Devices page view model. This contains all the
    /// logic necessary to connect the View to the data.
    /// </summary>
    public class DevicesViewModel : PropertyChangedBase
    {
        // an event handler for refreshing the view... might not be necessary anymore
        // but makes sure that the tree looks pretty.
        public event EventHandler RefreshView;
        protected virtual void OnRefresh(EventArgs e)
        {
            RefreshView?.Invoke(this, e);
        }
        
        // sets the properties for the tree layout
        private DirectedTreeLayout _dtl = new DirectedTreeLayout()
        {
            Type = LayoutType.Hierarchical,
            HorizontalSpacing = 5,
            VerticalSpacing = 60,
            Orientation = TreeOrientation.LeftToRight,
            Margin = new Thickness(25, 25, 25, 25)
        };
        // getter/setter for the tree layout
        public DirectedTreeLayout dtl
        {
            get { return _dtl; }
            set
            {
                _dtl = value;
                NotifyOfPropertyChange(() => dtl);
            }
        }

        // creates an empty layout manager
        private LayoutManager _lm = new LayoutManager();
        // getter/setter for the layout manager
        public LayoutManager lm
        {
            get { return _lm; }
            set
            {
                _lm = value;
                NotifyOfPropertyChange(() => lm);
            }
        }

        // creates an empty data source settings
        private DataSourceSettings _dss = new DataSourceSettings();
        // getter/setter for the data source settings
        public DataSourceSettings dss
        {
            get { return _dss; }
            set
            {
                _dss = value;
                NotifyOfPropertyChange(() => dss);
            }
        }

        // hides the loading visibility by default
        private string _loadingVisibility = "Hidden";
        // getter/setter for the loading window
        public string LoadingVisibility
        {
            get { return _loadingVisibility; }
            set
            {
                _loadingVisibility = value;
                NotifyOfPropertyChange(() => LoadingVisibility);
            }
        }

        // creates an empty diagram view model
        private DiagramViewModel _dvm = new DiagramViewModel();
        // getter/setter for the diagram view model
        public DiagramViewModel dvm
        {
            get { return _dvm; }
            set
            {
                _dvm = value;
                NotifyOfPropertyChange(() => dvm);
            }
        }

        // create an empty observable collection - we'll add data
        // to this later when the refresh button is pressed
        public ObservableCollection<Models.SCCMCollection> nodes = new ObservableCollection<Models.SCCMCollection>();

        // allows or disallows the refresh button to be pressed
        private bool _canRefreshCollections = true;
        public bool CanRefreshCollectionsBool
        {
            get { return _canRefreshCollections; }
            set
            {
                _canRefreshCollections = value;
                NotifyOfPropertyChange(() => CanRefreshCollectionsBool);
                NotifyOfPropertyChange(() => CanRefreshCollections);
            }
        }

        // details pane visibility
        private bool _detailsVisible = false;
        public bool DetailsVisible
        {
            get { return _detailsVisible; }
            set
            {
                _detailsVisible = value;
                NotifyOfPropertyChange(() => DetailsVisible);
            }
        }

        // sets the text for the details button
        public void ToggleDetails()
        {
            DetailsVisible = !DetailsVisible;
            if (DetailsVisible)
            {
                ToggleNote = "Hide Details";
            }
            else
            {
                ToggleNote = "Show Details";
            }
        }

        // the text for the details button
        private string _toggleNote = "Show Details";
        public string ToggleNote
        {
            get { return _toggleNote; }
            set
            {
                _toggleNote = value;
                NotifyOfPropertyChange(() => ToggleNote);
            }
        }

        // this is what happens when we click on an object in the tree
        // basically just sets some strings
        public void GetDetails(string Name, string CollectionID, string MemberCount, DateTime LastRefreshTime, List<string> IncludeCollections, List<string> ExcludeCollections, List<string> DirectMembership, Dictionary<string,string> QueryRules, string RefreshSchedule)
        {
            if (!DetailsVisible)
            {
                ToggleDetails();
            }
            CDCollectionName = Name;
            CDCollectionID = CollectionID;
            CDNumberOfMembers = MemberCount;
            _cdIncludeCollections = IncludeCollections;
            _cdExcludeCollections = ExcludeCollections;
            CDLastRefreshTime = LastRefreshTime;
            _cdDirectMemberships = DirectMembership;
            _cdQueryRules = QueryRules;
            CDRefreshSchedule = RefreshSchedule;
        }
        private string _cdCollectionName;
        public string CDCollectionName
        {
            get { return _cdCollectionName; }
            set
            {
                _cdCollectionName = value;
                NotifyOfPropertyChange(() => CDCollectionName);
            }
        }

        private string _cdIncludeCollectionsString;
        public string CDIncludeCollectionsString
        {
            get { return _cdIncludeCollectionsString; }
            set
            {
                _cdIncludeCollectionsString = value;
                NotifyOfPropertyChange(() => CDIncludeCollectionsString);
            }
        }

        private List<string> _cdIncludeCollections
        {
            set
            {
                CDIncludeCollectionsString = "";
                // yeah, yeah, I know...
                try
                {
                    if (value.Count > 0)
                    {
                        foreach (string ec in value)
                        {
                            CDIncludeCollectionsString += "\r\n - " + ec;
                        }
                    }
                    else
                    {
                        CDIncludeCollectionsString = "\r\n - None";
                    }
                }
                catch
                {
                    CDIncludeCollectionsString = "\r\n - None";

                }
            }
        }

        private string _cdExcludeCollectionsString;
        public string CDExcludeCollectionsString
        {
            get { return _cdExcludeCollectionsString; }
            set
            {
                _cdExcludeCollectionsString = value;
                NotifyOfPropertyChange(() => CDExcludeCollectionsString);
            }
        }

        private List<string> _cdExcludeCollections
        {
            set
            {
                CDExcludeCollectionsString = "";
                // yeah, yeah, I know...
                try
                {
                    if(value.Count > 0)
                    {
                        foreach (string ec in value)
                        {
                            CDExcludeCollectionsString += "\r\n - " + ec;
                        }
                    }
                    else
                    {
                        CDExcludeCollectionsString = "\r\n - None";
                    }

                }
                catch
                {
                    CDExcludeCollectionsString = "\r\n - None";
                    
                }
            }
        }

        // since these could be long, we've added a show/hide feature
        private string _cdDirectVisibility = "Show";
        public string CDDirectVisibility
        {
            get { return _cdDirectVisibility; }
            set
            {
                _cdDirectVisibility = value;
                NotifyOfPropertyChange(() => CDDirectVisibility);
            }
        }
        private string _cdDirectMembershipsVisible = "Collapsed";
        public string CDDirectMembershipsVisible
        {
            get { return _cdDirectMembershipsVisible; }
            set
            {
                if(_cdDirectMembershipsVisible == "Collapsed")
                {
                    _cdDirectMembershipsVisible = "Visible";
                    CDDirectVisibility = "Hide";
                }
                else
                {
                    _cdDirectMembershipsVisible = "Collapsed";
                    CDDirectVisibility = "Show";

                }
                NotifyOfPropertyChange(() => CDDirectMembershipsVisible);
            }
        }
        public void ToggleCDDirectMembership()
        {
            CDDirectMembershipsVisible = "";
        }

        private string _cdDirectMembershipsString;
        public string CDDirectMembershipsString
        {
            get { return _cdDirectMembershipsString; }
            set
            {
                _cdDirectMembershipsString = value;
                NotifyOfPropertyChange(() => CDDirectMembershipsString);
            }
        }

        private List<string> _cdDirectMemberships
        {
            set
            {
                CDDirectMembershipsString = "";
                try // yeah, yeah, I know...
                {
                    if (value.Count > 0)
                    {
                        foreach (string ec in value)
                        {
                            CDDirectMembershipsString += " - " + ec + "\r\n";
                        }
                    }
                    else
                    {
                        CDDirectMembershipsString = " - None";
                    }

                }
                catch
                {
                    CDDirectMembershipsString = " - None";

                }
            }
        }

        // since these could be long, we've added a show/hide feature
        private string _cdQueryVisibility = "Show";
        public string CDQueryVisibility
        {
            get { return _cdQueryVisibility; }
            set
            {
                _cdQueryVisibility = value;
                NotifyOfPropertyChange(() => CDQueryVisibility);
            }
        }
        private string _cdQueryRulesVisible = "Collapsed";
        public string CDQueryRulesVisible
        {
            get { return _cdQueryRulesVisible; }
            set
            {
                if (_cdQueryRulesVisible == "Collapsed")
                {
                    _cdQueryRulesVisible = "Visible";
                    CDQueryVisibility = "Hide";
                }
                else
                {
                    _cdQueryRulesVisible = "Collapsed";
                    CDQueryVisibility = "Show";
                }
                NotifyOfPropertyChange(() => CDQueryRulesVisible);
            }
        }

        public void ToggleCDQueryRules()
        {
            CDQueryRulesVisible = "";
        }

        private string _cdQueryRuleString;
        public string CDQueryRuleString
        {
            get { return _cdQueryRuleString; }
            set
            {
                _cdQueryRuleString = value;
                NotifyOfPropertyChange(() => CDQueryRuleString);
            }
        }

        private Dictionary<string,string> _cdQueryRules
        {
            set
            {
                CDQueryRuleString = "";
                try // yeah, yeah, I know...
                {
                    if (value.Count > 0)
                    {
                        foreach (KeyValuePair<string,string> qp in value)
                        {
                            CDQueryRuleString += "Name: " + qp.Key + "\r\nQuery: " + qp.Value + "\r\n";
                        }
                    }
                    else
                    {
                        CDQueryRuleString = "- None";
                    }

                }
                catch
                {
                    CDQueryRuleString = "- None";

                }
            }
        }

        private string _cdRefreshSchedule;
        public string CDRefreshSchedule
        {
            get { return _cdRefreshSchedule; }
            set
            {
                _cdRefreshSchedule = value;
                NotifyOfPropertyChange(() => CDRefreshSchedule);
            }
        }

        private DateTime _cdLastRefreshTime;
        public DateTime CDLastRefreshTime
        {
            get { return _cdLastRefreshTime; }
            set
            {
                _cdLastRefreshTime = value;
                NotifyOfPropertyChange(() => CDLastRefreshTime);
                NotifyOfPropertyChange(() => CDLastRefreshTimeString);
            }
        }

        public string CDLastRefreshTimeString
        {
            get { return CDLastRefreshTime.ToString(); }
        }

        private string _cdCollectionID;
        public string CDCollectionID
        {
            get { return _cdCollectionID; }
            set
            {
                _cdCollectionID = value;
                NotifyOfPropertyChange(() => CDCollectionID);
            }
        }

        private string _cdNumberOfMembers;
        public string CDNumberOfMembers
        {
            get { return _cdNumberOfMembers; }
            set
            {
                _cdNumberOfMembers = value;
                NotifyOfPropertyChange(() => CDNumberOfMembers);
            }
        }

        public bool CanRefreshCollections
        {
            get { return _canRefreshCollections; }
        }

        // by default we don't want to show anything (just a little bug correction)
        private string _collectionVis = "Collapsed";
        public string CollectionVis
        {
            get { return _collectionVis; }
            set
            {
                _collectionVis = value;
                NotifyOfPropertyChange(() => CollectionVis);
            }
        }

        // here's where we do the fun! 
        public async void RefreshCollections()
        {
            CanRefreshCollectionsBool = false; // make the refresh button inactive
            LoadingVisibility = "Visible"; // show the loading window
            CollectionVis = "Collapsed"; // don't show the collections while we build the tree

            nodes.Clear(); // clear any existing nodes
            ObservableCollection<Models.SCCMCollection> results = new ObservableCollection<Models.SCCMCollection>(); // create a blank set for the nodes

            // async fun
            await Task.Run(() =>
            {
                results = Models.CollectionCollector.GetCollection(2); // search for collections of type 2 (i.e. device collections)
                Application.Current.Dispatcher.Invoke(() => // need to marshal this back to the thread that contains the UI
                {
                    nodes = results; // set the nodes to the results
                    dss.DataSource = nodes; // set the data source to the nodes
                    dvm.DataSourceSettings = dss; // set the dss
                    dvm.LayoutManager = lm; // set the lm
                });
            });

            LoadingVisibility = "Hidden"; // hide the loading window
            CollectionVis = "Visible"; // show the collections
            OnRefresh(new EventArgs()); // trigger a refresh in code behind
            CanRefreshCollectionsBool = true; // enable the refresh button
        }

        public DevicesViewModel()
        {
            dss.ParentId = "LimitToCollectionID"; // this is the parent of the collection
            dss.Id = "CollectionID"; // this is the id of the collection

            lm.Layout = dtl; // set the layout
            lm.RefreshFrequency = RefreshFrequency.Load; // refresh on load, but nothing else

            PageSettings ps = new PageSettings(); // set the page settings
            ps.PageBackground = new SolidColorBrush(Colors.Transparent); // no background
            ps.PageBorderBrush = new SolidColorBrush(Colors.Transparent); // no border

            dvm.PageSettings = ps; // apply the page settings to the view model
        }
    }
}

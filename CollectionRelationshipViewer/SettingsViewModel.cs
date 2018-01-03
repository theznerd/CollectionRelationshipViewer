using Caliburn.Micro;

namespace CollectionRelationshipViewer
{
    public class SettingsViewModel : PropertyChangedBase
    {
        // on load, set the value of these fields based on the registry
        public SettingsViewModel()
        {
            SecurityServerName = Models.Settings.GetServerName();
            SecuritySiteCode = Models.Settings.GetSiteCode();
        }

        // this is the field for the server name
        // was initially called security because
        // I was going to add credential support
        // to the application. However, that felt
        // too insecure.
        private string _securityServerName;
        public string SecurityServerName
        {
            get { return _securityServerName; }
            set
            {
                _securityServerName = value;
                NotifyOfPropertyChange(() => SecurityServerName);
                NotifyOfPropertyChange(() => CanSave);
            }
        }

        // this is the field for the site code
        private string _securitySiteCode;
        public string SecuritySiteCode
        {
            get { return _securitySiteCode; }
            set
            {
                _securitySiteCode = value;
                NotifyOfPropertyChange(() => SecuritySiteCode);
            }
        }

        // this is what happens when you save
        public void Save()
        {
            int ssnResult = Models.Settings.SetServerName(_securityServerName);
            int sscResult = Models.Settings.SetSiteCode(_securitySiteCode);
            if (ssnResult == 1 || sscResult == 1)
            {
                System.Windows.MessageBox.Show("Something went wrong saving the settings.");
            }
        }

        // this determines whether you can save
        public bool CanSave
        {
            get {
                if (!string.IsNullOrEmpty(_securityServerName) || !string.IsNullOrEmpty(_securitySiteCode))
                {
                    if(_securitySiteCode.Length > 0 && _securityServerName.Length > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
    }
}

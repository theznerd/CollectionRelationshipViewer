using Microsoft.Win32;

namespace CollectionRelationshipViewer.Models
{
    public static class Settings
    {
       /// <summary>
       ///  This class sets up the basic settings necessary
       ///  for the application to function. In this case
       ///  site code and site server name.
       /// </summary>
        const string userRoot = "HKEY_CURRENT_USER";
        const string subkey = "SOFTWARE\\Z-NERD\\CRV\\";
        const string keyName = userRoot + "\\" + subkey;

        // Return the server name from the registry
        public static string GetServerName()
        {
            return (string)Registry.GetValue(keyName, "Server", "");
        }

        // Set the server name in the registry
        public static int SetServerName(string ServerName)
        {
            try
            {
                Registry.SetValue(keyName, "Server", ServerName);
                return 0; // everything is cool...
            }
            catch
            {
                return 1; // something went wrong?
            }
        }

        // Get the site code from the registry
        public static string GetSiteCode()
        {
            return (string)Registry.GetValue(keyName, "SiteCode", "");
        }

        // Set the site code in the registry
        public static int SetSiteCode(string ServerName)
        {
            try
            {
                Registry.SetValue(keyName, "SiteCode", ServerName);
                return 0; // everything is cool...
            }
            catch
            {
                return 1; // something went wrong?
            }
        }
    }
}

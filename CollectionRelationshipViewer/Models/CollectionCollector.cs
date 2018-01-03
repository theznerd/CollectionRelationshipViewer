using System.Linq;
using System.Management;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace CollectionRelationshipViewer.Models
{
    public static class CollectionCollector
    {
        /// <summary>
        /// This is where things get interesting. The bulk of the work
        /// in data collection and class creation takes place here.
        /// </summary>
        /// <param name="ctype">Type 1 is a user collection, otherwise expect device collection.</param>
        /// <returns>Returns a ObservableCollection of SCCMCollection</returns>
        public static ObservableCollection<SCCMCollection> GetCollection(int ctype)
        {
            // Create a blank observable collection
            ObservableCollection<SCCMCollection> sc = new ObservableCollection<SCCMCollection>();

            // Retrieve the SCCM settings from the regsitry
            string SCCMServer = Settings.GetServerName();
            string SCCMSiteCode = Settings.GetSiteCode();

            // Create some blank strings to exclude the base collections
            string excludeCol;
            string excludeID;

            // Set the name and ID of the base collection. This will be the
            // same for any instance of ConfigMgr.
            if(ctype == 1)
            {
                excludeCol = "All Users and User Groups";
                excludeID = "SMS00004";
            }
            else
            {
                excludeCol = "All Systems";
                excludeID = "SMS00001";
            }

            // Wrapping this all in a try catch is probably a pretty lazy 
            // way to do this honestly, but it works. Pretty much the only
            // reason this fails is if you cannot talk to the SCCM server
            // or you've set the name and site code improperly.
            try
            {
                // sets the management scope to \\SCCMServer\ROOT\SMS\sms_SCCMSiteCode
                ManagementScope scope = new ManagementScope("\\\\" + SCCMServer + "\\ROOT\\SMS\\site_" + SCCMSiteCode);

                // We need the default collection to be the first object in the list
                // due to a limitation with the tree builder. So we're going to search
                // for that collection and then add it to the ObservableCollection.
                ObjectQuery query = new ObjectQuery("SELECT * FROM SMS_Collection WHERE CollectionId = '" + excludeID + "'");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
                ManagementObjectCollection allCollections = searcher.Get();
                foreach (ManagementObject sCol in allCollections.Cast<ManagementObject>().OrderBy(obj => obj["Name"]))
                {
                    sc.Add(new SCCMCollection() { CollectionID = excludeID, CollectionType = ctype, LimitToCollectionID = "", Name = excludeCol, LastRefreshTime = ManagementDateTimeConverter.ToDateTime(sCol["LastRefreshTime"].ToString()), MemberCount = sCol["MemberCount"].ToString() });
                }

                // Now we'll search for the rest of the collections by type but exclude
                // the default collection. Since all collections build off of that the 
                // remaining order of collections is not important.
                query = new ObjectQuery("SELECT * FROM SMS_Collection WHERE CollectionType = " + ctype + " AND NOT CollectionId = '" + excludeID + "'");
                searcher = new ManagementObjectSearcher(scope, query);
                allCollections = searcher.Get();
                
                // This gets a bit more involved, but essentially we're just setting 
                // an SCCMCollection object with the information pulled from WMI.
                // We're also ordering them by name so that they show up in alphabetical
                // order per tier.
                foreach (ManagementObject sCol in allCollections.Cast<ManagementObject>().OrderBy(obj => obj["Name"]))
                {
                    SCCMCollection oCol = new SCCMCollection();  // create a new SCCMCollection object
                    oCol.CollectionID = sCol["CollectionId"].ToString(); // set the collection ID
                    oCol.CollectionType = int.Parse(sCol["CollectionType"].ToString()); // convert the collection type from an integer to a string
                    oCol.LastRefreshTime = ManagementDateTimeConverter.ToDateTime(sCol["LastRefreshTime"].ToString()); // convert the last refresh time from a WMI time to normal datetime
                    oCol.MemberCount = sCol["MemberCount"].ToString(); // set the member count
                    if (string.IsNullOrEmpty((string)sCol["LimitToCollectionID"]))
                    {
                        oCol.LimitToCollectionID = ""; // it doesn't have a limiting collection... this shouldn't be possible.
                    }
                    else
                    {
                        oCol.LimitToCollectionID = sCol["LimitToCollectionID"].ToString(); // set the limiting collection
                    }
                    oCol.Name = sCol["Name"].ToString(); // set the name of the collection

                    List<string> ics = new List<string>(); // create an empty list of include collections
                    List<string> ecs = new List<string>(); // create an empty list of exclude collections
                    Dictionary< string, string> qcs = new Dictionary<string, string>(); // create an empty dictionary of queries (name, query)
                    List<string> dcs = new List<string>();  // create an empty list of direct memberships

                    ManagementObject ic = new ManagementObject(); // now we have to get the list of include/exclude collections, queries, and direct relationships
                    ManagementPath mp = new ManagementPath(sCol["__PATH"].ToString()); // this is the path to the object in WMI so that we can get the lazy properties
                    ic.Path = mp;
                    ic.Get(); // grab all the lazy properties from this object in WMI

                    // shoot me now... this has to be the worst way to handle this
                    // maybe I'll find a way to check for null entries on mbos...
                    try
                    {
                        ManagementBaseObject[] mbos = (ManagementBaseObject[])ic["CollectionRules"]; // this represents all of the collection rules
                        if (mbos.Count() > 0)
                        {
                            foreach (ManagementBaseObject mbo in mbos)
                            {
                                // exclude collections
                                if (mbo["__CLASS"].ToString() == "SMS_CollectionRuleExcludeCollection")
                                {
                                    ecs.Add(mbo["ExcludeCollectionID"].ToString() + ": " + mbo["RuleName"].ToString());
                                }
                                // include collections
                                else if (mbo["__CLASS"].ToString() == "SMS_CollectionRuleIncludeCollection")
                                {
                                    ics.Add(mbo["IncludeCollectionID"].ToString() + ": " + mbo["RuleName"].ToString());
                                }
                                // query rules
                                else if (mbo["__CLASS"].ToString() == "SMS_CollectionRuleQuery")
                                {
                                    qcs.Add(mbo["RuleName"].ToString(), mbo["QueryExpression"].ToString());
                                }
                                // direct rules
                                else if (mbo["__CLASS"].ToString() == "SMS_CollectionRuleDirect")
                                {
                                    dcs.Add(mbo["RuleName"].ToString() + " (Resource ID: " + mbo["ResourceID"] + ")");
                                }
                            }
                        }
                        
                    }
                    catch
                    {
                        // bummer dude.
                        // basically this just means that the collection
                        // doesn't have any rules
                    }

                    // Connvert the refresh schedule to a readable format
                    try
                    {
                        oCol.RefreshSchedule = ScheduleConverter.ConvertSchedule(ic["RefreshType"].ToString(), (ManagementBaseObject[])ic["RefreshSchedule"]);
                    }
                    catch
                    {
                    oCol.RefreshSchedule = "Unknown";
                    }
                    

                    oCol.IncludeCollections = ics; // set the include collections
                    oCol.ExcludeCollections = ecs; // set the exclude collections
                    oCol.QueryRules = qcs; // set the query rules
                    oCol.DirectMembership = dcs; // set the direct memberships

                    sc.Add(oCol); // add the collection to the ObservableCollection
                }
            }
            catch
            {
                // Something went wrong when trying to get data from the SCCM server.
                System.Windows.MessageBox.Show("An error occurred. Did you set your SCCM server name and site code properly?");
                sc.Clear();
            }
            return sc;
        }
    }
}

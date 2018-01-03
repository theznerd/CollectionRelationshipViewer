using System;
using System.Collections.Generic;

namespace CollectionRelationshipViewer.Models
{
    public class SCCMCollection
    {
        /// <summary>
        /// This class contains all the information necessary
        /// to populate the collection view. Each object will
        /// represent a single collection. Everything is managed
        /// with a default getter/setter.
        /// </summary>
        public string CollectionID { get; set; }
        public int CollectionType { get; set; }
        public string LimitToCollectionID { get; set; }
        public string Name { get; set; }
        public DateTime LastRefreshTime { get; set; }
        public string MemberCount { get; set; }
        public List<string> IncludeCollections { get; set; }
        public List<string> ExcludeCollections { get; set; }
        public List<string> DirectMembership { get; set; }
        public Dictionary<string, string> QueryRules { get; set; }
        public string RefreshSchedule { get; set; }
    }
}

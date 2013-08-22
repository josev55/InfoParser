using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfoParser
{
    class ConnectionModel
    {
        private String connectionName;

        public String ConnectionName
        {
            get { return connectionName; }
            set { connectionName = value; }
        }

        private String cbValue;

        public String CBValue
        {
            get { return cbValue; }
            set { cbValue = value; }
        }

        private String cbText;

        public String CBText
        {
            get { return cbText; }
            set { cbText = value; }
        }

        private String comboboxID;

        public String CBId
        {
            get { return comboboxID; }
            set { comboboxID = value; }
        }

        private Boolean hasRelationship;
                                        
        public Boolean HasRelationship
        {
            get { return hasRelationship; }
            set { hasRelationship = value; }
        }

        private String relationshipMember;

        public String RelationshipMember
        {
            get { return relationshipMember; }
            set { relationshipMember = value; }
        }

        private String relatedProperty;

        public String RelatedProperty
        {
            get { return relatedProperty; }
            set { relatedProperty = value; }
        }
        
        public void cleanInfo()
        {
            var startIndex = connectionName.IndexOf("(\"");
            var endIndex = connectionName.IndexOf("\")");
            connectionName = connectionName.Substring(startIndex + 2, endIndex - 2 - startIndex) + "_offline.xml";
            cbValue = cbValue.Substring(cbValue.IndexOf(":") + 1);
            cbText = cbText.Substring(cbText.IndexOf(":") + 1);
        }
        
        
    }
}

namespace EntityLib
{
    public class MetabaseProperty
    {
        public int DBID { get; set; }
        
        public int PropId { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public string WWWTemplatePage { get; set; }
        
        public int UpdateStatus { get; set; }

        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        protected bool Equals(MetabaseProperty other)
        {
            return DBID == other.DBID && PropId == other.PropId 
                   && Name == other.Name && Description == other.Description 
                   && WWWTemplatePage == other.WWWTemplatePage && UpdateStatus == other.UpdateStatus;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(DBID, PropId, Name, Description, WWWTemplatePage, UpdateStatus);
        }
    }
}
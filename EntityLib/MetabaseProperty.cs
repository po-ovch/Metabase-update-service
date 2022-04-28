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

        public override bool Equals(Object other)
        {
            // TODO пофиксить наллы
            var otherProperty = (MetabaseProperty) other;
            return DBID == otherProperty.DBID && PropId == otherProperty.PropId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(DBID, PropId);
        }

        public void Modify(MetabaseProperty property)
        {
            Name = property.Name;
            Description = property.Description;
            WWWTemplatePage = property.WWWTemplatePage;
            UpdateStatus = property.UpdateStatus;
        }
    }
}
namespace Core.Services.Data
{
    public class PersistentDataServiceConfiguration : ServiceConfiguration
    {
        public string PersistentDataDirectoryName = "PersistentData";
        public string DataFileExtension = ".core";
        public override Service ServiceClass => new PersistentDataService(this);
    }
}
using System.Configuration;

namespace Mvc5.DocDB.Infrastructure.Configuration
{
    public interface IDocumentDBSettings
    {
        string DocDBUrl { get; }
        string DocDBPrimaryKey { get; }
        string DatabaseName { get; }
    }

    public class DocumentDBSettings : IDocumentDBSettings
    {
        private DocumentDBSettings()  { }

        public static IDocumentDBSettings Current()
        {
            return new DocumentDBSettings();
        }

        public string DocDBUrl
        {
            get
            {
               return ConfigurationManager.AppSettings["DocDBUrl"];
            }
        }

        public string DocDBPrimaryKey
        {
            get
            {
                return ConfigurationManager.AppSettings["DocDBPrimaryKey"];
            }
        }

        public string DatabaseName
        {
            get
            {
                return ConfigurationManager.AppSettings["DatabaseName"];
            }
        }
    }
}
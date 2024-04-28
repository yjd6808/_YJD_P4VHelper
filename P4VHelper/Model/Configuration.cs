// jdyun 24/04/26(금)

using System.Net.Mime;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using P4VHelper.Base;
using P4VHelper.Engine.Model;

namespace P4VHelper.Model
{
    public class Configuration : Bindable
    {
        private readonly P4VConfig p4_ = new ();
        private XDocument xDoc_ = new ();

        public P4VConfig P4VConfig => p4_;

        public string P4Uri
        {
            get => p4_.Uri;
            set
            {
                p4_.Uri = value;
                OnPropertyChanged();
            }
        }

        public string P4UserName
        {
            get => p4_.UserName;
            set
            {
                p4_.UserName = value;
                OnPropertyChanged();
            }
        }

        public string P4Workspace
        {
            get => p4_.Workspace;
            set
            {
                p4_.Workspace = value;
                OnPropertyChanged();
            }
        }

        public static Configuration Load()
        {
            Configuration config = new Configuration();
            P4VConfig p4 = config.p4_;

            config.xDoc_ = XDocument.Load("configuration.xml");
            XElement perforceElement = config.xDoc_.Descendants("P4VConfig").FirstOrDefault();

            p4.Uri = perforceElement.Attribute("uri").Value;
            p4.UserName = perforceElement.Attribute("user_name").Value;
            p4.Workspace = perforceElement.Attribute("workspace").Value;
            p4.ReadDelay = int.Parse(perforceElement.Attribute("read_delay").Value);
            p4.RefreshSize = int.Parse(perforceElement.Attribute("refresh_size").Value);
            p4.SegmentSize = int.Parse(perforceElement.Attribute("segment_size").Value);

            foreach (XElement searcherElement in perforceElement.Elements("Searcher"))
            {
                P4VConfig.Searcher searcher = new ();

                searcher.Path = searcherElement.Attribute("path").Value;
                searcher.Alias = searcherElement.Attribute("alias").Value;
                searcher.CachedSegmentCount = int.Parse(searcherElement.Attribute("cached_segment_count").Value);

                p4.Searchers.Add(searcher);
            }

            p4.Validate();
            return config;
        }
    }
}

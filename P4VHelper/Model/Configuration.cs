// jdyun 24/04/26(금)

using System.Net.Mime;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Linq;
using P4VHelper.Base;
using P4VHelper.Engine.Collection;
using P4VHelper.Engine.Model;
using static P4VHelper.Base.Extension.InterlockedEx;
using static P4VHelper.Engine.Model.P4VConfig;

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

        private static SegmentType ParseSegmentType(string _type)
        {
            if (_type == "changelist")
                return SegmentType.Changelist;
            if (_type == "changelistbyuser")
                return SegmentType.ChangelistByUser;

            throw new Exception("올바르지 않은 세그먼트 그룹 타입입니다.");
        }

        public static Configuration Load()
        {
            Configuration config = new Configuration();
            try
            {
                P4VConfig p4 = config.p4_;
                config.xDoc_ = XDocument.Load("configuration.xml");
                XElement perforceElement = config.xDoc_.Descendants("P4VConfig").FirstOrDefault();

                p4.Uri = perforceElement.Attribute("uri").Value;
                p4.UserName = perforceElement.Attribute("user_name").Value;
                p4.Workspace = perforceElement.Attribute("workspace").Value;
                p4.ReadDelay = int.Parse(perforceElement.Attribute("read_delay").Value);
                p4.RefreshSegmentCount = int.Parse(perforceElement.Attribute("refresh_segment_count").Value);

                foreach (XElement segElement in perforceElement.Elements("SegmentGroup"))
                {
                    P4VConfig.SegmentGroup segmentGroup = new();

                    string groupType = segElement.Attribute("type").Value;

                    segmentGroup.Type = ParseSegmentType(groupType);
                    XAttribute? linkTypeAttribute = segElement.Attribute("link_type");
                    XAttribute? linkAliasAttribute = segElement.Attribute("link_alias");

                    if (linkTypeAttribute != null || linkAliasAttribute != null)
                    {
                        SegmentType linkType = ParseSegmentType(linkTypeAttribute.Value);
                        string linkAlias = linkAliasAttribute.Value;

                        P4VConfig.SegmentGroup linkSegmentGorup = p4.GetGroup(linkType, linkAlias);
                        segmentGroup.Link = linkSegmentGorup;
                    }
                    else
                    {
                        segmentGroup.Path = segElement.Attribute("path").Value;
                        segmentGroup.Alias = segElement.Attribute("alias").Value;
                    }

                    segmentGroup.CachedSegmentCount = int.Parse(segElement.Attribute("cached_segment_count").Value);
                    segmentGroup.SegmentSize = int.Parse(segElement.Attribute("segment_size").Value);

                    IEnumerable<XElement> filters = segElement.Elements("Filter");
                    foreach (XElement filterElement in filters)
                    {
                        var filter = new P4VConfig.SegmentGroup.Filter();

                        string filterMode = filterElement.Attribute("mode").Value;
                        string filterType = filterElement.Attribute("type").Value;

                        if (filterMode == "start_with")
                            filter.Mode = P4VConfig.SegmentGroup.FilterMode.StartWith;
                        else if (filterMode == "contain")
                            filter.Mode = P4VConfig.SegmentGroup.FilterMode.Contain;
                        else if (filterMode == "end_with")
                            filter.Mode = P4VConfig.SegmentGroup.FilterMode.EndWith;
                        else
                            throw new Exception("올바르지 않은 필터 모드입니다.");

                        if (filterType == "string")
                            filter.Type = P4VConfig.SegmentGroup.FilterType.String;
                        else if (filterType == "type")
                            filter.Type = P4VConfig.SegmentGroup.FilterType.Type;
                        else
                            throw new Exception("올바르지 않은 필터 타입입니다.");

                        filter.Value = filterElement.Attribute("value").Value;

                        if (string.IsNullOrEmpty(filter.Value))
                            throw new Exception("필터 value 애튜리뷰트가 비어있습니다.");

                        segmentGroup.Filters.Add(filter);
                    }

                    segmentGroup.ConstructRegex();
                    p4.AddSegmentGroup(segmentGroup);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            return config;
        }
    }
}

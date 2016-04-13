using System.Xml.Serialization;
using System.Collections.Generic;
using System.Web;

namespace CongCuDungCu.Code
{
    public class OverviewPageModel : DemoPageModel
    {
        private List<KeyFeatureModel> _keyFeatures = new List<KeyFeatureModel>();

        [XmlIgnore]
        public override string Key
        {
            get
            {
                return "Overview";
            }
            set
            {
            }
        }
        [XmlElement("KeyFeature")]
        public List<KeyFeatureModel> KeyFeatures
        {
            get
            {
                return _keyFeatures;
            }
            set
            {
                _keyFeatures = value;
            }
        }
        [XmlElement("DescriptionTitle")]
        public string DescriptionTitle { get; set; }
    }
    public class KeyFeatureModel
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string DemoUrl { get; set; }
        [XmlElement]
        public string Description { get; set; }
        public string GetNameHtml()
        {
            return !string.IsNullOrEmpty(DemoUrl) ? string.Format("<a href='{0}'>{1}</a>", VirtualPathUtility.ToAbsolute(DemoUrl), Name) : Name;
        }
    }
}

using System.Xml.Serialization;
using System.Collections.Generic;

namespace CongCuDungCu.Code
{
    public class IntroPageModel : DemoModel
    {
        private string _bannerTitle;
        private string _bannerText;
        private string _bannerImageUrl;
        private string _bannerUrl;
        private string _descriptionTitle;
        private string _descriptionFooter;
        private List<ExternalDemoModel> _externalDemos = new List<ExternalDemoModel>();

        [XmlIgnore]
        public override string Key
        {
            get
            {
                return Utils.IsMvc ? "Index" : "Default";
            }
            set
            {
            }
        }

        [XmlElement]
        public string BannerTitle
        {
            get
            {
                if (_bannerTitle == null)
                {
                    return string.Empty;
                }
                return _bannerTitle;
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                }
                _bannerTitle = value;
            }
        }

        [XmlElement]
        public string BannerText
        {
            get
            {
                if (_bannerText == null)
                {
                    return string.Empty;
                }
                return _bannerText;
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                }
                _bannerText = value;
            }
        }

        [XmlElement]
        public string BannerImageUrl
        {
            get
            {
                if (_bannerImageUrl == null)
                {
                    return string.Empty;
                }
                return _bannerImageUrl;
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                }
                _bannerImageUrl = value;
            }
        }

        [XmlElement]
        public string BannerUrl
        {
            get
            {
                if (_bannerUrl == null)
                {
                    return string.Empty;
                }
                return _bannerUrl;
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                }
                _bannerUrl = value;
            }
        }

        [XmlElement]
        public string DescriptionTitle
        {
            get
            {
                if (_descriptionTitle == null)
                {
                    return string.Empty;
                }
                return _descriptionTitle;
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                }
                _descriptionTitle = value;
            }
        }


        [XmlElement]
        public string DescriptionFooter
        {
            get
            {
                if (_descriptionFooter == null)
                {
                    return string.Empty;
                }
                return _descriptionFooter;
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                }
                _descriptionFooter = value;
            }
        }

        [XmlElement("ExternalDemo")]
        public List<ExternalDemoModel> ExternalDemos
        {
            get
            {
                return _externalDemos;
            }
        }
    }
}

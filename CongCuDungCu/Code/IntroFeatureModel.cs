using System.Xml.Serialization;
using System.Collections.Generic;

namespace CongCuDungCu.Code
{
    public class IntroFeatureModel
    {
        private string _title;
        private string _description;
        private string _imageUrl;

        [XmlAttribute]
        public string Title
        {
            get
            {
                if (_title == null)
                {
                    return string.Empty;
                }
                return _title;
            }
            set
            {
                _title = value;
            }
        }

        [XmlAttribute]
        public string ImageUrl
        {
            get
            {
                if (_imageUrl == null)
                {
                    return string.Empty;
                }
                return _imageUrl;
            }
            set
            {
                _imageUrl = value;
            }
        }

        [XmlElement]
        public string Description
        {
            get
            {
                if (_description == null)
                {
                    return string.Empty;
                }
                return _description;
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                }
                _description = value;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CongCuDungCu.Code
{
    public class DemoModelBase : ModelBase
    {
        private string _key;
        private string _title;
        private string _seoTitle;

        [XmlAttribute]
        public virtual string Key
        {
            get
            {
                if (_key == null)
                {
                    return string.Empty;
                }
                return _key;
            }
            set
            {
                _key = value;
            }
        }

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
        public string SeoTitle
        {
            get
            {
                if (_seoTitle == null)
                {
                    return string.Empty;
                }
                return _seoTitle;
            }
            set
            {
                _seoTitle = value;
            }
        }

        [XmlAttribute]
        public bool IsNew { get; set; }

        [XmlAttribute]
        public bool IsUpdated { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }

    public class ModelBase
    {
        private string _keywords;
        private Dictionary<string, int> _keywordsRankList;

        [XmlElement]
        public string Keywords
        {
            get
            {
                if (_keywords == null)
                {
                    return string.Empty;
                }
                return _keywords;
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                }
                _keywords = value;
            }
        }

        [XmlIgnore]
        public Dictionary<string, int> KeywordsRankList
        {
            get
            {
                if (_keywordsRankList == null)
                {
                    _keywordsRankList = SearchUtils.GetKeywordsRankList(this);
                }
                return _keywordsRankList;
            }
        }
    }
}

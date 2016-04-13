using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace CongCuDungCu.Code
{
    public class ThemeModelBase
    {
        private string _name;
        private string _title;
        private string _key;
        [XmlAttribute]
        public string Name
        {
            get
            {
                if (_name == null)
                {
                    return string.Empty;
                }
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        [XmlAttribute]
        public string Key
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
    }
}

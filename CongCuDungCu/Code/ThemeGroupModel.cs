using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CongCuDungCu.Code
{
    public class ThemeGroupModel : ThemeModelBase
    {
        private List<ThemeModel> _themes = new List<ThemeModel>();

        [XmlElement(ElementName = "Theme")]
        public List<ThemeModel> Themes
        {
            get
            {
                return _themes;
            }
        }

        [XmlAttribute("Float")]
        public string Float { get; set; }
    }
}

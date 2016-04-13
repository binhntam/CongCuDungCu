using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace CongCuDungCu.Code
{
    public class ThemeModel : ThemeModelBase
    {
        private string _spriteCssClass;

        [XmlAttribute]
        public string SpriteCssClass
        {
            get
            {
                if (_spriteCssClass == null)
                {
                    return string.Empty;
                }
                return _spriteCssClass;
            }
            set
            {
                _spriteCssClass = value;
            }
        }
    }
}

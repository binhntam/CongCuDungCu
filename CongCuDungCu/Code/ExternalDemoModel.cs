using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CongCuDungCu.Code
{
    public class ExternalDemoModel
    {
        [XmlAttribute]
        public string ImageUrl { get; set; }

        [XmlAttribute]
        public string Url { get; set; }

        [XmlAttribute]
        public string Title { get; set; }
    }
}

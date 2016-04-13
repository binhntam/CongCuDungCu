using System.Xml.Serialization;
using System.Collections.Generic;

namespace CongCuDungCu.Code
{
    public class DemoGroupModel : DemoModelBase
    {
        private List<DemoPageModel> _demos = new List<DemoPageModel>();

        [XmlElement(Type = typeof(DemoPageModel), ElementName = "Demo")]
        public List<DemoPageModel> Demos
        {
            get
            {
                return _demos;
            }
        }

        [XmlElement(Type = typeof(OverviewPageModel), ElementName = "Overview")]
        public OverviewPageModel Overview { get; set; }

        public DemoModel FindDemo(string key)
        {
            key = key.ToLower();
            foreach (DemoModel demo in Demos)
            {
                if (key == demo.Key.ToLower())
                {
                    return demo;
                }
            }
            if (Overview != null && key == Overview.Key.ToLower())
            {
                return Overview;
            }
            return null;
        }

        public List<DemoPageModel> GetAllDemos()
        {
            var result = new List<DemoPageModel>();
            if (Overview != null)
            {
                var overviewDemo = Overview as DemoPageModel;
                overviewDemo.Key = Overview.Key;
                result.Add(overviewDemo);
            }
            result.AddRange(Demos);
            return result;
        }

        [XmlIgnore]
        public DemoProductModel Product { get; set; }
    }
}

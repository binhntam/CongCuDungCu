
using System.Collections.Generic;
using System.Linq;
using System;
using System.Xml.Serialization;
using System.Configuration;
using System.IO;
using System.Web;

namespace CongCuDungCu.Code
{
    [XmlRoot("Demos")]
    public class DemosModel
    {
        private static DemosModel _instance;
        private static DemoProductModel _current;
        private GlobalHeaderModel _globalHeader = new GlobalHeaderModel();

        private static readonly object _instanceLock = new object();
        private static readonly object _currentLock = new object();

        public static DemosModel Instance
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null)
                    {
                        using (var stream = File.OpenRead(HttpContext.Current.Server.MapPath("~/App_Data/Demos.xml")))
                        {
                            var serializer = new XmlSerializer(typeof(DemosModel));
                            _instance = (DemosModel)serializer.Deserialize(stream);
                        }
                        foreach (var demoProduct in _instance.DemoProducts)
                        {
                            foreach (var group in demoProduct.Groups)
                            {
                                foreach (var demo in group.GetAllDemos())
                                {
                                    demo.Group = group;
                                    demo.Product = demoProduct;
                                }
                                group.Product = demoProduct;
                            }
                            if (demoProduct.Intro != null)
                            {
                                demoProduct.Intro.Product = demoProduct;
                            }
                            if (demoProduct.Overview != null)
                            {
                                demoProduct.Overview.Product = demoProduct;
                            }
                        }
                    }
                    return _instance;
                }
            }
        }
        public static DemoProductModel Current
        {
            get
            {
                lock (_currentLock)
                {
                    if (_current == null)
                    {
                        _current = Instance.DemoProducts.FirstOrDefault(dp => dp.Key == ConfigurationManager.AppSettings["DemoProduct"]);
                    }
                    if (_current == null)
                    {
                        throw new Exception("The current demo is not found");
                    }
                    return _current;
                }
            }
        }

        private List<DemoProductModel> _demoProducts = new List<DemoProductModel>();
        private List<DemoProductModel> _sortedDemoProducts;

        [XmlElement("DemoProduct")]
        public List<DemoProductModel> DemoProducts
        {
            get
            {
                return _demoProducts;
            }
        }

        [XmlIgnore]
        public List<DemoProductModel> SortedDemoProducts
        {
            get
            {
                if (_sortedDemoProducts == null)
                {
                    _sortedDemoProducts = _demoProducts.OrderBy(p => p.OrderIndex).ToList();
                }
                return _sortedDemoProducts;
            }
        }

        [XmlElement("Search")]
        public SearchModel Search { get; set; }
        [XmlElement("GlobalHeader")]
        public GlobalHeaderModel GlobalHeader
        {
            get
            {
                return _globalHeader;
            }
            set
            {
                _globalHeader = value;
            }
        }
        [XmlAttribute]
        public bool ExpandAllDemosAtFirstTime { get; set; }
        [XmlAttribute]
        public bool DisableTextWrap { get; set; }
    }
    public class GlobalHeaderModel
    {
        private string _logoPlatformSubject = "ASP.NET AJAX";
        private string _logoPlatformDescription = "WHEN THE WEB MEANS BUSINESS";
        [XmlAttribute]
        public string LogoPlatformSubject
        {
            get
            {
                return _logoPlatformSubject;
            }
            set
            {
                _logoPlatformSubject = value;
            }
        }
        [XmlAttribute]
        public string LogoPlatformDescription
        {
            get
            {
                return _logoPlatformDescription;
            }
            set
            {
                _logoPlatformDescription = value;
            }
        }
    }
    public class DemoProductModel : ModelBase
    {
        private string _key;
        private string _url;
        private string _title;
        private string _seoTitle;
        private string _navItemTitle;
        private bool _supportsTheming = true;
        private bool _hideNavItem = false;
        private List<DemoGroupModel> _groups = new List<DemoGroupModel>();

        private int _orderIndex = 1000;
        private bool _integrationHighlighted = false;

        private string _downloadUrl;
        private string _buyUrl;
        private string _docUrl;

        private List<DemoPageModel> _highlighledDemos;

        [XmlAttribute]
        public bool IsMvc { get; set; }

        [XmlAttribute]
        public bool IsMvcRazor { get; set; }

        [XmlAttribute]
        public bool IsRootDemo { get; set; }

        [XmlAttribute]
        public bool IsPreview { get; set; }

        [XmlAttribute]
        public bool HideNavItem
        {
            get
            {
                return _hideNavItem;
            }
            set
            {
                _hideNavItem = value;
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
        public string Url
        {
            get
            {
                if (_url == null)
                {
                    throw new Exception("URL is not defined");
                }
                return _url;
            }
            set
            {
                _url = value;
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
        public string NavItemTitle
        {
            get
            {
                if (_navItemTitle == null)
                {
                    return string.Empty;
                }
                return _navItemTitle;
            }
            set
            {
                _navItemTitle = value;
            }
        }

        [XmlAttribute]
        public int OrderIndex
        {
            get
            {
                return _orderIndex;
            }
            set
            {
                _orderIndex = value;
            }
        }

        [XmlAttribute]
        public bool IntegrationHighlighted
        {
            get
            {
                return _integrationHighlighted;
            }
            set
            {
                _integrationHighlighted = value;
            }
        }

        [XmlElement]
        public string DownloadUrl
        {
            get
            {
                if (_downloadUrl == null)
                {
                    return string.Empty;
                }
                return _downloadUrl;
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                }
                _downloadUrl = value;
            }
        }

        [XmlElement]
        public string BuyUrl
        {
            get
            {
                if (_buyUrl == null)
                {
                    return string.Empty;
                }
                return _buyUrl;
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                }
                _buyUrl = value;
            }
        }

        [XmlElement]
        public string DocUrl
        {
            get
            {
                if (_docUrl == null)
                {
                    return string.Empty;
                }
                return _docUrl;
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                }
                _docUrl = value;
            }
        }

        [XmlElement]
        public string IntegrationImageUrl { get; set; }

        [XmlElement]
        public string IntegrationDescription { get; set; }

        [XmlElement("DemoGroup")]
        public List<DemoGroupModel> Groups
        {
            get
            {
                return _groups;
            }
        }

        [XmlAttribute]
        public bool IE7CompatModeRequired { get; set; }

        [XmlAttribute]
        public bool SupportsTheming
        {
            get
            {
                return _supportsTheming;
            }
            set
            {
                _supportsTheming = value;
            }
        }

        [XmlIgnore]
        public List<DemoPageModel> HighlightedDemos
        {
            get
            {
                if (_highlighledDemos == null)
                {
                    _highlighledDemos = CreateHighlightedDemos();
                }
                return _highlighledDemos;
            }
        }

        [XmlElement(Type = typeof(IntroPageModel), ElementName = "Intro")]
        public IntroPageModel Intro { get; set; }

        [XmlElement(Type = typeof(OverviewPageModel), ElementName = "Overview")]
        public OverviewPageModel Overview { get; set; }

        [XmlIgnore]
        public bool IsCurrent
        {
            get
            {
                return this == DemosModel.Current;
            }
        }

        public DemoGroupModel FindGroup(string key)
        {
            key = key.ToLower();
            foreach (DemoGroupModel group in Groups)
            {
                if (key == group.Key.ToLower())
                {
                    return group;
                }
            }
            return null;
        }

        private List<DemoPageModel> CreateHighlightedDemos()
        {
            var result = new List<DemoPageModel>();
            foreach (DemoGroupModel group in Groups)
            {
                foreach (DemoPageModel demo in group.Demos)
                {
                    if (demo.HighlightedIndex > -1)
                    {
                        result.Add(demo);
                    }
                }
            }
            result.Sort(CompareHighlightedDemos);
            return result;
        }

        private int CompareHighlightedDemos(DemoModel x, DemoModel y)
        {
            return Comparer<int>.Default.Compare(x.HighlightedIndex, y.HighlightedIndex);
        }

        public string GetSeoTitle()
        {
            if (!string.IsNullOrEmpty(SeoTitle))
            {
                return SeoTitle;
            }
            return Title;
        }
    }
    public class SearchModel
    {
        [XmlElement]
        public SynonymsSearchModel Synonyms { get; set; }

        [XmlElement]
        public ExclusionsSearchModel Exclusions { get; set; }
    }
    public class ExclusionsSearchModel
    {
        [XmlElement]
        public string Words { get; set; }
        [XmlElement]
        public string Prefixes { get; set; }
        [XmlElement]
        public string Postfixes { get; set; }
    }
    public class SynonymsSearchModel
    {
        private List<string> _groups = new List<string>();

        [XmlElement("Group")]
        public List<string> Groups
        {
            get
            {
                return _groups;
            }
            set
            {
                _groups = value;
            }
        }
    }
}

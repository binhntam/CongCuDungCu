using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxClasses.Internal;
using DevExpress.Web.ASPxClasses;

namespace CongCuDungCu.Code
{
    public class SourceCodePage
    {
        public string Title = string.Empty;
        public string Code = string.Empty;
        public bool Expanded = false;

        public SourceCodePage(string title, string code, bool expanded)
        {
            Title = title;
            Code = code;
            Expanded = expanded;
        }
    }

    public class FeaturedDemoInfo
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string NavigateUrl { get; set; }
        public string ImageUrl { get; set; }
    }

    public class ProductInfo
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string NavigateUrl { get; set; }
        public string ImageUrl { get; set; }
    }

    public static class Utils
    {
        private const string
        CurrentDemoKey = "DXCurrentDemo",
        CurrentThemeCookieKeyPrefix = "DXCurrentTheme",
        DefaultTheme = "Aqua";

        private static readonly Dictionary<DemoModel, IEnumerable<SourceCodePage>> sourceCodeCache = new Dictionary<DemoModel, IEnumerable<SourceCodePage>>();
        private static readonly object sourceCodeCacheLock = new object();

        private static string _codeLanguage;

        private static HttpContext Context
        {
            get
            {
                return HttpContext.Current;
            }
        }

        private static HttpRequest Request
        {
            get
            {
                return Context.Request;
            }
        }

        public static bool IsMvc
        {
            get
            {
                return DemosModel.Current.IsMvc;
            }
        }

        public static DemoModel CurrentDemo
        {
            get
            {
                return (DemoModel)Context.Items[CurrentDemoKey];
            }
        }
        public static DemoProductModel RootProduct
        {
            get
            {
                return DemosModel.Instance.SortedDemoProducts.Find(p => p.IsRootDemo);
            }
        }
        public static string CurrentDemoNodeName
        {
            get
            {
                if (IsOverview && CurrentOverview.Group == null)
                {
                    return string.Format("{0}_{1}", CurrentOverview.Product.Key, CurrentOverview.Key);
                }
                if (CurrentDemoPage != null)
                {
                    return string.Format("{0}_{1}_{2}", CurrentDemoPage.Product.Key, CurrentDemoPage.Group.Key, CurrentDemoPage.Key);
                }
                if (CurrentDemo != null)
                {
                    return CurrentDemo.Product.Key;
                }
                return null;
            }
        }

        public static string CurrentDemoTitleHtml
        {
            get
            {
                return GetDemoTitleHtml(CurrentDemo);
            }
        }

        public static IntroPageModel CurrentIntro
        {
            get
            {
                if (CurrentDemo is IntroPageModel || CurrentDemo == null)
                {
                    return (IntroPageModel)CurrentDemo;
                }
                return CurrentDemo.Product.Intro;
            }
        }
        public static OverviewPageModel CurrentOverview
        {
            get
            {
                if (CurrentDemo is OverviewPageModel || CurrentDemo == null)
                {
                    return (OverviewPageModel)CurrentDemo;
                }
                return CurrentDemo.Product.Overview;
            }
        }
        public static DemoPageModel CurrentDemoPage
        {
            get
            {
                return CurrentDemo as DemoPageModel;
            }
        }

        public static string CurrentThemeCookieKey
        {
            get
            {
                return CurrentThemeCookieKeyPrefix;
            }
        }

        public static string CurrentTheme
        {
            get
            {
                if (CanChangeTheme && Request.Cookies[CurrentThemeCookieKey] != null)
                {
                    return HttpUtility.UrlDecode(Request.Cookies[CurrentThemeCookieKey].Value);
                }
                return DefaultTheme;
            }
        }
        public static string CurrentThemeTitle
        {
            get
            {
                var theme = CurrentTheme;
                var themeModel = ThemesModel.Current.Groups.SelectMany(g => g.Themes).FirstOrDefault(t => t.Name == theme);
                return themeModel != null ? themeModel.Title : theme;
            }
        }

        public static bool IsIntro
        {
            get
            {
                return Utils.CurrentDemo is IntroPageModel;
            }
        }
        public static bool IsOverview
        {
            get
            {
                return Utils.CurrentDemo is OverviewPageModel;
            }
        }

        public static string GetDemoTitleHtml(DemoModel demo)
        {
            var result = string.Empty;
            if (demo is DemoPageModel)
            {
                result = string.Format("{0} - {1}", ((DemoPageModel)demo).Group.Title, demo.Title);
            }
            if (string.IsNullOrEmpty(result))
            {
                result = demo.Title;
            }
            else
            {
                if (result.Length > 60)
                {
                    result = demo.Title;
                }
            }
            return HttpUtility.HtmlEncode(result);
        }

        public static string CodeLanguage
        {
            get
            {
                if (_codeLanguage == null)
                {
                    try
                    {
                        using (var _file = File.OpenRead(Context.Server.MapPath("~/Site.master")))
                        {
                            using (TextReader reader = new StreamReader(_file))
                            {
                                var line = reader.ReadLine();
                                var match = Regex.Match(line, @"language=""([^""]+)", RegexOptions.IgnoreCase);
                                if (match.Success)
                                {
                                    _codeLanguage = match.Groups[1].Value.ToUpper();
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                    if (String.IsNullOrEmpty(_codeLanguage))
                    {
                        _codeLanguage = "C#";
                    }
                }
                return _codeLanguage;
            }
        }

        public static IEnumerable<SourceCodePage> GetCurrentSourceCodePages()
        {
            return GetSourceCodePages(CurrentDemo as DemoPageModel);
        }

        public static IEnumerable<SourceCodePage> GetSourceCodePages(DemoPageModel demo)
        {
            lock (sourceCodeCacheLock)
            {
                if (!sourceCodeCache.ContainsKey(demo))
                {
                    sourceCodeCache[demo] = CreateSourceCodePages(demo);
                }
                return sourceCodeCache[demo];
            }
        }

        private static IEnumerable<SourceCodePage> CreateSourceCodePages(DemoPageModel demo)
        {
            var result = new List<SourceCodePage>();
            if (IsMvc)
            {
                foreach (string fileName in demo.SourceFiles)
                {
                    if (fileName.StartsWith("~/Models/"))
                    {
                        AddSourceCodePage(result, string.Format("Model ({0})", Path.GetFileNameWithoutExtension(fileName)), fileName, false);
                    }
                }
                var controllerUrl = string.Format("~/Controllers/{0}/{0}Controller.{1}.cs", demo.Group.Key, demo.Key);
                AddSourceCodePage(result, "Controller", controllerUrl, true, false);
                var commonControllerUrl = string.Format("~/Controllers/{0}Controller.cs", demo.Group.Key);
                AddSourceCodePage(result, "Controller (common)", commonControllerUrl, false);
                var viewUrl = string.Format("~/Views/{0}/{1}.cshtml", demo.Group.Key, demo.Key);
                AddSourceCodePage(result, "View", viewUrl, true, false);
                foreach (string fileName in demo.SourceFiles)
                {
                    if (fileName.StartsWith("~/Views/"))
                    {
                        AddSourceCodePage(result, string.Format("View ({0})", Path.GetFileNameWithoutExtension(fileName)), fileName, true);
                    }
                    else
                    {
                        if (fileName.StartsWith("~/Code/"))
                        {
                            AddSourceCodePage(result, string.Format("{0}", Path.GetFileName(fileName)), fileName, true);
                        }
                        else
                        {
                            if (!fileName.StartsWith("~/Models/"))
                            {
                                AddSourceCodePage(result, Path.GetFileName(fileName), fileName, false);
                            }
                        }
                    }
                }
            }
            else
            {
                var baseUrl = GenerateDemoUrl(demo);
                AddSourceCodePage(result, "ASPX", baseUrl, true);
                AddSourceCodePage(result, "C#", baseUrl + ".cs", CodeLanguage == "C#");
                AddSourceCodePage(result, "VB", baseUrl + ".vb", CodeLanguage == "VB");
                foreach (string fileName in demo.SourceFiles)
                {
                    AddSourceCodePage(result, Path.GetFileName(fileName), fileName, false);
                }
            }
            return result;
        }

        private static void AddSourceCodePage(List<SourceCodePage> list, string title, string url, bool expanded)
        {
            AddSourceCodePage(list, title, url, expanded, true);
        }
        private static void AddSourceCodePage(List<SourceCodePage> list, string title, string url, bool expanded, bool showIfError)
        {
            var content = string.Empty;
            try
            {
                content = GetHighlightedFileContent(url);
            }
            catch
            {
                content = showIfError ? "Error rendering source code" : string.Empty;
            }
            if (!string.IsNullOrEmpty(content))
            {
                list.Add(new SourceCodePage(title, content, expanded));
            }
        }
        private static string GetHighlightedFileContent(string virtualPath)
        {
            var filePath = GetHighlightedFilePath(virtualPath);
            var text = File.ReadAllText(filePath);
            return CodeFormatter.GetFormattedCode(Path.GetExtension(filePath), text, IsMvc);
        }
        private static string GetHighlightedFilePath(string virtualPath)
        {
            var result = new DirectoryInfo(Context.Server.MapPath("~/")).FullName;
            result = Path.Combine(result, ".Source");
            result = Path.Combine(result, virtualPath.Substring(2));
            if (File.Exists(result))
            {
                return result;
            }
            result = Context.Server.MapPath(virtualPath);
            if (!File.Exists(result))
            {
                result = CorrectSourceFilePath(result);
            }
            return result;
        }

        private static string CorrectSourceFilePath(string filePath)
        {
            var csPathItem = String.Format("{0}cs{0}", Path.DirectorySeparatorChar);
            var vbPathItem = String.Format("{0}vb{0}", Path.DirectorySeparatorChar);
            filePath = filePath.ToLower();
            if (filePath.EndsWith(".cs"))
            {
                return filePath.Replace(vbPathItem, csPathItem);
            }
            if (filePath.EndsWith(".vb"))
            {
                return filePath.Replace(csPathItem, vbPathItem);
            }
            return filePath;
        }

        public static string GetVersionText()
        {
            AssemblyInfo.Version.Split('.');

            return string.Format("v2015 vol 1.0");
        }

        public static void RegisterCurrentWebFormsDemo(Page page)
        {
            var path = page.AppRelativeVirtualPath.Replace("~/", string.Empty).Replace(".aspx", string.Empty);
            var parts = path.Split('/');
            if (parts.Length < 1)
            {
                throw new ArgumentException("Invalid demo URL");
            }
            var groupKey = string.Empty;
            var demoKey = string.Empty;

            if (parts.Length > 1)
            {
                groupKey = parts[0];
                demoKey = parts[1];
            }
            else
            {
                demoKey = parts[0];
            }

            RegisterCurrentDemo(groupKey, demoKey);
        }

        public static void RegisterCurrentMvcDemoOnCallback()
        {
            var controllerName = string.Empty;
            var actionName = string.Empty;
            var demoUriParts = Request.UrlReferrer.Segments;
            var controllerNamePartIndex = demoUriParts.Length > 2 ? demoUriParts.Length - 2 : -1;
            if (controllerNamePartIndex > -1)
            {
                var controllerNamePart = demoUriParts[controllerNamePartIndex];
                if (Request.AppRelativeCurrentExecutionFilePath.Contains(controllerNamePart))
                {
                    controllerName = controllerNamePart.Replace("/", string.Empty);
                    actionName = demoUriParts[controllerNamePartIndex + 1];
                }
            }
            RegisterCurrentMvcDemo(!string.IsNullOrEmpty(controllerName) ? controllerName : "Home",
                !string.IsNullOrEmpty(actionName) ? actionName : "Index");
        }
        public static void RegisterCurrentMvcDemo(string controllerName, string actionName)
        {
            RegisterCurrentDemo(controllerName, actionName);
        }
        public static bool IsIntroPage(string groupKey, string demoKey)
        {
            if (IsMvc)
            {
                return groupKey.Equals("Home", StringComparison.InvariantCultureIgnoreCase) && demoKey.Equals("Index", StringComparison.InvariantCultureIgnoreCase);
            }
            return demoKey.Equals("default", StringComparison.InvariantCultureIgnoreCase);
        }
        public static bool IsOverviewPage(string demoKey)
        {
            return demoKey.Equals("overview", StringComparison.InvariantCultureIgnoreCase);
        }
        private static void RegisterCurrentDemo(string groupKey, string demoKey)
        {
            DemoModel demo = null;
            if (IsIntroPage(groupKey, demoKey))
            {
                demo = DemosModel.Current.Intro;
            }
            else
            {
                if (IsOverviewPage(demoKey) && String.IsNullOrEmpty(groupKey))
                {
                    demo = DemosModel.Current.Overview;
                }
                else
                {
                    var group = DemosModel.Current.FindGroup(groupKey);
                    if (group != null)
                    {
                        demo = group.FindDemo(demoKey);
                    }
                }
            }
            if (demo == null)
            {
                demo = CreateBogusDemoModel();
            }
            Context.Items[CurrentDemoKey] = demo;
            DevExpress.Web.ASPxClasses.Internal.DemoUtils.RegisterDemo(DemosModel.Current.Key, groupKey, demoKey);
        }

        private static DemoPageModel CreateBogusDemoModel()
        {
            var group = new DemoGroupModel();
            group.Title = "ASP.NET";

            var result = new DemoPageModel();
            result.Group = group;
            result.HideSourceCode = true;
            result.Title = "Delivered!";

            return result;
        }

        public static string GetCurrentDemoPageTitle()
        {
            var builder = new StringBuilder();
            if (CurrentDemo is IntroPageModel)
            {
                builder.Append(CurrentDemo.SeoTitle);
            }
            else
            {
                if (CurrentDemo is DemoPageModel)
                {
                    var product = DemosModel.Current.GetSeoTitle();
                    var demoGroup = ((DemoPageModel)CurrentDemo).Group;
                    var group = demoGroup != null ? demoGroup.SeoTitle : null;

                    builder.Append(CurrentDemo.GetSeoTitle());
                    builder.Append(" - ");
                    builder.Append(string.IsNullOrEmpty(group) ? product : group);
                    builder.Append(" Demo");
                }
            }
            builder.Append(" | DevExpress");
            return builder.ToString();
        }

        public static string GetDemoNavigationTitle()
        {
            var result = Utils.CurrentDemo.Product.NavItemTitle;
            if (Utils.CurrentDemoPage.Group != null)
            {
                result += " - " + Utils.CurrentDemoPage.Group.Title;
            }
            return result;
        }

        public static void RegisterCssLink(Page page, string url)
        {
            RegisterCssLink(page, url, null);
        }

        public static string GetDescriptionTitle()
        {
            if (Utils.CurrentOverview != null && !string.IsNullOrEmpty(Utils.CurrentOverview.DescriptionTitle))
            {
                return Utils.CurrentOverview.DescriptionTitle;
            }
            return string.Format("About the {0}", Utils.CurrentDemoPage.Group == null ? Utils.CurrentDemo.Product.NavItemTitle : Utils.CurrentDemoPage.Group.Title);
        }

        public static void RegisterCssLink(Page page, string url, string clientId)
        {
            if (IsMvc)
            {
                throw new NotImplementedException();
            }
            var link = new HtmlLink();
            page.Header.Controls.Add(link);
            link.EnableViewState = false;
            link.Attributes["type"] = "text/css";
            link.Attributes["rel"] = "stylesheet";
            if (!string.IsNullOrEmpty(clientId))
            {
                link.Attributes["id"] = clientId;
            }
            link.Href = url;
        }

        public static string GenerateDemoUrl(DemoModel demo)
        {
            if (!string.IsNullOrEmpty(demo.HighlightedLink))
            {
                return demo.HighlightedLink;
            }
            var str = new StringBuilder();
            if (demo.Product.IsCurrent)
            {
                str.Append("~/");
            }
            else
            {
                var url = HttpContext.Current.Request.Url.AbsolutePath;
                var productUrl = "/" + CurrentDemo.Product.Url;
                url = url.Substring(0, url.IndexOf(productUrl, StringComparison.InvariantCultureIgnoreCase) + 1);
                str.AppendFormat("{0}{1}/", url, demo.Product.Url);
            }

            var demoGroup = demo is DemoPageModel ? ((DemoPageModel)demo).Group : null;
            if (demoGroup != null && !string.IsNullOrEmpty(demoGroup.Key))
            {
                str.Append(demoGroup.Key);
                str.Append("/");
            }
            if (!IsMvc || !string.Equals("Index", demo.Key))
            {
                str.Append(demo.Key);
            }
            if (!IsMvc)
            {
                str.Append(".aspx");
            }
            return str.ToString();
        }

        public static List<FeaturedDemoInfo> GenerateFeaturedDemos()
        {
            var result = new List<FeaturedDemoInfo>();
            foreach (var demo in DemosModel.Current.HighlightedDemos)
            {
                result.Add(new FeaturedDemoInfo {
                    Title = demo.GetHighlightedTitle(),
                    ImageUrl = demo.HighlightedImageUrl,
                    NavigateUrl = GenerateDemoUrl(demo),
                    Description = demo.HighlightedDescription
                });
            }
            if (Utils.CurrentIntro != null)
            {
                foreach (var demo in Utils.CurrentIntro.ExternalDemos)
                {
                    result.Add(new FeaturedDemoInfo {
                        Title = demo.Title,
                        ImageUrl = demo.ImageUrl,
                        NavigateUrl = demo.Url
                    });
                }
            }
            return result;
        }
        public static List<ProductInfo> GenerateProductDemos(bool highlited)
        {
            var result = new List<ProductInfo>();
            foreach (var item in DemosModel.Instance.SortedDemoProducts.Where(p => !p.HideNavItem && !p.IsRootDemo && p.IntegrationHighlighted == highlited))
            {
                result.Add(new ProductInfo() {
                    NavigateUrl = GenerateDemoUrl(item.Intro),
                    ImageUrl = item.IntegrationImageUrl,
                    Description = item.IntegrationDescription,
                    Title = item.NavItemTitle
                });
            }
            return result;
        }
        public static void EnsureRequestValidationMode()
        {
            try
            {
                if (Environment.Version.Major >= 4)
                {
                    var type = typeof(WebControl).Assembly.GetType("System.Web.Configuration.RuntimeConfig");
                    var getConfig = type.GetMethod("GetConfig", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { }, null);
                    var runtimeConfig = getConfig.Invoke(null, null);
                    var getSection = type.GetMethod("GetSection", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(string), typeof(Type) }, null);
                    var httpRuntimeSection = (HttpRuntimeSection)getSection.Invoke(runtimeConfig, new object[] { "system.web/httpRuntime", typeof(HttpRuntimeSection) });
                    var bReadOnly = typeof(ConfigurationElement).GetField("_bReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
                    bReadOnly.SetValue(httpRuntimeSection, false);

                    var pi = typeof(HttpRuntimeSection).GetProperty("RequestValidationMode");
                    if (pi != null)
                    {
                        var version = (Version)pi.GetValue(httpRuntimeSection, null);
                        var RequiredRequestValidationMode = new Version(2, 0);
                        if (version != null && !Version.Equals(version, RequiredRequestValidationMode))
                        {
                            pi.SetValue(httpRuntimeSection, RequiredRequestValidationMode, null);
                        }
                    }
                    bReadOnly.SetValue(httpRuntimeSection, true);
                }
            }
            catch
            {
            }
        }

        private static bool? _isSiteMode;
        public static bool IsSiteMode
        {
            get
            {
                if (!_isSiteMode.HasValue)
                {
                    _isSiteMode = ConfigurationManager.AppSettings["SiteMode"].Equals("true", StringComparison.InvariantCultureIgnoreCase);
                }
                return _isSiteMode.Value;
            }
        }

        public static string GetReadOnlyMessageHtml()
        {
            return String.Format(
                "<b>Data modifications are not allowed in the online demo.</b><br />" +
                "If you need to test data editing functionality, please install " +
                "{0} on your machine and run the demo locally.", DemosModel.Current.Title);
        }
        public static string GetReadOnlyMessageText()
        {
            return String.Format(
                "Data modifications are not allowed in the online demo.\n" +
                "If you need to test data editing functionality, please install \n" +
                "{0} on your machine and run the demo locally.", DemosModel.Current.Title);
        }

        public static void AssertNotReadOnly()
        {
            if (!IsSiteMode)
            {
                return;
            }
            throw new InvalidOperationException(GetReadOnlyMessageHtml());
        }
        public static void AssertNotReadOnlyText()
        {
            if (!IsSiteMode)
            {
                return;
            }
            throw new InvalidOperationException(GetReadOnlyMessageText());
        }
        public static bool CanChangeTheme
        {
            get
            {
                return !IsIntro && !IsIE6() && DemosModel.Current.SupportsTheming;
            }
        }

        public static void InjectDescriptionMeta(Control parent)
        {
            if (String.IsNullOrEmpty(Utils.CurrentDemo.MetaDescription))
            {
                return;
            }
            var page = parent as Page;
            var header = (page != null && page.Header != null) ? page.Header : RenderUtils.FindHead(parent);
            if (header != null)
            {
                var metaControl = new LiteralControl(string.Format("<meta name=\"description\" content=\"{0}\" />", Utils.CurrentDemo.MetaDescription));
                header.Controls.AddAt(0, metaControl);
            }
        }

        public static void InjectIE7CompatModeMeta(Control parent)
        {
            InjectIECompatModeMeta(parent, 7);
        }
        public static void InjectIEEdgeCompatModeMeta(Control parent)
        {
            if (RenderUtils.Browser.IsIE)
            {
                ASPxWebControl.SetIECompatibilityModeEdge(parent);
            }
        }

        public static void InjectIECompatModeMeta(Control parent, int compatibilityVersion)
        {
            if (!RenderUtils.Browser.IsIE || RenderUtils.Browser.Version >= 10  || RenderUtils.Browser.Version < compatibilityVersion + 1)
            {
                return;
            }
            ASPxWebControl.SetIECompatibilityMode(compatibilityVersion, parent);
        }

        public static bool IsIE6()
        {
            return RenderUtils.Browser.IsIE && RenderUtils.Browser.Version < 7;
        }

        public static bool IsIE9()
        {
            return RenderUtils.Browser.IsIE && RenderUtils.Browser.Version > 8;
        }

        public static bool IsIE10()
        {
            return RenderUtils.Browser.IsIE && RenderUtils.Browser.Version > 9;
        }
    }
    public static class SearchUtils
    {
        private static readonly string[] separators = new string[] { " ", ",", "/", "\\", "-", "+" };

        private static string[] _requestExclusions;
        private static string[] _prefixes;
        private static string[] _postfixes;
        private static string[][] _synonyms;

        private static string[] WordsExclusions
        {
            get
            {
                if (_requestExclusions == null)
                {
                    _requestExclusions = DemosModel.Instance.Search.Exclusions.Words.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                }
                return _requestExclusions;
            }
        }
        private static string[] PrefixesExclusions
        {
            get
            {
                if (_prefixes == null)
                {
                    _prefixes = DemosModel.Instance.Search.Exclusions.Prefixes.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                }
                return _prefixes;
            }
        }
        private static string[] PostfixesExclusions
        {
            get
            {
                if (_postfixes == null)
                {
                    _postfixes = DemosModel.Instance.Search.Exclusions.Postfixes.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                }
                return _postfixes;
            }
        }
        private static string[][] Synonyms
        {
            get
            {
                if (_synonyms == null)
                {
                    _synonyms = DemosModel.Instance.Search.Synonyms.Groups.Select(s => s.Split(separators, StringSplitOptions.RemoveEmptyEntries)).ToArray();
                }
                return _synonyms;
            }
        }

        public static List<SearchResult> DoSearch(string request)
        {
            var results = new List<SearchResult>();
            var requests = SplitRequests(request);
            try
            {
                var products = DemosModel.Instance.SortedDemoProducts.Where(dp => !dp.IsRootDemo && (dp == DemosModel.Current || (!dp.HideNavItem && Utils.IsSiteMode)));
                foreach (var product in products)
                {
                    results.AddRange(DoSearch(requests, product));
                }
            }
            catch
            {
            }
            results.Sort();
            return results;
        }
        public static Dictionary<string, int> GetKeywordsRankList(ModelBase model)
        {
            var textRanks = new List<TextRank>() { new TextRank(model.Keywords, 3)
            };

            var product = model as DemoProductModel;
            var group = model as DemoGroupModel;
            var demo = model as DemoPageModel;

            if (product != null)
            {
                textRanks.Add(new TextRank(product.NavItemTitle, 5));
                textRanks.Add(new TextRank(product.Key, 3));
                textRanks.Add(new TextRank(product.Title, 3));
                textRanks.Add(new TextRank(product.SeoTitle, 2));
            }
            else
            {
                if (group != null)
                {
                    textRanks.Add(new TextRank(group.Title, 5));
                    textRanks.Add(new TextRank(group.Key, 3));
                    textRanks.Add(new TextRank(group.SeoTitle, 2));
                }
                else
                {
                    if (demo != null)
                    {
                        textRanks.Add(new TextRank(demo.Title, 5));
                        textRanks.Add(new TextRank(demo.Key, 3));
                        textRanks.Add(new TextRank(demo.SeoTitle, 2));
                    }
                }
            }
            return GetKeywordsRankList(textRanks);
        }

        private static int CalculateRank(List<string[]> requests, DemoPageModel demo)
        {
            var resultRank = 0;
            var keywordRank = 0;
            foreach (var request in requests)
            {
                var requestRank = -1;
                if (CalculateRank(request, demo.KeywordsRankList, out keywordRank))
                {
                    requestRank += keywordRank;
                }
                if (CalculateRank(request, demo.Group.KeywordsRankList, out keywordRank))
                {
                    requestRank += keywordRank;
                }
                if (CalculateRank(request, demo.Product.KeywordsRankList, out keywordRank))
                {
                    requestRank += keywordRank;
                }
                if (requestRank == -1 && WordsExclusions.Any(re => re.Equals(request[0], StringComparison.InvariantCultureIgnoreCase)))
                {
                    requestRank = 0;
                }
                if (requestRank > -1)
                {
                    resultRank += requestRank;
                }
                else
                {
                    return -1;
                }
            }
            return resultRank;
        }
        private static bool CalculateRank(string[] synonyms, Dictionary<string, int> keywordsRankList, out int rank)
        {
            var keyword = keywordsRankList.Keys.FirstOrDefault(k => MatchWord(synonyms[0], k));
            rank = keyword != null ? keywordsRankList[keyword] : -1;
            if (rank == -1)
            {
                foreach (var syn in synonyms.Skip(1))
                {
                    keyword = keywordsRankList.Keys.FirstOrDefault(k => MatchWord(syn, k));
                    rank += keyword != null ? keywordsRankList[keyword] : -1;
                    if (rank > -1)
                    {
                        break;
                    }
                }
            }
            return rank > -1;
        }
        private static IEnumerable<SearchResult> DoSearch(List<string[]> requests, DemoProductModel product)
        {
            var results = new List<SearchResult>();
            foreach (var demo in product.Groups.SelectMany(g => g.Demos))
            {
                var rank = CalculateRank(requests, demo);
                if (rank > -1)
                {
                    var sr = new SearchResult(demo, rank);
                    sr.Text = string.Format("{0} ({1})", HighlightOccurences(demo.Title, requests), HighlightOccurences(demo.Group.Title, requests));
                    sr.ProductText = demo.Product.Title.ToUpper();
                    results.Add(sr);
                }
            }
            return results;
        }

        private static string HighlightOccurences(string text, List<string[]> requests)
        {
            var validRequest = new Regex("[0-9a-zA-Z]+", RegexOptions.IgnoreCase);
            foreach (var request in requests.SelectMany(r => r))
            {
                if (validRequest.IsMatch(request))
                {
                    var re = new Regex("([a-zA-Z0-9]*" + request + "[a-zA-Z0-9]*)", RegexOptions.IgnoreCase);
                    text = re.Replace(text, "<b>$0</b>");
                }
            }
            return text;
        }

        private static List<string[]> SplitRequests(string request)
        {
            var words = request.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            var result = new List<string[]>();
            foreach (var word in words)
            {
                var resultWord = PrepareWord(word);
                var synonymList = Synonyms.FirstOrDefault(list => list.Any(s => MatchWord(resultWord, s)));
                var wordSynonyms = new List<string>() { resultWord };
                if (synonymList != null)
                {
                    wordSynonyms.AddRange(synonymList.Where(s => !MatchWord(resultWord, s)));
                }
                result.Add(wordSynonyms.Distinct().ToArray());
            }
            return result;
        }
        private static string PrepareWord(string word)
        {
            foreach (var prefix in PrefixesExclusions)
            {
                if (word.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase) && word.Length > prefix.Length)
                {
                    word = word.Remove(0, prefix.Length);
                }
            }
            foreach (var postfix in PostfixesExclusions)
            {
                if (word.EndsWith(postfix, StringComparison.InvariantCultureIgnoreCase) && word.Length > postfix.Length)
                {
                    word = word.Substring(0, word.Length - postfix.Length);
                }
            }
            return word;
        }
        private static bool MatchWord(string request, string word)
        {
            return word.IndexOf(request, StringComparison.InvariantCultureIgnoreCase) > -1;
        }
        internal static string[] GetKeywordsList(params string[] words)
        {
            return words
                .SelectMany(w => w.Split(separators, StringSplitOptions.RemoveEmptyEntries))
                .Distinct()
                .ToArray();
        }

        private static Dictionary<string, int> GetKeywordsRankList(List<TextRank> textRanks)
        {
            var result = new Dictionary<string, int>();
            foreach (var textRank in textRanks)
            {
                var words = textRank.Text.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                foreach (var word in words)
                {
                    var rankWord = PrepareWord(word);
                    if (!result.ContainsKey(rankWord))
                    {
                        result[rankWord] = textRank.Rank;
                    }
                }
            }
            return result;
        }
    }


    public class TextRank
    {
        public TextRank(string text, int rank)
        {
            Text = text;
            Rank = rank;
        }
        public string Text { get; set; }
        public int Rank { get; set; }
    }
    public class SearchResult : IComparable<SearchResult>
    {
        public SearchResult(DemoModel demo, int rank)
        {
            Demo = demo;
            Rank = rank;
            Product = demo.Product;
            if (demo is DemoPageModel)
            {
                Group = (demo as DemoPageModel).Group;
            }
        }

        public DemoProductModel Product { get; set; }
        public DemoModel Demo { get; set; }
        public DemoGroupModel Group { get; set; }
        public string Text { get; set; }
        public string ProductText { get; set; }

        public int Rank = 0;



        public int CompareTo(SearchResult other)
        {
            return other.Rank.CompareTo(Rank);
        }
    }
}

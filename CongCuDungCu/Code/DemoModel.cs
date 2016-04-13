using System;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DevExpress.Web.ASPxTabControl;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxClasses.Internal;

namespace CongCuDungCu.Code
{
    public class DemoPageModel : DemoModel
    {
        private List<SeeAlsoLinkModel> _seeAlsoLinks = new List<SeeAlsoLinkModel>();
        private string _highlightedDescription = string.Empty;

        [XmlIgnore]
        public DemoGroupModel Group { get; internal set; }

        [XmlElement("SeeAlso")]
        public List<SeeAlsoLinkModel> SeeAlsoLinks
        {
            get
            {
                return _seeAlsoLinks;
            }
            set
            {
                _seeAlsoLinks = value;
            }
        }

        [XmlElement("HighlightedDescription")]
        public string HighlightedDescription
        {
            get
            {
                return _highlightedDescription;
            }
            set
            {
                _highlightedDescription = value;
            }
        }
    }


    public class DemoModel : DemoModelBase
    {
        private string _description;
        private string _metaDescription;
        private List<string> _sourceFiles = new List<string>();

        private int _highlightedIndex = -1;
        private string _highlightedImageUrl;
        private string _highlightedTitle;
        private bool _descriptionProcessed;

        [XmlIgnore]
        public DemoProductModel Product { get; internal set; }

        [XmlAttribute]
        public virtual bool HideSourceCode { get; set; }


        [XmlElement]
        public string Description
        {
            get
            {
                if (!_descriptionProcessed)
                {
                    _description = ProcessDescription(_description);
                    _descriptionProcessed = true;
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
        [XmlElement]
        public string MetaDescription
        {
            get
            {
                if (_metaDescription == null)
                {
                    return string.Empty;
                }
                return _metaDescription;
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                }
                _metaDescription = value;
            }
        }

        [XmlElement("SourceFile")]
        public List<string> SourceFiles
        {
            get
            {
                return _sourceFiles;
            }
        }

        [XmlAttribute]
        public int HighlightedIndex
        {
            get
            {
                return _highlightedIndex;
            }
            set
            {
                _highlightedIndex = value;
            }
        }

        [XmlAttribute]
        public string HighlightedImageUrl
        {
            get
            {
                if (_highlightedImageUrl == null)
                {
                    return string.Empty;
                }
                return _highlightedImageUrl;
            }
            set
            {
                _highlightedImageUrl = value;
            }
        }

        [XmlAttribute]
        public string HighlightedTitle
        {
            get
            {
                if (_highlightedTitle == null)
                {
                    return string.Empty;
                }
                return _highlightedTitle;
            }
            set
            {
                _highlightedTitle = value;
            }
        }

        [XmlAttribute]
        public string HighlightedLink { get; set; }

        [XmlAttribute]
        public bool IE7CompatModeRequired { get; set; }

        public string GetHighlightedTitle()
        {
            if (!String.IsNullOrEmpty(HighlightedTitle))
            {
                return HighlightedTitle;
            }
            return Title;
        }

        private static string ProcessDescription(string text)
        {
            if (text == null)
            {
                text = string.Empty;
            }
            text = Regex.Replace(text, @"<code\s+lang=([^>]+)>(.*?)</code>", DescriptionCodeReplacer, RegexOptions.Singleline);
            text = Regex.Replace(text, "<pageControl>(.*?)</pageControl>", DescriptionPageControlReplacer, RegexOptions.Singleline);
            text = Regex.Replace(text, "<helplink([^>]*)>(.*?)</helplink>", DescriptionHelpLinkReplacer, RegexOptions.Singleline);
            return text;
        }

        private static string DescriptionCodeReplacer(Match match)
        {
            var lang = match.Groups[1].Value.Trim('"', '\'');
            var code = match.Groups[2].Value;
            var result = "<code>" + CodeFormatter.GetFormattedCode(CodeFormatter.ParseLanguage(lang), code, Utils.IsMvc) + "<br /></code>";
            return Utils.IsOverview ? string.Format("<div class='{0}'>{1}</div>", "CodeBlock", result) : result;
        }
        private static string DescriptionHelpLinkReplacer(Match match)
        {
            var attributes = new Dictionary<string, string>();
            var reg = new Regex("(\\S+)=[\"']?((?:.(?![\"']?\\s+(?:\\S+)=|[>\"']))+.)[\"']?");
            var attrMatches = reg.Matches(match.Groups[1].Value);
            foreach (Match am in attrMatches)
            {
                attributes[am.Groups[1].Value] = am.Groups[2].Value;
            }
            if (!attributes.ContainsKey("href"))
            {
                attributes["href"] = "http://help.devexpress.com/";
            }
            return string.Format("<a href=\"{0}\" class=\"{1}\">{2}</a>", attributes["href"], "helplink", match.Groups[2].Value);
        }
        private static string DescriptionPageControlReplacer(Match match)
        {
            var tabPages = Regex.Matches(match.Value, @"<tabPage\s+text=([^>]+)>(.*?)</tabPage>", RegexOptions.Singleline);
            if (tabPages.Count == 0)
            {
                return match.Groups[1].Value;
            }
            var pageControl = new ASPxPageControl();
            pageControl.ID = "OverviewPageControl";
            pageControl.CssClass = "DemoPageControl";
            pageControl.EnableTabScrolling = true;
            pageControl.TabAlign = TabAlign.Justify;
            pageControl.EnableTheming = false;
            pageControl.ActiveTabIndex = 0;
            pageControl.EnableViewState = false;
            pageControl.Width = Unit.Percentage(100);
            foreach (Match tabPage in tabPages)
            {
                var text = tabPage.Groups[1].Value.Trim('"', '\'');
                var tab = pageControl.TabPages.Add(text, text);
                tab.Controls.Add(RenderUtils.CreateLiteralControl(tabPage.Groups[2].Value));
            }
            return RenderUtils.GetRenderResult(pageControl);
        }

        public string GetSeoTitle()
        {
            if (!String.IsNullOrEmpty(SeoTitle))
            {
                return SeoTitle;
            }
            return Title;
        }
    }

    public class SeeAlsoLinkModel
    {
        [XmlAttribute]
        public string Url { get; set; }
        [XmlAttribute]
        public string Title { get; set; }
    }
}

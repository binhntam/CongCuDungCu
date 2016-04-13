using System;
using System.Collections.Generic;
using System.Web;
using DevExpress.CodeParser;
using DevExpress.CodeParser.CSharp;
using DevExpress.CodeParser.Html;
using DevExpress.CodeParser.VB;
using DevExpress.CodeParser.JavaScript;
using DevExpress.CodeParser.Css;
using System.Text;

namespace CongCuDungCu.Code
{
    public static class CodeFormatter
    {
        private static Dictionary<TokenCategory, TokenCategoryClassProvider> _cssClasses = new Dictionary<TokenCategory, TokenCategoryClassProvider>();

        static CodeFormatter()
        {
            CssClasses.Add(TokenCategory.Text, new TokenCategoryClassProvider("cr-text", new KeyValuePair<TokenLanguage, string>(TokenLanguage.Html, "cr-text-html")));
            CssClasses.Add(TokenCategory.Keyword, new TokenCategoryClassProvider("cr-keyword", new KeyValuePair<TokenLanguage, string>(TokenLanguage.Html, "cr-keyword-html"), new KeyValuePair<TokenLanguage, string>(TokenLanguage.Css, "cr-keyword-css")));
            CssClasses.Add(TokenCategory.Operator, new TokenCategoryClassProvider("cr-operator"));
            CssClasses.Add(TokenCategory.PreprocessorKeyword, new TokenCategoryClassProvider("cr-preproc", new KeyValuePair<TokenLanguage, string>(TokenLanguage.Html, "cr-preproc-html")));
            CssClasses.Add(TokenCategory.String, new TokenCategoryClassProvider("cr-string", new KeyValuePair<TokenLanguage, string>(TokenLanguage.Html, "cr-string-html"), new KeyValuePair<TokenLanguage, string>(TokenLanguage.Css, "cr-string-css")));
            CssClasses.Add(TokenCategory.Number, new TokenCategoryClassProvider("cr-number"));
            CssClasses.Add(TokenCategory.Identifier, new TokenCategoryClassProvider("cr-identifier"));
            CssClasses.Add(TokenCategory.HtmlServerSideScript, new TokenCategoryClassProvider("cr-htmlserverscript"));
            CssClasses.Add(TokenCategory.HtmlString, new TokenCategoryClassProvider("cr-htmlstring"));
            CssClasses.Add(TokenCategory.Unknown, new TokenCategoryClassProvider("cr-unknown"));
            CssClasses.Add(TokenCategory.Comment, new TokenCategoryClassProvider("cr-comment"));
            CssClasses.Add(TokenCategory.XmlComment, new TokenCategoryClassProvider("cr-xmlcomment"));
            CssClasses.Add(TokenCategory.CssComment, new TokenCategoryClassProvider("cr-csscomment"));
            CssClasses.Add(TokenCategory.CssKeyword, new TokenCategoryClassProvider("cr-csskeyword"));
            CssClasses.Add(TokenCategory.CssPropertyName, new TokenCategoryClassProvider("cr-csspropertyname"));
            CssClasses.Add(TokenCategory.CssPropertyValue, new TokenCategoryClassProvider("cr-csspropertyvalue"));
            CssClasses.Add(TokenCategory.CssSelector, new TokenCategoryClassProvider("cr-cssselector"));
            CssClasses.Add(TokenCategory.CssStringValue, new TokenCategoryClassProvider("cr-cssstringvalue"));
            CssClasses.Add(TokenCategory.HtmlElementName, new TokenCategoryClassProvider("cr-htmlelementname"));
            CssClasses.Add(TokenCategory.HtmlEntity, new TokenCategoryClassProvider("cr-htmlentity"));
            CssClasses.Add(TokenCategory.HtmlOperator, new TokenCategoryClassProvider("cr-htmloperator"));
            CssClasses.Add(TokenCategory.HtmlComment, new TokenCategoryClassProvider("cr-htmlcomment"));
            CssClasses.Add(TokenCategory.HtmlAttributeName, new TokenCategoryClassProvider("cr-htmlattributename"));
            CssClasses.Add(TokenCategory.HtmlAttributeValue, new TokenCategoryClassProvider("cr-htmlattributevalue"));
            CssClasses.Add(TokenCategory.HtmlTagDelimiter, new TokenCategoryClassProvider("cr-htmltagdelimiter"));
        }

        public static Dictionary<TokenCategory, TokenCategoryClassProvider> CssClasses
        {
            get
            {
                return _cssClasses;
            }
        }

        public static TokenLanguage ParseLanguage(string lang)
        {
            return (TokenLanguage)Enum.Parse(typeof(TokenLanguage), lang, true);
        }

        public static TokenLanguage GetLanguageByFileExtension(string extension)
        {
            switch (extension.ToLower())
            {
                case ".cs":
                    return TokenLanguage.CSharp;
                case ".vb":
                    return TokenLanguage.Basic;
                case ".html":
                case ".htm":
                case ".aspx":
                case ".ascx":
                case ".master":
                case ".cshtml":
                    return TokenLanguage.Html;
                case ".js":
                    return TokenLanguage.JavaScript;
                case ".xml":
                    return TokenLanguage.Xml;
                case ".css":
                    return TokenLanguage.Css;
                default:
                    return TokenLanguage.Unknown;
            }
        }

        public static string GetFormattedCode(string fileExtension, string code)
        {
            return GetFormattedCode(GetLanguageByFileExtension(fileExtension), code, false);
        }
        public static string GetFormattedCode(string fileExtension, string code, bool isMvc)
        {
            return GetFormattedCode(GetLanguageByFileExtension(fileExtension), code, isMvc);
        }
        public static string GetFormattedCode(TokenLanguage language, string code)
        {
            return GetFormattedCode(language, code, false);
        }
        public static string GetFormattedCode(TokenLanguage language, string code, bool isMvc)
        {
            var tokens = GetTokens(language, code, isMvc);
            if (tokens != null)
            {
                return GetFormattedCode(code, tokens);
            }
            return string.Empty;
        }

        private class CodeLine
        {
            public int Indent;
            public string Html = string.Empty;

            public bool IsEmpty
            {
                get
                {
                    return Html.Trim().Length < 1;
                }
            }
        }

        private static string GetFormattedCode(string code, TokenCollection tokens)
        {
            var currentLine = new CodeLine();
            var lines = new List<CodeLine>();
            var pos = 0;
            foreach (CategorizedToken token in tokens)
            {
                AppendCode(lines, ref currentLine, code.Substring(pos, token.StartPosition - pos), null);
                AppendCode(lines, ref currentLine, token.Value, CssClasses[token.Category].GetClassName(token.Language));
                pos = token.EndPosition;
            }
            AppendCode(lines, ref currentLine, code.Substring(pos), null);
            lines.Add(currentLine);
            return MergeCodeLines(lines);
        }
        private static void AppendCode(List<CodeLine> lines, ref CodeLine currentLine, string code, string cssClass)
        {
            var hasCss = !String.IsNullOrEmpty(cssClass);
            var first = true;
            code = code.Replace("\r", string.Empty).Replace("\t", "    ");
            foreach (string line in code.Split('\n'))
            {
                var text = line;
                if (!first)
                {
                    lines.Add(currentLine);
                    currentLine = new CodeLine();
                    text = text.TrimStart();
                    currentLine.Indent = line.Length - text.Length;
                }
                if (first || text.Trim().Length > 0)
                {
                    if (hasCss)
                    {
                        currentLine.Html += String.Format("<span class=\"{0}\">", cssClass);
                    }
                    currentLine.Html += HttpUtility.HtmlEncode(text);
                    if (hasCss)
                    {
                        currentLine.Html += "</span>";
                    }
                }
                first = false;
            }
        }
        private static string MergeCodeLines(List<CodeLine> lines)
        {
            var minIndent = int.MaxValue;
            foreach (CodeLine line in lines)
            {
                if (line.IsEmpty)
                {
                    continue;
                }
                if (line.Indent < minIndent)
                {
                    minIndent = line.Indent;
                }
            }

            var result = new StringBuilder();
            var emptyLineCount = 0;

            foreach (CodeLine line in lines)
            {
                if (line.IsEmpty)
                {
                    if (result.Length > 0)
                    {
                        emptyLineCount++;
                    }
                    continue;
                }
                if (emptyLineCount > 0)
                {
                    for (var i = 0; i < emptyLineCount; i++)
                    {
                        result.Append("<br />");
                    }
                    emptyLineCount = 0;
                }
                var indent = line.Indent - minIndent;
                for (var i = 0; i < indent; i++)
                {
                    result.Append("&nbsp;");
                }
                result.Append(line.Html);
                if (result.Length > 0)
                {
                    result.Append("<br />");
                }
            }

            return result.ToString().Trim();
        }

        private static TokenCollection GetTokens(TokenLanguage language, string code, bool isMvc)
        {
            switch (language)
            {
                case TokenLanguage.CSharp:
                    return CSharpTokensHelper.GetTokens(code);
                case TokenLanguage.Basic:
                    return VBTokensHelper.GetTokens(code);
                case TokenLanguage.JavaScript:
                    return JavaScriptTokensHelper.GetTokens(code);
                case TokenLanguage.Html:
                    if (!isMvc)
                    {
                        return HtmlTokensHelper.GetTokens(code);
                    }
                    else
                    {
                        return HtmlTokensHelper.GetTokens(code, LanguageKind.Razor, DotNetLanguageType.CSharp);
                    }
                case TokenLanguage.Xml:
                    return new XmlTokensCategoryHelper().GetTokens(code);
                case TokenLanguage.Css:
                    return new CssTokensCategoryHelper().GetTokens(code);
                default:
                    return null;
            }
        }
    }
    public class TokenCategoryClassProvider
    {
        private string className;
        private Dictionary<TokenLanguage, string> languagesClassNames = new Dictionary<TokenLanguage, string>();

        public TokenCategoryClassProvider(string className)
            : this(className, null)
        {
        }
        public TokenCategoryClassProvider(string className, params KeyValuePair<TokenLanguage, string>[] languagesClassNames)
        {
            this.className = className;
            if (languagesClassNames != null)
            {
                foreach (KeyValuePair<TokenLanguage, string> languageClassName in languagesClassNames)
                {
                    this.languagesClassNames[languageClassName.Key] = languageClassName.Value;
                }
            }
        }
        public string GetClassName(TokenLanguage language)
        {
            if (languagesClassNames.ContainsKey(language))
            {
                return languagesClassNames[language];
            }
            return className;
        }
    }
}

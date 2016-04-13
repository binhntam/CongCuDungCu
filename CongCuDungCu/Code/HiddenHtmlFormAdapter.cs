using System;
using System.Web.UI;
using System.Web.UI.Adapters;

namespace CongCuDungCu.Code
{
    public class HiddenHtmlFormAdapter : ControlAdapter
    {
        protected override void OnInit(EventArgs e)
        {
        }

        protected override void OnPreRender(EventArgs e)
        {
        }

        protected override void Render(HtmlTextWriter writer)
        {
            foreach (Control child in Control.Controls)
            {
                child.RenderControl(writer);
            }
        }
    }
}

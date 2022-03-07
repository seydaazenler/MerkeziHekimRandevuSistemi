using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MHRSLite_UI.CustomTagHalper
{
    [HtmlTargetElement("myemail")]
    
    public class MailTagHelper:TagHelper
    {
        //a etiketine bir email verilecek
        //Tıklandığında mail açılacak
        public string MailTo { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a";
            var mailTo = MailTo;
            output.Attributes.SetAttribute("href", "MailTo:" + mailTo);
        }
    }
}

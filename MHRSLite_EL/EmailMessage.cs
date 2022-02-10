using System;
using System.Collections.Generic;
using System.Text;

namespace MHRSLite_EL
{
    public class EmailMessage
    {
        //contact
        public string[] Contacts { get; set; }
        public string[] CC { get; set; }
        public string[] BCC { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

    }
}

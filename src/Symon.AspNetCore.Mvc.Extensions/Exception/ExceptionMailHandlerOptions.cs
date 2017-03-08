using System.Collections.Generic;
using zxm.MailKit.Abstractions;

namespace Symon.AspNetCore.Mvc.Extensions.Exception
{
    public class ExceptionMailHandlerOptions
    {
        public string MailSubject { get; set; }

        public IEnumerable<MailAddress> MailTos { get; set; }
    }
}

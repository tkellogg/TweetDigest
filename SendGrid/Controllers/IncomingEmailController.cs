using System.Web.Http;
using TweetSharp;

namespace SendGrid.Controllers
{
    public class IncomingEmailController : ApiController
    {
        public string Get()
        {
            return "GET";
        }

        public string Post()
        {
            return "POST";
        }

    }
}
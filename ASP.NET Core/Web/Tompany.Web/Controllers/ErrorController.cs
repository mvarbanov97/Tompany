using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tompany.Web.Controllers
{
    public class ErrorController : Controller
    {
        [Route("/Error/{statusCode}")]
        public IActionResult HttpInvalidStatusCodeHandler(int statusCode)
        {
            return this.View("NotFound");
        }
    }
}

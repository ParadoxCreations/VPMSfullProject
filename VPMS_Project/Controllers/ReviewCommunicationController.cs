using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VPMS_Project.Controllers
{
    public class ReviewCommunicationController : Controller
    {
        public IActionResult Communication()
        {
            return View();
        }
    }
}

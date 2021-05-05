using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VPMS_Project.Controllers
{
    public class ReviewDashController : Controller
    {
        public IActionResult ReviewDash()
        {
            return View();
        }
    }
}

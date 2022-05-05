using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace akademikArama.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string ePosta, string password)
        {
            /** veritabanı kontrol
            * sonra yonlendirme 
            * adminse admin sayfasına  
            * 
            **/
            return View();
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using akademikArama.Models;
namespace akademikArama.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult EklemeSayfasi(int? id)
        {
            List<EklemeSayfasiModel> modelList = new List<EklemeSayfasiModel>();

            return View(modelList);
        }
    }
}
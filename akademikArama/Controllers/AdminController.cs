using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neo4j.Driver;
using akademikArama.Models;
namespace akademikArama.Controllers
{
    public class AdminController : Controller
    {
        //GET
        [HttpGet]
        public ActionResult EklemeSayfasi(int? id)
        {
            List<EklemeSayfasiModel> modelList = new List<EklemeSayfasiModel>();

            return View(modelList);
        }
        [HttpPost]
        public ActionResult EklemeSayfasi(string a, string b)
        {
            List<EklemeSayfasiModel> modelList = new List<EklemeSayfasiModel>();



            return View(modelList);
        }

    }
}
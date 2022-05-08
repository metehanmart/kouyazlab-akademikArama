using akademikArama.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neo4j.Driver;
using akademikArama.Services;
#nullable enable
namespace akademikArama.Controllers
{
    public class KullaniciController : Controller
    {
        // GET
        [HttpGet]
        public ActionResult AramaSayfasi(int? id)
        {
            List<AramaSayfasiModel> modelList = new List<AramaSayfasiModel>();

            return View(modelList);
        }

        //POST
        [HttpPost]
        public ActionResult AramaSayfasi(string? AranacakArastirmaci, string? AranacakEser)
        {
            //arama işlemleri
            List<AramaSayfasiModel> modelList = new List<AramaSayfasiModel>();
            Neo4jDriverHelper helper = new Neo4jDriverHelper(MyConstants.Uri, MyConstants.UserName, MyConstants.Password);
            if (AranacakArastirmaci != null || AranacakArastirmaci != "")
            {
                List<ArastirmaciModel> arastirmaciList = helper.FindArastirmaci(AranacakArastirmaci);
                foreach (var i in arastirmaciList)
                {
                    AramaSayfasiModel model = new AramaSayfasiModel();
                    model.ArastirmaciID = i.ArastirmaciID;
                    model.ArastirmaciAdi = i.ArastirmaciAdi;
                    model.ArasirmaciSoyadi = i.ArasirmaciSoyadi;
                    modelList.Add(model);
                }
                return View(modelList);
            }

            //else if (AranacakEser != null || AranacakEser != "")
            //    helper.FindArastirmaci(AranacakArastirmaci);
            return View();
        }

        public ActionResult GrafikArastirmaci()
        {
            return View();
        }
    }
}
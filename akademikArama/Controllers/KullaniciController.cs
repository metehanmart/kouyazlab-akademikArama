using akademikArama.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using akademikArama.Services;
#nullable enable
namespace akademikArama.Controllers
{
    public class KullaniciController : Controller
    {
        // GET
        [HttpGet]
        public ActionResult AramaSayfasi(string? ArastirmaciAdi, string? ArastirmaciSoyadi)
        {
            List<AramaSayfasiModel> modelList = new List<AramaSayfasiModel>();
            Neo4jDriverHelper helper = new Neo4jDriverHelper(MyConstants.Uri, MyConstants.UserName, MyConstants.Password);

            List<AramaSayfasiModel> arastirmaciList = helper.FindArastirmaci(ArastirmaciAdi, ArastirmaciSoyadi, "", 0);
            foreach (var i in arastirmaciList)
            {
                AramaSayfasiModel model = new AramaSayfasiModel();
                model.ArastirmaciID = i.ArastirmaciID;
                model.ArastirmaciAdi = i.ArastirmaciAdi;
                model.ArasirmaciSoyadi = i.ArasirmaciSoyadi;
                model.YayinAdi = i.YayinAdi;
                model.YayinYili = i.YayinYili;
                model.YayinTuru = i.YayinTuru;
                model.CalistigiKisiler = i.CalistigiKisiler;
                modelList.Add(model);
            }
            return View(modelList);
        }

        //POST
        [HttpPost]
        public ActionResult AramaSayfasi(string? ArastirmaciAdi, string? ArastirmaciSoyadi, string? AranacakEser, int? YayinYili)
        {
            //arama işlemleri
            List<AramaSayfasiModel> modelList = new List<AramaSayfasiModel>();
            Neo4jDriverHelper helper = new Neo4jDriverHelper(MyConstants.Uri, MyConstants.UserName, MyConstants.Password);

            List<AramaSayfasiModel> arastirmaciList = helper.FindArastirmaci(ArastirmaciAdi, ArastirmaciSoyadi, AranacakEser, YayinYili);
            foreach (var i in arastirmaciList)
            {
                AramaSayfasiModel model = new AramaSayfasiModel();
                model.ArastirmaciID = i.ArastirmaciID;
                model.ArastirmaciAdi = i.ArastirmaciAdi;
                model.ArasirmaciSoyadi = i.ArasirmaciSoyadi;
                model.YayinAdi = i.YayinAdi;
                model.YayinYili = i.YayinYili;
                model.YayinTuru = i.YayinTuru;
                model.CalistigiKisiler = i.CalistigiKisiler;
                modelList.Add(model);
            }
            return View(modelList);

        }

        public ActionResult GrafikArastirmaci()
        {
            return View();
        }

    }
}
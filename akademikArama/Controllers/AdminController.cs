using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neo4j.Driver;
using akademikArama.Models;
using akademikArama.Services;


namespace akademikArama.Controllers
{
    public class AdminController : Controller
    {

        //GET
        [HttpGet]
        public ActionResult EklemeSayfasi(int? id)
        {
            List<EklemeSayfasiModel> modelList = new List<EklemeSayfasiModel>();
            Neo4jDriverHelper helper = new Neo4jDriverHelper(MyConstants.Uri, MyConstants.UserName, MyConstants.Password);
            modelList = helper.HepsiniGetir();

            return View(modelList);
        }
        [HttpPost]
        public ActionResult EklemeSayfasi(EklemeSayfasiModel eklemeSayfasiModel)
        {
            List<EklemeSayfasiModel> modelList = new List<EklemeSayfasiModel>();
            Neo4jDriverHelper neo4JDriverHelper = new Neo4jDriverHelper(MyConstants.Uri, MyConstants.UserName, MyConstants.Password);

            //Sadece Arastirmaci Eklenmesi
            //Arastirmaci ve Eser Eklenmesi
            //Eser sahipsiz eklenemez
            /**
             * Ilk önce daha önce o arastirmacidan eklenmis mi diye kontrol et varsa admine bildir
             * Eser zaten arastirmaci ile iliskiliyse bir şey yapma
             * **/
            bool status = true;
            if (eklemeSayfasiModel.ArastirmaciAdi != "" && eklemeSayfasiModel.ArasirmaciSoyadi != "" && eklemeSayfasiModel.ArastirmaciID != 0)
            {
                status = neo4JDriverHelper.ArastirmaciEkleme(eklemeSayfasiModel);
                if (!status)
                {
                    ViewBag.Message = "Lutfen ArastirmaciID degerinin diger ArastirmaciID'lerden farklı oldugundan emin olun";
                }

            }
            else if (eklemeSayfasiModel.ArastirmaciAdi != "" && eklemeSayfasiModel.ArasirmaciSoyadi != "" && eklemeSayfasiModel.YayinAdi != "" && eklemeSayfasiModel.YayinYili != 0 && eklemeSayfasiModel.YayinTuru != "")
            {
                neo4JDriverHelper.EserleArastirmaciyiBagla(eklemeSayfasiModel);
            }
            else
            {
                //napsak ki
                System.Diagnostics.Debug.WriteLine("admincontroller eklemesayfasi actionresulttaki else dustum");
            }
            modelList = neo4JDriverHelper.HepsiniGetir();
            return View(modelList);
        }

    }
}
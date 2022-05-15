using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neo4j.Driver;
using akademikArama.Models;
using akademikArama.Services;
using System.Web.Security;

namespace akademikArama.Controllers
{
    public class AdminController : Controller
    {

        //GET
        [Authorize]
        [HttpGet]
        public ActionResult EklemeSayfasi()
        {
            System.Diagnostics.Debug.WriteLine("ekleme bas");
            List<EklemeSayfasiModel> modelList = new List<EklemeSayfasiModel>();
            Neo4jDriverHelper helper = new Neo4jDriverHelper(MyConstants.Uri, MyConstants.UserName, MyConstants.Password);
            modelList = helper.HepsiniGetir();
            List<EklemeSayfasiModel> tmplist = helper.EserleriGetir();
            modelList.AddRange(tmplist);
            System.Diagnostics.Debug.WriteLine("ekleme son");
            return View(modelList);
        }
        [Authorize]
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
            if (eklemeSayfasiModel.ArastirmaciID != 0 && eklemeSayfasiModel.YayinID == 0 && eklemeSayfasiModel.YayinTuruID == 0)
            {
                status = neo4JDriverHelper.ArastirmaciEkleme(eklemeSayfasiModel);
                if (!status)
                {
                    ViewBag.Message = "Lutfen ArastirmaciID degerinin diger ArastirmaciID'lerden farklı oldugundan emin olun";
                }

            }
            else if (eklemeSayfasiModel.ArastirmaciID != 0 && eklemeSayfasiModel.YayinID != 0 && eklemeSayfasiModel.YayinTuruID != 0)
            {
                neo4JDriverHelper.EserleArastirmaciyiBagla(eklemeSayfasiModel);
            }
            else
            {
                //napsak ki
                System.Diagnostics.Debug.WriteLine("admincontroller eklemesayfasi actionresulttaki else dustum");
            }
            modelList = neo4JDriverHelper.HepsiniGetir();
            modelList.AddRange(neo4JDriverHelper.EserleriGetir());
            return View(modelList);
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string EMail, string password)
        {
            /** veritabanı kontrol
            * sonra yonlendirme 
            * adminse admin sayfasına  
            * 
            **/
            const string AdminEmail = "proje3@gmail.com";
            const string Password = "1234";
            System.Diagnostics.Debug.WriteLine("password = " + password);
            System.Diagnostics.Debug.WriteLine("password = " + EMail);
            System.Diagnostics.Debug.WriteLine("password = " + Password);
            System.Diagnostics.Debug.WriteLine("password = " + AdminEmail);
            if (EMail == AdminEmail && Password == password)
            {
                FormsAuthentication.SetAuthCookie(AdminEmail, false);
                return RedirectToAction("EklemeSayfasi", "Admin");
            }

            else
            {
                ViewBag.Yanlis = "Hatalı giriş lütfen e-posta adresininizi ve şifrenizi doğru girdiğinizden emin olun";
                return View();
            }

        }

    }
}
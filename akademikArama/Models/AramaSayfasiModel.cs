using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace akademikArama.Models
{
    public class AramaSayfasiModel
    {
        public int ArastirmaciID { get; set; }
        public string ArastirmaciAdi { get; set; }
        public string ArasirmaciSoyadi { get; set; }
        public string YayinAdi { get; set; }
        public int YayinYili { get; set; }
        public string YayinTuru { get; set; }
    }
}
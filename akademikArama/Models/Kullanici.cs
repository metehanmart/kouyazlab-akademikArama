using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace akademikArama.Models
{
    public class Kullanici
    {
        public int Id { get; set; }
        public string EMail { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
    }
}
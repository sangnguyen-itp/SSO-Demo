using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SSO.Models
{
    public class Session
    {
        [Key]
        public int ID { get; set; }
        public string SessionID { get; set; }
        public int AccountID { get; set; }
    }
}

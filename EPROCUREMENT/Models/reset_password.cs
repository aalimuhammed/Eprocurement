using System;
using System.ComponentModel.DataAnnotations;

namespace EPROCUREMENT.Models
{
    public class reset_password
    {
        [Key]
        public int user_id { get; set; }

        public string code { get; set; }
    }
}

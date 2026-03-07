using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.ComponentModel.DataAnnotations;

namespace EPROCUREMENT.ViewModel
{
    public class user_header_vm
    {
        [Key]
        public int id { get; set; }

        public string FullName { get; set; }

        public string email { get; set; }

        public string username { get; set; }

        public string company { get; set; }

        public DateTime createdTime { get; set; }

        public string ISO { get; set; }

        public string Area { get; set; }

        public string keyperson_name { get; set; }

        public string keyperson_mail { get; set; }

        public string keyperson_phone { get; set; }

        public string moneybudget { get; set; }

        public string projectno { get; set; }

        public string engineersno { get; set; }

        public string taxid_document_file_path { get; set; }

        public string commercial_register_document_file_path { get; set; }

        public string prev_work_file_path {get; set;}

        public string income_tax_file_path { get; set; }

        public string electronice_invoice_file_path { get; set; }

        public string category { get; set; }

        public string equipment { get; set; }

	}
}

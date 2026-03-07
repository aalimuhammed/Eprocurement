using System;

namespace EPROCUREMENT.Models
{
	public class sap_users
	{

		public int id { get; set; }

		public string? fname { get; set; }

		public string? lname { get; set; }

		public string? username { get; set; }

		public string? password { get; set; }

		public string? address { get; set; }

		public bool? verifyied { get; set; }

		public string? phone { get; set; }

		public string? email { get; set; }

		public string? company { get; set; }

		public DateTime user_datetime { get; set; }

		public bool? refused { get; set; } = false;


		public string? taxid_document_file_path { get; set; }

		public string? commercial_register_document_file_path { get; set; }

        public string? prev_work_document_file_path { get; set; }

        public string? income_tax_document { get; set; }

        public string? electronic_invoice_document { get; set; }

        public bool? iso_verifyed { get; set; } = false;

		public int? areas_id { get; set; }


		public string? keyperson_name { get; set; }

		public string? keyperson_mail { get; set; }

		public string? keyperson_phone { get; set; }

		public string? moneybudget { get; set; }

		public string? projectno { get; set; }

		public string? engineersno { get; set; }

		public string? equipment { get; set; }

		public bool? uploaded_data { get; set; }
		public string? tax_id { get; set; }
		public string? sap_code { get; set; }

		//public string? prev_work_document_file_path { get; set; }

		public string? phone_two { get; set; }

		public string? city { get; set; }

		public string? postal_code { get; set; }

		public string? fax { get; set; }

		public string? CommentsSalesPerson { get; set; }

		public string? ExternalAddressNumberSale { get; set; }

		public string? SalesPersonEmail { get; set; }
	}
}

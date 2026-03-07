namespace EPROCUREMENT.Models
{
    public class VendorData
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string Vendor { get; set; }
        public string Telephone { get; set; }
        public string Mobile { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string CommentsSalesPerson { get; set; }
        public string ExternalAddressNumberSale { get; set; }
        public string SalesPersonEmail { get; set; }
        public string Industry { get; set; }
        public string BpType { get; set; }
        public bool refused { get; set; }
        public string[] selectedIndustry { get; set; }
        public int? area_id { get; set; }
        public string engineerno { get; set; }
        public string money { get; set; }
        public string equipment { get; set; }
        public string engineer_num { get; set; }
        public string password { get; set; }
        public bool iso { get; set; }
        public string? tax_file { get; set; }
        public string? prev_work_file { get; set; }
        public string? commercial_file { get; set; }
        public string? idcard { get; set; }
        public string? income_tax_file { get; set; }
        public string? electronic_invoice_file { get; set; }
        public bool uploaded_data { get; set; }
        public bool verifyed { get; set; }
    }
}

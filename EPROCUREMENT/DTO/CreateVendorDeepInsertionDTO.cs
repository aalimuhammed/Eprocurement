using System.Collections.Generic;

namespace EPROCUREMENT.DTO
{
    public class CreateVendorDeepInsertionDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Fax { get; set; }
        public string Tax_Id { get; set; }
        public string Telephone { get; set; }
        public string CommentsSalesPerson { get; set; }
        public string ExternalAddressNumberSale { get; set; }
        public string SalesPersonEmail { get; set; }
        public string BpType { get; set; }
        public string KeyPersonName { get; set; }
        public string KeyPersonEmail { get; set; }
        public string KeyPersonMobile { get; set; }

        public List<string> Industries { get; set; }
    }
}

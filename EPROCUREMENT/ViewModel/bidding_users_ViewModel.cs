using System;

namespace EPROCUREMENT.ViewModel
{
    public class bidding_users_ViewModel
    {
        public int Id { get; set; }
        public int userid { get; set; }
		public string FullName { get; set; }
        public string username { get; set; }
        public string Email { get; set; }
        public string project_name { get; set; }
        public string PackageName { get; set; }
		public string Company { get; set; }
		public DateTime createdTime { get; set; }
		public string file_name { get; set; }
        public string user_category { get; set; }
    }
}
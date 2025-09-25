namespace FuelmanagementApi.Models
{
    public class User
    {
        public int id { get; set; }
        public string issuer_name { get; set; }
        public string contact { get; set; }
        public int is_deleted { get; set; }
    }
}

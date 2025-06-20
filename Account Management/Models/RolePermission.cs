namespace Account_Management.Models
{
    public class RolePermission
    {
        public string RoleId { get; set; }
        public string ModuleName { get; set; }
        public bool CanView { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
    }
}

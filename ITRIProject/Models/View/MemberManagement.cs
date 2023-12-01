namespace ITRIProject.Models.View
{
    public class MemberManagement
    {
        public int MemberId { get; set; }

        public string Account { get; set; } = null!;

        public string? Name { get; set; }

        public string? Position { get; set; }

        public string? Addr { get; set; }

        public string? Tel { get; set; }

        public string? Email { get; set; }

        public string? CompanyName { get; set; }
        public string? UserType { get; set; }
        public DateTime? LoginDate { get; set; }
    }
}

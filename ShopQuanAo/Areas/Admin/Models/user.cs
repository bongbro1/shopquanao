namespace ShopQuanAo.Areas.Admin.Models
{
	public class user
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public string Role { get; set; }
		public string? Phone { get; set; }
		public string? Address { get; set; }
		public string? ResetPassword { get; set; }
		public string? VerifyAccount { get; set; }
	}
}

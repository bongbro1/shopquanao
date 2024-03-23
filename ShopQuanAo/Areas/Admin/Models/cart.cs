namespace ShopQuanAo.Areas.Admin.Models
{
	public class cart
	{
		public int Id { get; set; }
		public int ProductId { get; set; }
		public int UserId { get; set; }
		public int Quantity { get; set; }
		public string SizeOrder { get; set; }
		public string ColorOrder { get; set; }
		public double? Total { get; set; }
		public bool? Status { get; set; }
	}
}

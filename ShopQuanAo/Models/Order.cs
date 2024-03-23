namespace ShopQuanAo.Models
{
	public class Order
	{
		public int Id { get; set; }	
		public int User_Id { get; set; }
		public double totalPrice { get; set; }
		public DateTime order_date { get; set;}
		public DateTime? order_approval {  get; set; }
		public string Country_address { get; set; }
		public string State_address { get; set; }
		public string City_Address { get; set; }
		public string? Specific_address { get; set; }
		public string? Fullname { get; set; }
		public string? Phone { get; set; }
		public bool? Status { get; set; }
	}
}

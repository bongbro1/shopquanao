namespace ShopQuanAo.Areas.Admin.Models
{
  public class blog
  {
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? Image {  get; set; }
    public int? CreateBy { get; set; }
    public DateTime? CreateDate { get; set; }
    public string? CategoryRelated { get; set; }
    public string? Tags { get; set; }
  }
}

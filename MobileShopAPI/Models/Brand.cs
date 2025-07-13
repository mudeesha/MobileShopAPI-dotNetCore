namespace MobileShopAPI.Models
{
    public class Brand
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public ICollection<Model> Models { get; set; } = new List<Model>();
    }


}

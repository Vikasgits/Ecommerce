namespace E_commerce.DTOS
{
    public class BulkUploadDto
    {
        public int ProductId { get; set; }   // Assuming there is an Id
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public double Price { get; set; }

        // **New Field**
        public string FilePath { get; set; }
    }
}

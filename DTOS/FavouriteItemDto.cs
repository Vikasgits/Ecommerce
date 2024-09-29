using AutoMapper;

namespace E_commerce.DTOS
{
    public class FavouriteItemDto
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
    }
}

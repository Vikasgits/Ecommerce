using E_commerce.Models;

namespace E_commerce.Services
{
    public interface IFavouriteItemService
    {
        public List<FavouriteItem> Get();
        bool AddFavouriteItem(FavouriteItem favourite);
        bool RemoveFavouriteItem( Guid UserId,Guid productId);
        List<Product> GetFavouriteItems(Guid userId);
    }
}

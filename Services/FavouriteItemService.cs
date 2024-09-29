using E_commerce.Exceptions;
using E_commerce.Models;
using E_commerce.Repository;

namespace E_commerce.Services
{
    public class FavouriteItemService : IFavouriteItemService
    {
        private readonly IEntityRepository<FavouriteItem> _favouriteItemRepository;
        private readonly IProductService _productService;
        private readonly IUserService _userService;
        public FavouriteItemService(IEntityRepository<FavouriteItem> favouriteItemRepository, IProductService productService, IUserService userService) { 
            _favouriteItemRepository = favouriteItemRepository;
            _productService = productService;
            _userService = userService;
        }
        public bool AddFavouriteItem(FavouriteItem favourite)
        {
            var user = _userService.GetById(favourite.UserId);
            var product = _productService.GetById(favourite.ProductId);
            return _favouriteItemRepository.Add(favourite);
            
               
        }

        public List<Product> GetFavouriteItems(Guid userId)
        {
           var favList= _favouriteItemRepository.Get().Where(fav=>fav.UserId == userId).ToList(); 
            var productIds = favList.Select(fav => fav.ProductId).ToList();
            var productList = _productService.GetAll()
                            .Where(prod => productIds.Contains(prod.ProductId))
                            .ToList();

            return productList;
        }

        public bool RemoveFavouriteItem(Guid userId, Guid productId)
        {
            var getFavourite = _favouriteItemRepository.Get().FirstOrDefault(fav => fav.UserId == userId && fav.ProductId == productId);
            if (getFavourite == null) throw new FavouriteItemNotFoundException("No such favourite item exists");
            return _favouriteItemRepository.Delete(getFavourite);
        }

        public FavouriteItem GetById(Guid id) {

            var getFavourite = _favouriteItemRepository.Get().Where(fav => fav.ItemID == id).FirstOrDefault();
            if (getFavourite == null) throw new FavouriteItemNotFoundException("No such Favourite Item exist");
            return getFavourite;

        }

        public List<FavouriteItem> Get()
        {
            return _favouriteItemRepository.Get().ToList();
        }

    }
}

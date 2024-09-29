using AutoMapper;
using E_commerce.DTOS;
using E_commerce.Exceptions;
using E_commerce.Models;
using E_commerce.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavouriteItemController : ControllerBase
    {
        private readonly IFavouriteItemService _favouriteItemService;
        private readonly IMapper _mapper;

        public FavouriteItemController(IFavouriteItemService favouriteItemService, IMapper mapper)
        {
            _favouriteItemService = favouriteItemService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetFavouriteItems(Guid userId)
        {
            try
            {
                var products = _favouriteItemService.GetFavouriteItems(userId);
                return Ok(products);
            }
            catch (FavouriteItemNotFoundException unfe)
            {
                return NotFound(new { error = $"{unfe.Message},{unfe.StatusCode}" });
            }
            catch (UserNotFoundException unfe)
            {
                return NotFound(new { error = $"{unfe.Message},{unfe.StatusCode}" });
            }
            catch (InvalidUserException iue)
            {
                return BadRequest(new { error = $"{iue.Message},{iue.StatusCode}" });
            }
            catch (InvalidProductException ipe)
            {
                return BadRequest(new { error = $"{ipe.Message},{ipe.StatusCode}" });
            }
        }

        [HttpGet("GetFavourites")]
        public IActionResult GetAllFavourites() {
            var favList = _favouriteItemService.Get();
            return Ok(favList);
        }

        [HttpPost]
        public IActionResult AddFavouriteItem(FavouriteItemDto favouriteDto)
        {
            try
            {
                var favourite= _mapper.Map<FavouriteItem>(favouriteDto);
                _favouriteItemService.AddFavouriteItem(favourite);
                return CreatedAtAction(nameof(GetAllFavourites), new { id = favourite.ItemID}, new { message = $" Favourite Item added + {favourite.ItemID}" });

            }
            catch (FavouriteItemNotFoundException unfe)
            {
                return NotFound(new { error = $"{unfe.Message},{unfe.StatusCode}" });
            }
            catch (UserNotFoundException unfe)
            {
                return NotFound(new { error = $"{unfe.Message},{unfe.StatusCode}" });
            }
            catch (ProductNotFoundException pnfe)
            {
                return NotFound(new { error = $"{pnfe.Message},{pnfe.StatusCode}" });
            }
            catch (InvalidUserException iue)
            {
                return BadRequest(new { error = $"{iue.Message},{iue.StatusCode}" });
            }
            catch (InvalidProductException ipe)
            {
                return BadRequest(new { error = $"{ipe.Message},{ipe.StatusCode}" });
            }
        }

        [HttpDelete("{userId}/{productId}")]
        public IActionResult DeleteFavouriteItem(Guid userId, Guid productId)
        {
            try
            {
                _favouriteItemService.RemoveFavouriteItem(userId, productId);
                return Ok(new { message = "Favourite Item Deleted" });
            }
            catch (FavouriteItemNotFoundException pnfe)
            {
                return NotFound(new { error = $"{pnfe.Message},{pnfe.StatusCode}" });
            }
        }


    }
}

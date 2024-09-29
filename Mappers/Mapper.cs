using AutoMapper;
using E_commerce.DTOS;
using E_commerce.Models;

namespace E_commerce.Mappers
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<User, UserSendDto>();
            CreateMap<UserSendDto, User>();
            CreateMap<UserLoginDto, User>();
            CreateMap<User, UserLoginDto>();
            CreateMap<UserSignUpDto, User>();
            CreateMap<UserUpdateDto, User>();
            CreateMap<User,UserUpdateDto>();
            CreateMap<User,PostUserDto>();
            CreateMap<PostUserDto, User>();

            CreateMap<PostProductDto,Product>();
            CreateMap<Product, PostProductDto>();
            CreateMap<UpdateProductDto, Product>();
            CreateMap<Product, UpdateProductDto>();


            CreateMap<LineItemDto,LineItem>();
            CreateMap<LineItem,LineItemDto>();


            CreateMap<OrderDto,Order>();
            CreateMap<Order,OrderDto>();


            CreateMap<FavouriteItemDto,FavouriteItem>();
            CreateMap<FavouriteItem,FavouriteItemDto>();
           
        }
    }
}

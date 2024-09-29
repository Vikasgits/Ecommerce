using E_commerce.Models;

namespace E_commerce.Services
{
    public interface IOrderService
    {
        public List<Order> GetAll();
        public Order GetById(Guid id);
        public bool Add(Order order);
        public Order Update(Order order);

        public List<Order>GetAllOrdersOfUser(Guid userId);
        public Order GetOrders(Guid userId);
        public bool Delete(Guid id);
    }
}

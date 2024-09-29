using E_commerce.Exceptions;
using E_commerce.Models;
using E_commerce.Repository;

namespace E_commerce.Services
{
    public class OrderService : IOrderService
    {
        private readonly IEntityRepository<Order> _orderRepository;
        public OrderService(IEntityRepository<Order> orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public bool Add(Order order)
        {
            if (order == null) throw new InvalidOrderException("Invalid Order");
            return _orderRepository.Add(order);
        }

        public bool Delete(Guid id)
        {
            var getOrder= _orderRepository.Get().Where(order => order.OrderId==id).FirstOrDefault();
            return _orderRepository.Delete(getOrder);
            
        }

        public List<Order> GetAll()
        {
            var orderList= _orderRepository.Get().ToList();
            return orderList;
        }

        public Order GetById(Guid id)
        {
           var getOrder=_orderRepository.Get().Where(order=>order.OrderId == id ).FirstOrDefault(); 
            if (getOrder == null) throw new WishlistNotFoundException("No such order exist");
            return getOrder;
        }

        public List<Order> GetAllOrdersOfUser(Guid userId)
        {
            var getAllOrder = _orderRepository.Get().Where(order => order.UserId == userId && order.StatusId == 3).ToList();
            return getAllOrder;
        }

        public Order GetOrders(Guid userId)
        {
            var orderList = _orderRepository.Get().Where(order => order.UserId == userId  && order.StatusId==4).FirstOrDefault(); ///gets you the active order only with status 4
            return orderList;
        }



        public Order Update(Order order)
        {
            if (order == null) throw new InvalidOrderException("Invalid Order");
            var existOrder = _orderRepository.Get().Where(oder => order.OrderId == oder.OrderId).FirstOrDefault();
            
            existOrder.StatusId = order.StatusId;
            var updatedOrder=_orderRepository.Update(existOrder);
            return updatedOrder;
        }
    }
}

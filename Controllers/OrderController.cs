using AutoMapper;
using E_commerce.DTOS;
using E_commerce.Exceptions;
using E_commerce.Models;
using E_commerce.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO.Pipelines;

namespace E_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public OrderController(IOrderService orderService, IMapper mapper)
        {
            _mapper = mapper;
            _orderService = orderService;
        }

        [HttpGet]
        public IActionResult GetAllOrders()
        {
            var orderList = _orderService.GetAll();
            List<OrderDto> orderDtos = _mapper.Map<List<OrderDto>>(orderList);
            return Ok(orderDtos);
        }


        [HttpGet("{id:guid}")]
        public IActionResult GetOrder(Guid id)  ///to search a order
        {
            try
            {
                var getOrder = _orderService.GetById(id);
                OrderDto resultOrder = _mapper.Map<OrderDto>(getOrder);
                return Ok(resultOrder);
            }
            catch (WishlistNotFoundException onfe)
            {
                return NotFound(new { error = $"{onfe.Message},{onfe.StatusCode}" });
            }
            catch (InvalidOrderException ioe)
            {
                return BadRequest(new { error = $"{ioe.Message},{ioe.StatusCode}" });
            }
        }

        [HttpGet("UserId/{userId:guid}")] //get active orders
        public IActionResult GetOrders(Guid userId) {
            var orderList=_orderService.GetOrders(userId);
            return Ok(orderList);
        }

        [HttpGet("User/{userId:guid}")] //get all orders
        public IActionResult GetAllOrdersOfUser(Guid userId) {
            
                var orderList = _orderService.GetAllOrdersOfUser(userId);
                List<OrderDto> orderDtos = _mapper.Map<List<OrderDto>>(orderList);
                return Ok(orderDtos);
           
        }

        [HttpPost]
        public IActionResult AddOrder(OrderDto orderDto)
        {
            try
            {
                Order order = _mapper.Map<Order>(orderDto);
                _orderService.Add(order);
                return CreatedAtAction(nameof(GetAllOrders), new { id = order.OrderId }, new { orderId = order.OrderId });
            }
            catch (InvalidOrderException ioe)
            {
                return BadRequest(new { error = $"{ioe.Message},{ioe.StatusCode}" });
            }
        }

        [HttpPut]
        public IActionResult UpdateOrder(OrderDto orderDto)
        {
            try
            {
                Order order = _mapper.Map<Order>(orderDto);
                Order updatedOrder = _orderService.Update(order);
                return Ok(updatedOrder);
            }
            catch (InvalidOrderException ioe)
            {
                return BadRequest(new { error = $"{ioe.Message},{ioe.StatusCode}" });
            }
            catch (InvalidOrderUpdateRequestException ioe)
            {
                return Conflict(new { error = $"{ioe.Message},{ioe.StatusCode}" });
            }
        }

        
        [HttpDelete("{id}")]
        public IActionResult DeleteOrder(Guid id)
        {
            try
            {
                _orderService.Delete(id);
                return Ok(new { message = "Order Deleted" });
            }
            catch (WishlistNotFoundException onfe)
            {
                return NotFound(new { error = $"{onfe.Message},{onfe.StatusCode}" });
            }
            catch (InvalidOrderException ioe)
            {
                return BadRequest(new { error = $"{ioe.Message},{ioe.StatusCode}" });
            }


        }
    }
}

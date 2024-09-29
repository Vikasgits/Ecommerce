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
    public class LineItemController : ControllerBase
    {
        private readonly ILineItemService _lineItemService;
        private readonly IMapper _mapper;
        
        public LineItemController(ILineItemService lineItemService, IMapper mapper)
        {
            _lineItemService = lineItemService;
            _mapper = mapper;
            
        }


        [HttpGet]
        public IActionResult GetAllLineItems()
        {
            List<LineItem> lineItemList = _lineItemService.GetAll();
            List<LineItemDto> lineItemListDto = _mapper.Map<List<LineItemDto>>(lineItemList);
            return Ok(lineItemListDto);
        }

        [HttpGet("{id:guid}")]
        public IActionResult GetLineItem(Guid id)
        {
            try
            {
                var lineItem = _lineItemService.GetById(id);
                LineItemDto lineItemDto =_mapper.Map<LineItemDto>(lineItem);
                return Ok(lineItemDto);

            }
            catch (LineItemNotFoundException linfe)
            {
                return NotFound(new { error = $"{linfe.Message},{linfe.StatusCode}" });
            }
            
        }

        [HttpPost]
        public IActionResult PostLineItem(LineItemDto lineItemDto)
        {
            try
            {
                LineItem lineItem = _mapper.Map<LineItem>(lineItemDto);
                _lineItemService.Add(lineItem);
                return CreatedAtAction(nameof(GetAllLineItems), new { id = lineItem.Id }, new { message = $"Line Item added successfully + {lineItem.Id}" });
            }
            catch (InvalidLineItemException ilipe)
            {
                return BadRequest(new { error = $"{ilipe.Message},{ilipe.StatusCode}" });
            }
        }



        [HttpPut]
        public IActionResult UpdateLineItem(LineItemDto lineItemDto)
        {
            try
            {
                LineItem lineItem = _mapper.Map<LineItem>(lineItemDto);
                LineItem updatedLineItem = _lineItemService.Update(lineItem);
                return Ok(updatedLineItem);
            }
            catch (InvalidLineItemException ipe)
            {
                return BadRequest(new { error = $"{ipe.Message},{ipe.StatusCode}" });
            }
            catch (InsufficientQuantityException iqe)
            {
                return BadRequest(new { error = $"{iqe.Message}, {iqe.StatusCode}" });
            }

        }

        [HttpGet("OrderId/{orderId:guid}")]
        public IActionResult GetLineItemsByOrderId(Guid orderId)
        {
            var lineItems= _lineItemService.GetLineItems(orderId);
            return Ok(lineItems);
        }

        [HttpDelete]
        public IActionResult DeleteLineItem(Guid id)
        {
            try
            {
                _lineItemService.Delete(id);
                return Ok(new { message = "Line Item Removed" });
            }
            catch (InvalidLineItemException ipe)
            {
                return BadRequest(new { error = $"{ipe.Message},{ipe.StatusCode}" });
            }
        }
    }
}

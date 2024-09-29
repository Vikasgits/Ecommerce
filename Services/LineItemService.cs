using Azure.Core.Pipeline;
using E_commerce.Exceptions;
using E_commerce.Models;
using E_commerce.Repository;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.Services
{
    public class LineItemService : ILineItemService
    {

        private readonly IEntityRepository<LineItem> _lineItemRepository;

        public LineItemService(IEntityRepository<LineItem> lineItemRepository)
        {
            _lineItemRepository = lineItemRepository;
        }
        public bool Add(LineItem lineItem)
        {
            if (lineItem == null || lineItem.Quantity <= 0) throw new InvalidLineItemException("Not a valid line item to add");
            return _lineItemRepository.Add(lineItem);
        }

        public bool Delete(Guid id)
        {
            var getLineItem=GetById(id);
            return _lineItemRepository.Delete(getLineItem);
        }

        public List<LineItem> GetAll()
        {
            var lineItemList = _lineItemRepository.Get().ToList();
            return lineItemList;
        }

        public LineItem GetById(Guid id)
        {
            
                var lineItem = _lineItemRepository.Get().Where(item => item.Id == id).FirstOrDefault();
                if (lineItem == null)
                {
                    throw new LineItemNotFoundException("No such lineItem exists");
                }
                return lineItem;
            
        }

        public List<LineItem> GetLineItems(Guid orderId) {
            var lineItems = _lineItemRepository.Get().Where(item => item.OrderId == orderId).ToList();
            return lineItems;
        }
        

        public LineItem GetLineItemById(Guid id)      ///for local use
        {

            var lineItem = _lineItemRepository.Get().Where(item => item.Id == id).FirstOrDefault();
            return lineItem;
        }

        public LineItem Update(LineItem lineItem)
        {
            var existLineItem= GetLineItemById(lineItem.Id);
            if (existLineItem == null) throw new InvalidLineItemException("No such line item exist to be updated");
            if (lineItem.Quantity <= 0) throw new InsufficientQuantityException("Minimum Qunatity of line Item should be 1 ");
            return _lineItemRepository.Update(lineItem);
        }
    }
}

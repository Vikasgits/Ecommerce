using E_commerce.Models;

namespace E_commerce.Services
{
    public interface ILineItemService
    {
        public List<LineItem> GetAll();
        public LineItem GetById(Guid id);
        public bool Add(LineItem lineItem);
        public LineItem Update(LineItem lineItem);

        public List<LineItem> GetLineItems(Guid orderId);
        public bool Delete(Guid id);
    }
}

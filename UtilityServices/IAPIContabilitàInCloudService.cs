using System;
using System.Collections.Generic;
using System.Text;
using UtilityServices.Dtos;

namespace UtilityServices
{
    public interface IAPIContabilitàInCloudService
    {
        public void AddOrderCloud(OrderDto order);
        public void DeleteOrder(string productId);
        public void UpdateInventory(OrderDto order);
    }
}

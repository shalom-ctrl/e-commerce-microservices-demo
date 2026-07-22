using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Extensions;
using OrderApi.Application.Interfaces;
using Polly;
using Polly.Registry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Application.Services
{
    public class OrderService(IOrder orderinterface,HttpClient httpClient, ResiliencePipelineProvider<string> resiliencePipeline) : IOrderService
    {
        public async Task<ProductDTO> GetProduct(int productId)
        {
            var product = await httpClient.GetAsync($"/api/products/{productId}");
            if (!product.IsSuccessStatusCode)
            {
                return null!;
            }

            var result = await product.Content.ReadFromJsonAsync<ProductDTO>();
            return result!;
        }

        public async Task<AppUserDTO> GetUser(int userId)
        {
            var user = await httpClient.GetAsync($"/api/users/{userId}");
            if(!user.IsSuccessStatusCode)
            {
                return null!;
            }

            var result = await user.Content.ReadFromJsonAsync<AppUserDTO>();
            return result!;
        }
        public async Task<OrderDetailsDTO> GetOrderDetails(int orderId)
        {
            var order = await orderinterface.FindByIdAsync(orderId);
            if(order is null || order!.Id <= 0)
            {
                return null!;
            }

            var retryPipeline = resiliencePipeline.GetPipeline("my-retry-pipeline");

            var productdto = await retryPipeline.ExecuteAsync(async token => await GetProduct(order.ProductId));

            var appuserdto = await retryPipeline.ExecuteAsync(async token => await GetUser(order.ClientId));

            return new OrderDetailsDTO(
                order.Id,
                productdto.Id,
                appuserdto.Id,
                appuserdto.Name,
                appuserdto.Email,
                appuserdto.Address,
                appuserdto.TelephoneNumber,
                productdto.Name,
                order.PurchaseQuantity,
                productdto.Price,
                productdto.Quantity * order.PurchaseQuantity,
                order.OrderedDate
                );
        }

        public async Task<IEnumerable<OrderDTO>> GetOrdersByClientId(int clientId)
        {
           var orders = await orderinterface.GetAllOrdersAsync(o => o.ClientId == clientId);
            if (!orders.Any())
            {
                return Enumerable.Empty<OrderDTO>();
            }

            var (_, _orders) = OrderConversion.FromEntity(null, orders);
            return _orders!;
        }
    }
}

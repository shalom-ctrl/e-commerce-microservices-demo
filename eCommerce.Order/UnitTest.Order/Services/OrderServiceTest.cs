using FakeItEasy;
using FluentAssertions;
using OrderApi.Application.DTOs;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Services;
using System.Linq.Expressions;
using System.Net.Http.Json;

namespace UnitTest.Order.Services
{
    public class OrderServiceTest
    {
        private readonly IOrderService orderServiceInterface;
        private readonly IOrder orderInterface;

        public OrderServiceTest()
        {
            orderInterface = A.Fake<IOrder>();
            orderServiceInterface = A.Fake<IOrderService>();
        }

        public class FakeHttpMessageHandler(HttpResponseMessage response) : HttpMessageHandler
        {
            private readonly HttpResponseMessage _response = response;

            protected override Task<HttpResponseMessage> SendAsync
                                (HttpRequestMessage request, CancellationToken cancellationToken)

                => Task.FromResult(_response);
        }

        private static HttpClient CreateFakeHttpClient(object o)
        {
            var httpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = JsonContent.Create(o)
            };
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(httpResponseMessage);
            var _httptClient = new HttpClient(fakeHttpMessageHandler)
            {
                BaseAddress = new Uri("http://localhost")
            };
            return _httptClient;
        }

        [Fact]
        public async Task GetProduct_ValidProductId_ReturnProduct()
        {
            int productId = 1;
            var productDTO = new ProductDTO(1, "Product 1", 13, 56.78m);
            var _httpClient = CreateFakeHttpClient(productDTO);

            var _orderService = new OrderService(null!, _httpClient, null!);
            var result = await _orderService.GetProduct(productId);

            result.Should().NotBeNull();
            result.Id.Should().Be(productId);
            result.Name.Should().Be("Product 1");
        }

        [Fact]
        public async Task GetProduct_InvalidProductId_ReturnNull()
        {
            int productId = 1;
            var _httpclient = CreateFakeHttpClient(null!);
            var _orderService = new OrderService(null!, _httpclient, null!);
            var result = await _orderService.GetProduct(productId);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetOrdersByClientId_OrdersExist_ReturnOrderDetails()
        {
            int clientId = 1;
            var orders = new List<OrderApi.Domain.Entities.Order>
            {
                new() { Id = 1, ProductId = 1, ClientId = clientId, PurchaseQuantity = 2, OrderedDate = DateTime.Now },
                new() { Id = 2, ProductId = 2, ClientId = clientId, PurchaseQuantity = 1, OrderedDate = DateTime.Now }
            };

            A.CallTo(() => orderInterface.GetAllOrdersAsync
            (A<Expression<Func<OrderApi.Domain.Entities.Order, bool>>>.Ignored)).Returns(orders);
            var _orderService = new OrderService(orderInterface, null!, null!);
            var result = await _orderService.GetOrdersByClientId(clientId);

            result.Should().NotBeNull();
            result.Should().HaveCount(orders.Count);
            result.Should().HaveCountGreaterThanOrEqualTo(2);
        }
    }
}

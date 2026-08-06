using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;
using System.Linq.Expressions;

namespace UnitTest.Product.Repositories
{
    public class ProductRepositoryTest
    {
        private readonly ProductDbContext productDbContext;
        private readonly ProductRepository productRepository;

        public ProductRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<ProductDbContext>()
                .UseInMemoryDatabase(databaseName: "TestProductDb")
                .Options;

            productDbContext = new ProductDbContext(options);
            productRepository = new ProductRepository(productDbContext);
        }

        [Fact]
        public async Task CreateAsync_WhenProductAlreadyExist_ReturnErrorResponse()
        {
            var existingProduct = new ProductApi.Domain.Entity.Product { Name = "ExistingProduct" };
            productDbContext.Products.Add(existingProduct);
            await productDbContext.SaveChangesAsync();

            var result = await productRepository.CreateAsync(existingProduct);

            result.Should().NotBeNull();
            result.Flag.Should().BeFalse();
            result.Message.Should().Be("ExistingProduct already exists");
        }

        [Fact]
        public async Task CreateAsync_WhenProductDoesNotExist_AddProductAndReturnsSuccessResponse()
        {
            var product = new ProductApi.Domain.Entity.Product() { Name = "New Product" };
            var result = await productRepository.CreateAsync(product);

            result.Should().NotBeNull();
            result.Flag.Should().BeTrue();
            result.Message.Should().Be("New Product added to database successfully");
        }

        [Fact]
        public async Task DeleteAsync_WhenProductIsFound_ReturnsSuccessResponse()
        {
            var product = new ProductApi.Domain.Entity.Product() { Id = 1, Name = "Existing Product", Price = 76.87m, Quantity = 100 };
            productDbContext.Products.Add(product);
            var result = await productRepository.DeleteAsync(product);

            result.Should().NotBeNull();
            result.Flag.Should().BeTrue();
            result.Message.Should().Be("Existing Product is deleted successfully");
        }

        [Fact]
        public async Task DeleteAsync_WhenProductIsNotFound_ReturnsNotFoundResponse()
        {
            var product = new ProductApi.Domain.Entity.Product() { Id = 2, Name = "NonExistingProduct", Price = 78.67m, Quantity = 50 };
            var result = await productRepository.DeleteAsync(product);

            result.Should().NotBeNull();
            result.Flag.Should().BeFalse();
            result.Message.Should().Be("NonExistingProduct not found");
        }

        [Fact]
        public async Task FindByIdAsync_WhenProductIsFound_ReturnsProduct()
        {
            var product = new ProductApi.Domain.Entity.Product() { Id = 1, Name = "ExistingProduct", Price = 76.87m, Quantity = 5 };
            productDbContext.Products.Add(product);
            await productDbContext.SaveChangesAsync();

            var result = await productRepository.FindByIdAsync(product.Id);

            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("ExistingProduct");
        }

        [Fact]
        public async Task FindByIdAsync_WhenProductIsNotFound_ReturnNull()
        {
            var result = await productRepository.FindByIdAsync(99);
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllAsync_WhenProductsAreFound_ReturnProducts()
        {
            var products = new List<ProductApi.Domain.Entity.Product>
            {
                new(){Id = 1, Name = "Product 1"},
                new(){Id = 2, Name = "Product 2"},
            };
            productDbContext.Products.AddRange(products);
            await productDbContext.SaveChangesAsync();

            var result = await productRepository.GetAllAsync();

            result.Should().NotBeNull();
            result.Count().Should().Be(2);
            result.Should().Contain(p => p.Name == "Product 1");
            result.Should().Contain(p => p.Name == "Product 2");
        }

        [Fact]
        public async Task GetAllAsync_WhenProductsAreNotFound_ReturnNull()
        {
            var result = await productRepository.GetAllAsync();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetByAsync_WhenProductIsFound_ReturnProduct()
        {
            var product = new ProductApi.Domain.Entity.Product() { Id = 1, Name = "Product 1" };
            productDbContext.Products.Add(product);
            await productDbContext.SaveChangesAsync();
            Expression<Func<ProductApi.Domain.Entity.Product, bool>> predicate = p => p.Name == "Product 1";

            var result = await productRepository.GetByAsync(predicate);
            result.Should().NotBeNull();
            result.Name.Should().Be("Product 1");
        }

        [Fact]
        public async Task GetByAsync_WhenProductIsNotFound_ReturnNull()
        {
            Expression<Func<ProductApi.Domain.Entity.Product, bool>> predicate = p => p.Name == "Product 2";
            var result = await productRepository.GetByAsync(predicate);
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAsync_WhenProductIsUpdatedSuccessfully_ReturnsSuccessResponse()
        {
            var product = new ProductApi.Domain.Entity.Product() { Id = 1, Name = "Product 1" };
            productDbContext.Products.Add(product);
            await productDbContext.SaveChangesAsync();

            var result = await productRepository.UpdateAsync(product);

            result.Should().NotBeNull();
            result.Flag.Should().BeTrue();
            result.Message.Should().Be("Product 1 updated successfully");
        }

        [Fact]
        public async Task UpdateAsync_WhenProductIsNotFound_ReturnErrorResponse()
        {
            var updateProduct = new ProductApi.Domain.Entity.Product() { Id = 2, Name = "Product 2" };
            var result = await productRepository.UpdateAsync(updateProduct);

            result.Should().NotBeNull();
            result.Flag.Should().BeFalse();
            result.Message.Should().Be("Product 2 not found");
        }
    }
}

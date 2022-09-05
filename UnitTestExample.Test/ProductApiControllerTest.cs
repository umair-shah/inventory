using Microsoft.AspNetCore.Mvc;
using Moq;
using UnitTestExample.Web.Controllers;
using UnitTestExample.Web.Models;
using UnitTestExample.Web.Repositories;

namespace UnitTestExample.Test
{
    public class ProductApiControllerTest
    {
        #region Ctor
        private readonly Mock<IRepository<Product>> _mockRepo;
        private readonly ProductsApiController _controller;

        private List<Product> products;

        public ProductApiControllerTest()
        {
            _mockRepo = new Mock<IRepository<Product>>();
            _controller = new ProductsApiController(_mockRepo.Object);

            products = new List<Product>()
            {
                new Product{Id =1, Name="Pencil", Price=100, Stock=50, Color="Red"},
                new Product{Id =2, Name="Notebook", Price=200, Stock=500, Color="Blue"}
            };
        }
        #endregion

        #region GetProducts_ActionExecute_ReturnOkResultWithProduct
        [Fact]
        public async void GetProducts_ActionExecute_ReturnOkResultWithProduct()
        {
            _mockRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(products);

            var result = await _controller.GetProducts();

            var okResult = Assert.IsType<OkObjectResult>(result);

            // View result da ".Model" property den product a ulasiyoruz api de ".Value" property uzerinden producta erisiyoruz.
            var returnProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);

            Assert.Equal<int>(2, returnProducts.ToList().Count());
        }
        #endregion


    }
}
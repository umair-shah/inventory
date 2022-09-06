using Microsoft.AspNetCore.Mvc;
using Moq;
using UnitTestExample.Web.Controllers;
using UnitTestExample.Web.Helpers;
using UnitTestExample.Web.Models;
using UnitTestExample.Web.Repositories;

namespace UnitTestExample.Test
{
    public class ProductApiControllerTest
    {
        #region Ctor
        private readonly Mock<IRepository<Product>> _mockRepo;
        private readonly ProductsApiController _controller;
        private readonly Helper _helper;

        private List<Product> products;

        public ProductApiControllerTest()
        {
            _mockRepo = new Mock<IRepository<Product>>();
            _controller = new ProductsApiController(_mockRepo.Object);
            _helper = new Helper();

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

        #region GetProduct_IdInValid_ReturnNotFound
        [Theory]
        [InlineData(3)]
        public async void GetProduct_IdInValid_ReturnNotFound(int id)
        {
            Product product = null;
            _mockRepo.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(product);

            var result = await _controller.GetProduct(id);

            Assert.IsType<NotFoundResult>(result);
        }
        #endregion

        #region GetProduct_IdIsValid_ReturnOkResultWithProduct
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async void GetProduct_IdIsValid_ReturnOkResultWithProduct(int id)
        {
            var product = products.First(x => x.Id == id);

            _mockRepo.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(product);

            var result = await _controller.GetProduct(id);

            var okResult = Assert.IsType<OkObjectResult>(result);

            var returnProdcut = Assert.IsType<Product>(okResult.Value);

            Assert.Equal(id, returnProdcut.Id);
            Assert.Equal(product.Name, returnProdcut.Name);
        }
        #endregion

        #region PutProduct_IdIsNotEqualProduct_ReturnBadRequest
        [Theory]
        [InlineData(1)]
        public void PutProduct_IdIsNotEqualProduct_ReturnBadRequest(int id)
        {
            var product = products.First(x => x.Id == id);

            var result = _controller.PutProduct(2, product);

            Assert.IsType<BadRequestResult>(result);
        }
        #endregion

        #region PutProduct_ActionExecute_ReturnNoContent
        [Theory]
        [InlineData(1)]
        public void PutProduct_ActionExecute_ReturnNoContent(int id)
        {
            var product = products.First(x => x.Id == id);

            _mockRepo.Setup(x => x.Update(product));

            var result = _controller.PutProduct(id, product);

            _mockRepo.Verify(x => x.Update(product), Times.Once);

            Assert.IsType<NoContentResult>(result);
        }
        #endregion

        #region PostProduct_ActionExecute_ReturnCreatedAtAction
        [Fact]
        public async void PostProduct_ActionExecute_ReturnCreatedAtAction()
        {
            var product = products.First();

            _mockRepo.Setup(x => x.CreateAsync(product)).Returns(Task.CompletedTask);

            var result = await _controller.PostProduct(product);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);

            _mockRepo.Verify(x => x.CreateAsync(product), Times.Once);

            Assert.Equal("GetProduct", createdAtActionResult.ActionName);
        }
        #endregion

        #region DeleteProduct_IdInValid_ReturnNotFound
        [Theory]
        [InlineData(0)]
        public async void DeleteProduct_IdInValid_ReturnNotFound(int id)
        {
            Product product = null;

            _mockRepo.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(product);

            var resultNotFound = await _controller.DeleteProduct(id);

            Assert.IsType<NotFoundResult>(resultNotFound.Result);
        }
        #endregion

        #region DeleteProduct_ActionExecute_ReturnNocontent
        [Theory]
        [InlineData(1)]
        public async void DeleteProduct_ActionExecute_ReturnNocontent(int id)
        {
            var product = products.First(x => x.Id == id);

            _mockRepo.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(product);

            _mockRepo.Setup(x => x.Delete(product));

            var noContentResult = await _controller.DeleteProduct(id);

            _mockRepo.Verify(x => x.Delete(product), Times.Once);

            Assert.IsType<NoContentResult>(noContentResult.Result);
        }
        #endregion

        #region HelperAdd_IntegerValues_ReturnTotal
        [Theory]
        [InlineData(2, 3, 5)]
        public void HelperAdd_IntegerValues_ReturnTotal(int number1, int number2, int total)
        {
            var result = _helper.Add(number1, number2);
            Assert.Equal(total, result);
        }
        #endregion
    }
}
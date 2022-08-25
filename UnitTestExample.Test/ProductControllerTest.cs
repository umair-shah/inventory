using Microsoft.AspNetCore.Mvc;
using Moq;
using UnitTestExample.Web.Controllers;
using UnitTestExample.Web.Models;
using UnitTestExample.Web.Repositories;

namespace UnitTestExample.Test
{
    public class ProductControllerTest
    {
        #region Ctor
        private readonly Mock<IRepository<Product>> _mockProductRepository;
        private readonly ProductsController _productsController;
        private List<Product> products;

        public ProductControllerTest()
        {
            _mockProductRepository = new Mock<IRepository<Product>>();
            _productsController = new ProductsController(_mockProductRepository.Object);
            products = new List<Product>()
            {
                new Product{Id =1, Name="Pencil", Price=100, Stock=50, Color="Red"},
                new Product{Id =1, Name="Notebook", Price=200, Stock=500, Color="Blue"}
            };
        }
        #endregion

        #region Index_ActionExecutes_ReturnView
        [Fact]
        public async void Index_ActionExecutes_ReturnView()
        {
            var result = await _productsController.Index();

            Assert.IsType<ViewResult>(result);
        }
        #endregion

        #region Index_ActionExecutes_ReturnProductList
        [Fact]
        public async void Index_ActionExecutes_ReturnProductList()
        {
            _mockProductRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(products);

            var result = await _productsController.Index();

            var viewResult = Assert.IsType<ViewResult>(result);

            var productList = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);

            Assert.Equal<int>(2, productList.Count());
        }
        #endregion

        #region Details_IdIsNull_ReturnRedirectToIndexAction
        [Fact]
        public async void Details_IdIsNull_ReturnRedirectToIndexAction()
        {
            var result = await _productsController.Details(null);

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }
        #endregion

        #region Details_IdInValid_ReturnNotFound
        [Fact]
        public async void Details_IdInValid_ReturnNotFound()
        {
            Product product = null;

            _mockProductRepository.Setup(repo => repo.GetByIdAsync(0)).ReturnsAsync(product);

            var result = await _productsController.Details(0);

            var redirect = Assert.IsType<NotFoundResult>(result);

            Assert.Equal<int>(404, redirect.StatusCode);
        }
        #endregion

        #region Details_ValidId_ReturnProduct
        [Theory]
        [InlineData(1)]
        public async void Details_ValidId_ReturnProduct(int productId)
        {
            Product product = products.FirstOrDefault(x => x.Id == productId);

            _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(product);

            var result = await _productsController.Details(productId);

            var viewResult = Assert.IsType<ViewResult>(result);

            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);

            Assert.Equal(product.Id, resultProduct.Id);
            Assert.Equal(product.Name, resultProduct.Name);
        }
        #endregion

        #region Create_ActionExecutes_ReturnView
        [Fact]
        public void Create_ActionExecutes_ReturnView()
        {
            var result = _productsController.Create();

            Assert.IsType<ViewResult>(result);
        }
        #endregion

        #region CreatePost_InValidModelState_ReturnView
        [Fact]
        public async void CreatePost_InValidModelState_ReturnView()
        {
            _productsController.ModelState.AddModelError("Name", "The Name field is required.");

            var result = await _productsController.Create(products.First());

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.IsType<Product>(viewResult.Model);
        }
        #endregion

        #region CreatePost_ValidModelSate_ReturnRedirectToIndexAction
        [Fact]
        public async void CreatePost_ValidModelSate_ReturnRedirectToIndexAction()
        {
            var result = await _productsController.Create(products.First());

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }
        #endregion

        #region CreatePost_ValidModelState_CreateMethodExecute
        [Fact]
        public async void CreatePost_ValidModelState_CreateMethodExecute()
        {
            Product newProduct = null;

            _mockProductRepository.Setup(repo => repo.CreateAsync(It.IsAny<Product>()))
                                  .Callback<Product>(x => newProduct = x);

            var result = await _productsController.Create(products.First());

            _mockProductRepository.Verify(repo => repo.CreateAsync(It.IsAny<Product>()), Times.Once);

            Assert.Equal(products.First().Id, newProduct.Id);
        }
        #endregion

        #region CreatePost_InValidModelState_NeverCreateExecute
        [Fact]
        public async void CreatePost_InValidModelState_NeverCreateExecute()
        {
            _productsController.ModelState.AddModelError("Name", "");

            var result = await _productsController.Create(products.First());

            _mockProductRepository.Verify(repo => repo.CreateAsync(It.IsAny<Product>()), Times.Never);
        }
        #endregion
    }
}
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
                new Product{Id =2, Name="Notebook", Price=200, Stock=500, Color="Blue"}
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

            var notFound = Assert.IsType<NotFoundResult>(result);

            Assert.Equal<int>(404, notFound.StatusCode);
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

        #region Edit_IdIsNull_RedirectToIndexAction
        [Fact]
        public async void Edit_IdIsNull_RedirectToIndexAction()
        {
            var result = await _productsController.Edit(null);

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }
        #endregion

        #region Edit_IdInValid_ReturnNotFound
        [Theory]
        [InlineData(3)]
        public async void Edit_IdInValid_ReturnNotFound(int productId)
        {
            Product product = null;

            _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(product);

            var result = await _productsController.Edit(productId);

            var notFound = Assert.IsType<NotFoundResult>(result);

            Assert.Equal<int>(404, notFound.StatusCode);
        }
        #endregion

        #region Edit_ActionExecutes_ReturnProduct
        [Theory]
        [InlineData(2)]
        public async void Edit_ActionExecutes_ReturnProduct(int productId)
        {
            var product = products.First(x => x.Id == productId);

            _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(product);

            var result = await _productsController.Edit(productId);

            var viewResult = Assert.IsType<ViewResult>(result);

            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);

            Assert.Equal(product.Id, resultProduct.Id);
            Assert.Equal(product.Name, resultProduct.Name);
        }
        #endregion

        #region EditPost_IdIsNotEqualProduct_ReturnNotFound
        [Theory]
        [InlineData(2)]
        public void EditPost_IdIsNotEqualProduct_ReturnNotFound(int productId)
        {
            var result = _productsController.Edit(1, products.First(x => x.Id == productId));

            Assert.IsType<NotFoundResult>(result);
        }
        #endregion

        #region EditPost_InValidModelState_ReturnView
        [Theory]
        [InlineData(2)]
        public void EditPost_InValidModelState_ReturnView(int productId)
        {
            _productsController.ModelState.AddModelError("Name", "");

            var result = _productsController.Edit(productId, products.First(x => x.Id == productId));

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.IsType<Product>(viewResult.Model);
        }
        #endregion

        #region EditPost_ValidModelState_ReturnRedirectToIndexAction
        [Theory]
        [InlineData(1)]
        public void EditPost_ValidModelState_ReturnRedirectToIndexAction(int productId)
        {
            var result = _productsController.Edit(productId, products.First(x => x.Id == productId));

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }
        #endregion

        #region EditPost_ValidModelState_UpdateMethodExecute
        [Theory]
        [InlineData(1)]
        public void EditPost_ValidModelState_UpdateMethodExecute(int productId)
        {
            var product = products.First(x => x.Id == productId);

            _mockProductRepository.Setup(repo => repo.Update(product));

            _productsController.Edit(productId, product);

            _mockProductRepository.Verify(repo => repo.Update(It.IsAny<Product>()), Times.Once);
        }
        #endregion

        #region Delete_IdIsNull_ReturnNotFound
        [Fact]
        public async void Delete_IdIsNull_ReturnNotFound()
        {
            var result = await _productsController.Delete(null);

            Assert.IsType<NotFoundResult>(result);
        }
        #endregion

        #region Delete_IdISNotEqualProduct_ReturnNotFound
        [Theory]
        [InlineData(0)]
        public async void Delete_IdISNotEqualProduct_ReturnNotFound(int productId)
        {
            Product product = null;
            _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(product);

            var result = await _productsController.Delete(productId);

            Assert.IsType<NotFoundResult>(result);
        }
        #endregion

        #region Delete_ActionExecutes_ReturnProduct
        [Theory]
        [InlineData(1)]
        public async void Delete_ActionExecutes_ReturnProduct(int productId)
        {
            var product = products.First(x => x.Id == productId);

            _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(product);

            var result = await _productsController.Delete(productId);

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.IsAssignableFrom<Product>(viewResult.Model);
        }
        #endregion

        #region DeleteConfirm_ActionExecutes_ReturnRedirectToIndexAction
        [Theory]
        [InlineData(1)]
        public async void DeleteConfirmed_ActionExecutes_ReturnRedirectToIndexAction(int productId)
        {
            var result = await _productsController.DeleteConfirmed(productId);
            Assert.IsType<RedirectToActionResult>(result);
        }
        #endregion

        #region DeleteConfirmed_ActionExecutes_DeleteMethodExecute
        [Theory]
        [InlineData(1)]
        public async void DeleteConfirmed_ActionExecutes_DeleteMethodExecute(int productId)
        {
            Product product = products.First(x => x.Id == productId);

            _mockProductRepository.Setup(repo => repo.Delete(product));

            await _productsController.DeleteConfirmed(productId);

            _mockProductRepository.Verify(repo => repo.Delete(It.IsAny<Product>()), Times.Once);
        }
        #endregion

        #region MyRegion

        #endregion
    }
}
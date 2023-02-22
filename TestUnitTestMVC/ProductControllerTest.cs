using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Moq;
using System.Linq;
using UnitTestMVC.web.Controllers;
using UnitTestMVC.web.Models;
using UnitTestMVC.web.Repositories;

namespace TestUnitTestMVC
{
    public class ProductControllerTest
    {
        private readonly Mock<IRepository<Product>> _mock;

        private readonly ProductsController _productController;

        private List<Product> products;

        public ProductControllerTest()
        {
            _mock = new Mock<IRepository<Product>>();
            _productController = new ProductsController(_mock.Object);

            products = new List<Product>()
            {
                new Product{ Id=1, Name="Kalem", Price=100, Stock=50,Color="Kırmızı"},
                new Product{ Id=2, Name="Defter", Price=200, Stock=20,Color="Sarı"},
                new Product{ Id=3, Name="Silgi", Price=10, Stock=500,Color="Mor"}
            };
        }

        [Fact]
        public async void Index_ActionExecutes_ReturnView()
        {
            var result = await _productController.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void Index_ActionExecutes_ReturnProductList()
        {
            _mock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(products);

            var result = await _productController.Index();

            var viewResult = Assert.IsType<ViewResult>(result);

            var productList = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);

            Assert.Equal<int>(3, productList.Count());

        }

        [Fact]
        public async void Details_IdIsNull_RedirectToIndexAction()
        {
            var result = await _productController.Details(null);

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async void Details_IdInvalid_NotFound()
        {
            Product product = null;

            _mock.Setup(x => x.GetByIdAsync(0)).ReturnsAsync(product);

            var result = await _productController.Details(0);

            var redirect = Assert.IsType<NotFoundResult>(result);

            Assert.Equal<int>(404, redirect.StatusCode);
        }

        [Theory]
        [InlineData(1)]
        public async void Details_ValidId_ReturnProducts(int id)
        {
            //Test sınıfındaki product'dan id al
            Product product = products.FirstOrDefault(x => x.Id == id);

            //Mock ile Repository'deki method'u çalıştır 
            _mock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(product);

            //Controller'daki asıl method'u çalıştır
            var result = await _productController.Details(id);

            //Return view'ın tipini kontrol et
            var viewResult = Assert.IsType<ViewResult>(result);

            //Return'de hangi tip döndüğünü kontrol et
            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);

            //Geriye döndürdüğün model ile burdaki döndürdüğün model'in değerlerini kontrol et
            Assert.Equal(product.Id, resultProduct.Id);
            Assert.Equal(product.Name, resultProduct.Name);
        }

        [Fact]
        public void Create_ActionExcute_ReturnView()
        {
            var result = _productController.Create();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void CreatePost_InValidModelState_ReturnView()
        {
            _productController.ModelState.AddModelError("Name", "Name alanı boş geçilemez");

            var result = await _productController.Create(products.First());

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.IsType<Product>(viewResult.Model);
        }

        [Fact]
        public async void CreatePost_ValidModelState_ReturnRedirectToIndexAction()
        {
            var result = await _productController.Create(products.First());

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async void CreatePost_ValidModelState_CreateMethodExecuted()
        {
            Product newProduct = null;

            _mock.Setup(x => x.CreateAsync(It.IsAny<Product>()))
                .Callback<Product>(x => newProduct = x);

            var result = await _productController.Create(products.First());

            _mock.Verify(x => x.CreateAsync(It.IsAny<Product>()), Times.Once);

            Assert.Equal(products.First().Id, newProduct.Id);

        }

        [Fact]
        public async void CreatePost_InValidModelState_NeverCreateMethodEcecute()
        {
            _productController.ModelState.AddModelError("Name", "");

            var result = await _productController.Create(products.First());

            _mock.Verify(x => x.CreateAsync(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async void Edit_IdIsNull_ReturnRedirectToIndexAction()
        {
            var result = await _productController.Edit(null);

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }

        [Theory]
        [InlineData(5)]
        public async void Edit_IdInvalid_ReturnNotFound(int id)
        {
            Product product = null;

            _mock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(product);

            var result = await _productController.Edit(id);

            var redirect = Assert.IsType<NotFoundResult>(result);

            Assert.Equal(404, redirect.StatusCode);

        }

        [Theory]
        [InlineData(1)]
        public async void Edit_ActionExecutes_ReturnProduct(int id)
        {
            var product = products.First(x => x.Id == id);

            _mock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(product);

            var result = await _productController.Edit(id);

            var viewResult = Assert.IsType<ViewResult>(result);

            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);

            Assert.Equal(id, resultProduct.Id);
            Assert.Equal(product.Name, resultProduct.Name);
        }

        [Theory]
        [InlineData(1)]
        public void EditPost_IdIsNotEqualProductId_ReturnNotFound(int id)
        {
            var result = _productController.Edit(2, products.First(x => x.Id == id));

            var redirect = Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public void EditPost_InValidModelState_ReturnView(int id)
        {
            _productController.ModelState.AddModelError("Name", "");

            var result = _productController.Edit(id, products.First(x => x.Id == id));

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.IsType<Product>(viewResult.Model);

        }

        [Theory]
        [InlineData(1)]
        public void EditPost_ValidModelState_ReturnRedirectToIndexAction(int id)
        {
            var result = _productController.Edit(id, products.First(x => x.Id == id));

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }

        [Theory]
        [InlineData(1)]
        public void EditPost_ValidModelState_ReturnUpdateMethodExecute(int id)
        {
            var product = products.First(x => x.Id == id);

            _mock.Setup(x => x.Update(product));

            var result = _productController.Edit(id, product);

            _mock.Verify(x => x.Update(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async void Delete_IdIsNull_ReturnNotFound()
        {
            var result = await _productController.Delete(null);

            var redirect = Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(0)]
        public async void Delete_IdIsNotEqualProduct_ReturnNotFound(int id)
        {
            Product product = null;

            _mock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(product);

            var result = await _productController.Delete(id);

            var redirect = Assert.IsType<NotFoundResult>(result);

            Assert.IsType<NotFoundResult>(redirect);
        }

        [Theory]
        [InlineData(1)]
        public async void Delete_ActionExecutes_ReturnProduct(int id)
        {
            var product = products.First(x => x.Id == id);

            _mock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(product);

            var result = await _productController.Delete(id);

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.IsAssignableFrom<Product>(viewResult.Model);
        }

        [Theory]
        [InlineData(1)]
        public async void DeleteConfirm_ActionExecutes_DeleteMethodExecutes(int id)
        {
            var product = products.First(x => x.Id == id);

            _mock.Setup(x => x.Delete(product));

            await _productController.DeleteConfirmed(id);

            _mock.Verify(x=> x.Delete(It.IsAny<Product>()),Times.Once);
        }

        [Theory]
        [InlineData(1)]
        public async void DeleteConfirm_ActionExecutes_ReturnRedirectToIndexAction(int id)
        {
            var result = await _productController.DeleteConfirmed(id);

            Assert.IsType<RedirectToActionResult>(result);
        }

    }
}

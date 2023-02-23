using Microsoft.AspNetCore.Mvc;
using Moq;
using UnitTestMVC.web.Controllers;
using UnitTestMVC.web.Helpers;
using UnitTestMVC.web.Models;
using UnitTestMVC.web.Repositories;

namespace TestUnitTestMVC
{
    public class ProductApiControllerTest
    {
        private readonly Mock<IRepository<Product>> _mock;

        private readonly ProductsApiController _productApiController;

        private List<Product> products;

        private readonly Helper _helper;

        public ProductApiControllerTest()
        {
            _mock = new Mock<IRepository<Product>>();
            _productApiController = new ProductsApiController(_mock.Object);

            products = new List<Product>()
            {
                new Product{ Id=1, Name="Kalem", Price=100, Stock=50,Color="Kırmızı"},
                new Product{ Id=2, Name="Defter", Price=200, Stock=20,Color="Sarı"},
                new Product{ Id=3, Name="Silgi", Price=10, Stock=500,Color="Mor"}
            };

            _helper = new Helper();
        }


        [Fact]
        public async void GetProduct_ActionExecute_ReturnOkResultWithProduct()
        {
            _mock.Setup(x => x.GetAllAsync()).ReturnsAsync(products);

            var result = await _productApiController.GetProducts();

            var okResult = Assert.IsType<OkObjectResult>(result);

            var returnProduct = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);

            Assert.Equal<int>(3, returnProduct.Count());
        }

        [Theory]
        [InlineData(0)]
        public async void GetProduct_IdIsInvalid_ReturnNotFound(int id)
        {
            Product product = null;

            _mock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(product);

            var result = await _productApiController.GetProduct(id);

            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public async void GetProduct_IdValid_ReturnOkResult(int id)
        {
            Product product = products.First(x => x.Id == id);

            _mock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(product);

            var result = await _productApiController.GetProduct(id);

            var okResult = Assert.IsType<OkObjectResult>(result);

            var returnProduct = Assert.IsType<Product>(okResult.Value);

            Assert.Equal(id, returnProduct.Id);
        }

        [Theory]
        [InlineData(1)]
        public void PutProduct_IdIsNotEqualProductId_ReturnBadRequestResult(int id)
        {
            var product = products.First(x => x.Id == id);

            var result = _productApiController.PutProduct(2, product);

            Assert.IsType<BadRequestResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public void PutProduct_ActionExecute_ReturnNoContent(int id)
        {
            var product = products.First(x => x.Id == id);

            _mock.Setup(x => x.Update(product));

            var result = _productApiController.PutProduct(id, product);

            _mock.Verify(x => x.Update(product), Times.Once);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async void PostProduct_ActionExevute_ReturnCreateAtAction()
        {
            var product = products.First(x => x.Id == 1);

            _mock.Setup(x => x.CreateAsync(product)).Returns(Task.CompletedTask);

            var result = await _productApiController.PostProduct(product);

            var actionResult = Assert.IsType<CreatedAtActionResult>(result);

            _mock.Verify(x => x.CreateAsync(product), Times.Once);

            Assert.Equal("GetProduct", actionResult.ActionName);
        }

        [Theory]
        [InlineData(0)]
        public async void DeleteProduct_IdInvalid_ReturnNotFound(int id)
        {
            Product product = null;

            _mock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(product);

            var resultNotFound = await _productApiController.DeleteProduct(id);

            //Test edilen method Interface değil generic sınıf dönüyorsa .Result veya .Value
            Assert.IsType<NotFoundResult>(resultNotFound.Result);
        }

        [Theory]
        [InlineData(1)]
        public async void DeleteProduct_ActionExecute_ReturnNoContent(int id)
        {
            var product = products.First(x => x.Id == id);

            _mock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(product);

            _mock.Setup(x => x.Delete(product));

            var resultNoContent = await _productApiController.DeleteProduct(id);

            _mock.Verify(x => x.Delete(product), Times.Once);

            Assert.IsType<NoContentResult>(resultNoContent.Result);
        }

        [Theory]
        [InlineData(4, 5, 9)]
        public void Add_SamplerValues_ReturnTotal(int a, int b, int total)
        {
           var result = _helper.Add(a, b);
           Assert.Equal(total, result);
        }

    }
}

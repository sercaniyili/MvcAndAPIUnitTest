﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnitTestMVC.web.Models;
using UnitTestMVC.web.Repositories;

namespace UnitTestMVC.web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsApiController : ControllerBase
    {
        private readonly IRepository<Product> _repository;

        public ProductsApiController(IRepository<Product> repository)
        {
            _repository = repository;
        }


        // GET: api/ProductsApi
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {         
            var products = await _repository.GetAllAsync();

            return Ok(products);
        }

        // GET: api/ProductsApi/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // PUT: api/ProductsApi/5
        [HttpPut("{id}")]
        public IActionResult PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }
           
            _repository.Update(product);
          
            return NoContent();
        }

        // POST: api/ProductsApi
        [HttpPost]
        public async Task<ActionResult> PostProduct(Product product)
        {
            await _repository.CreateAsync(product);

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        // DELETE: api/ProductsApi/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _repository.Delete(product);

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            Product product = _repository.GetByIdAsync(id).Result;

            if (product is null)
                return false;
            else
                return true;
        }


        [HttpGet("{a}/{b}")]
        public IActionResult Add(int a, int b)
        {
            return Ok(new Helpers.Helper().Add(2, 5));
        }


    }
}

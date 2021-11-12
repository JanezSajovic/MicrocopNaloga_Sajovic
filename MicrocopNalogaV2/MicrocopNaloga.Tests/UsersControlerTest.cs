using FakeItEasy;
using MicrocopNalogaV2;
using MicrocopNalogaV2.Controllers;
using System;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MicrocopNalogaV2.Models.Models;
using System.Threading.Tasks;
using System.Text;
using System.Security.Cryptography;

namespace MicrocopNaloga.Tests
{
    // Arrange mean that we need to arrange or set up the necessary parameters for the test.
    // Act means just do the action which might be calling a method, calling a controller.
    // Assert means just evaluate the result.
    public class UsersControlerTest
    {
        private readonly UserController _controller;
        private readonly IUserRepository _context;
        private readonly ILogger<UserController> _logger;
        private readonly IUserRepository _service;

        public UsersControlerTest(IUserRepository userRepository, ILogger<UserController> logger)
        {
            _logger = logger;
            _context = userRepository;
            _service = new IUserServiceFake();
            _controller = new UserController(_service, logger);

        }

        [Fact]
        public void GetAllUsersTest()
        {
            // Arange
            // Act
            var result = _controller.Gets();
            Assert.IsType<OkObjectResult>(result.Result);
            var list = result.Result as OkObjectResult;
            Assert.IsType<List<UserModel>>(list.Value);
            var listBooks = list.Value as List<UserModel>;

            // Pričakujemo da so v bazi 3 uporabniki
            Assert.Equal(3, listBooks.Count);
        }

        // Test s katerim pričakujemo da se pod ID = 1 nahajajo podatki o Cirilu Kosmaču
        // ID = 99 pa uporabnik ne obstaja 
        [Theory]
        [InlineData(1, 99)]
        public void GetFirstUserById(int trueId, int fakeId)
        {
            // Arange
            var validId = trueId;
            var invalidId = fakeId;

            // Act
            var notFoundResult = _controller.Get(validId);
            var okResult = _controller.Get(fakeId);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult.Result);
            Assert.IsType<OkObjectResult>(okResult.Result);

            var item = okResult.Result as OkObjectResult;

            Assert.IsType<UserModel>(item.Value);

            var user = item.Value as UserModel;
            Assert.Equal(trueId, user.Id);
            Assert.Equal("CirilK", user.UserName);
            Assert.Equal("ciril.k@gmail.com", user.Email);
            Assert.Equal("Ciril Kosmač", user.FullName);
            Assert.Equal("017541474", user.PhoneNumber);
            Assert.Equal("slovenščina", user.Language);
            Assert.Equal("si", user.Culture);
            Assert.True(user.IsValidated);

        }


        // Test dodajanje novega uporabnika z vsemi parametri ki so pravilni
        // Test dodajanje novega uporabnika z manjkajočimi parametri ki pa so zahtevani (required)
        [Fact]
        public void AddUserTest() {

            //Arrange
            var user = new UserMiniModel()
            {
                UserName = "KLovro",
                Email = "lovro.kuhar@gmail.com",
                Language = "slovenščina",
                Culture = "si",
                Password = "Kuhar123!",
                PhoneNumber = "030888555"                           
            };

            //Act
            var createdResponse = _controller.Create(user);

            Assert.IsType<Task<IActionResult>>(createdResponse);

            //value of the result
            var item = createdResponse as Task<IActionResult>;
            Assert.IsType<UserModel>(item.Result);

            //check value of this book
            var tempUser = item.Result as UserModel;
            Assert.Equal(tempUser.UserName, user.UserName);
            Assert.Equal(tempUser.Password, user.Password);
            Assert.Equal(tempUser.Email, user.Email);
            Assert.Equal(tempUser.PhoneNumber, user.PhoneNumber);
            Assert.Equal(tempUser.Culture, user.Culture);
            Assert.Equal(tempUser.Language, user.Language);


            //Arrange
            var incompleteUser = new UserMiniModel()
            {
                UserName = "BojanL",
                Email = "bojan.lovec@gmail.com",
                Language = "slovenščina",
                Culture = "si",
                Password = "Lovec123!",
            };

            //Act
            _controller.ModelState.AddModelError("Phone", "Title is a requried filed");
            var badResponse = _controller.Create(incompleteUser);

            //Assert
            Assert.IsType<BadRequestObjectResult>(badResponse);
        }


        // Brisanje uporabnika preko IDja ki ne obstaja. Velikost seznama se ne spremeni
        // Brisanje uporabnika. Velikost seznama se spremeni 
        [Theory]
        [InlineData(1, 99)]
        public void RemoveUserTest(int trueId, int fakeId) {
            // Arange
            var validId = trueId;
            var invalidId = fakeId;

            // Act
            var notFoundResult = _controller.Delete(invalidId);

            // Assert
            // Velikost seznama se ne spremeni
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.Equal(4, _service.Gets().Result.Count);

            // Act
            var okResult = _controller.Delete(trueId);


            // Assert
            // VElikost seznama se spremeni
            Assert.IsType<OkResult>(okResult);
            Assert.Equal(3, _service.Gets().Result.Count);
        }


        public string HashPassword(string password)
        {
            var hashed = "";
            using (var myHash = SHA256Managed.Create())
            {
                var byteArrayResultOfRawData = Encoding.UTF8.GetBytes(password);
                var byteArrayResult = myHash.ComputeHash(byteArrayResultOfRawData);
                hashed = string.Concat(Array.ConvertAll(byteArrayResult, h => h.ToString("X2")));
            }

            return hashed;
        }
    }
}

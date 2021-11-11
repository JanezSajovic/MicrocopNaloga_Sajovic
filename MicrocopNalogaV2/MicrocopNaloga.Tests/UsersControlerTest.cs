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

namespace MicrocopNaloga.Tests
{
    public class UsersControlerTest
    {
        public UserController _controller;
        public IUserRepository _context;
        public ILogger<UserController> _logger;
        public IConfiguration _config;

        public UsersControlerTest(IUserRepository userRepository, IConfiguration config, ILogger<UserController> logger)
        {
            _logger = logger;
            _config = config;
            _context = userRepository;
            _controller = new UserController(_context, _config, _logger);

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

            // Pričakujemo da je v bazi 5 uporabnikov
            Assert.Equal(5, listBooks.Count);
        }

        // Test s katerim pričakujemo da se pod ID=1 nahajajo podatki o Cirilu Kosmaču
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

        [Fact]
        public void AddUserTest() {
            //OK RESULT TEST START

            //Arrange
            var user = new UserMiniModel()
            {
                UserName = "KLovro",
                Email = "lovro.kuhar@gmail.com",
                Language = "slovenščina",
                Culture = "si",
                Password = "Kurah123!",
                PhoneNumber = "030888555"                           
            };

            //Act
            var createdResponse = _controller.Create(user);

            //Assert
            Assert.IsType<UserModel>(createdResponse);

            //value of the result
            //var item = createdResponse as UserModel;
            //Assert.IsType<UserModel>(item.Value);

            //check value of this book
            //var tempUser = item.Value as UserModel;
            //Assert.Equal(user.Author, bookItem.Author);
        }
    }
}

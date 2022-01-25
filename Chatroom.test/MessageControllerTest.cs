using AutoMapper;
using Chatroom.Controllers;
using Chatroom.DTOs;
using Chatroom.Mapper;
using Chatroom.Models;
using Chatroom.Services.Interfaces;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatroom.test
{
    [TestClass]
    public class MessageControllerTest
    {

        private readonly MessageController _controller;
        private readonly Mock<IMessageService> _messageServiceMock;
        private readonly IMapper _mapper;

        public MessageControllerTest()
        {
            _messageServiceMock = new Mock<IMessageService>();

            var dataProtectorMock = new Mock<IDataProtector>();
            var dataProtectionProviderMock = new Mock<IDataProtectionProvider>();
            dataProtectionProviderMock.Setup(d => d.CreateProtector(It.IsAny<string>()))
                .Returns(dataProtectorMock.Object);

            var configurationMock = new Mock<IConfiguration>();

            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapperProfile(configurationMock.Object, dataProtectionProviderMock.Object));
            }).CreateMapper();

            _controller = new MessageController(_messageServiceMock.Object, _mapper);
        }


        [TestMethod]
        public async Task GetAll_MustSucceed()
        {
            var credential = new Credential("Lewis_Hamilton");
            var messages = new List<Message>()
            {
                new Message("2021 champ", credential)
            };
            _messageServiceMock.Setup(m => m.GetLatestMessagesAsync(It.IsAny<int>()))
                .ReturnsAsync(messages);

            var response = await _controller.GetAll();

            var result = response as OkObjectResult;
            var messagesDtos = result.Value as IEnumerable<MessageDTO>;
            _messageServiceMock.Verify(m => m.GetLatestMessagesAsync(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(messages[0].User.UserName, messagesDtos.First().UserName);
        }

        [TestMethod]
        public async Task DeleteAll_MustSucceed()
        {
            var response = await _controller.DeleteAll();

            Assert.IsInstanceOfType(response, typeof(NoContentResult));
            _messageServiceMock.Verify(m => m.DeleteAllAsync(), Times.Once);
        }
    }
}
using AutoMapper;
using Chatroom.Mapper;
using Chatroom.Models;
using Chatroom.Services;
using Chatroom.Services.Interfaces;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Chatroom.test
{
    [TestClass]
    public class ChatroomServiceTest
    {

        private readonly ChatroomService _chatroomService;
        private readonly Mock<IDataProtectionProvider> _dataProtectionProviderMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IMessageService> _messageServiceMock;
        private readonly Mock<UserManager<Credential>> _userManagerMock;
        private readonly IMapper _mapper;
        private readonly Mock<IHubCallerClients> _hubCallerClientsMock;

        public ChatroomServiceTest()
        {
            _messageServiceMock = new Mock<IMessageService>();
            var users = new List<Credential>();
            _userManagerMock = MockUserManager(users);
            _dataProtectionProviderMock = new Mock<IDataProtectionProvider>();
            _configurationMock = new Mock<IConfiguration>();

            var dataProtectorMock = new Mock<IDataProtector>();
            _dataProtectionProviderMock = new Mock<IDataProtectionProvider>();
            _dataProtectionProviderMock.Setup(d => d.CreateProtector(It.IsAny<string>()))
                .Returns(dataProtectorMock.Object);

            _hubCallerClientsMock = new Mock<IHubCallerClients>();

            var configurationMock = new Mock<IConfiguration>();

            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapperProfile(configurationMock.Object, _dataProtectionProviderMock.Object));
            }).CreateMapper();

            _chatroomService = new ChatroomService(_messageServiceMock.Object, _userManagerMock.Object, _mapper, _dataProtectionProviderMock.Object, _configurationMock.Object);
        }


        [TestMethod]
        public async Task Message_Test()
        {
            // Arrenge
            var content = "Max Versappen 2021 champ";

            _chatroomService.Clients = MockHubCallerClients();
            _chatroomService.Context = MockHubCallerContext();

            // Act
            await _chatroomService.PublishMessage(content);

            // Assert
            _userManagerMock.Verify(m => m.FindByNameAsync(_chatroomService.Context.User.Identity.Name));
            _hubCallerClientsMock.Verify(m => m.All);
        }

        private IHubCallerClients MockHubCallerClients()
        {
            var clientProxyMock = new Mock<IClientProxy>();


            _hubCallerClientsMock.SetupGet(h => h.All)
                .Returns(clientProxyMock.Object);

            return _hubCallerClientsMock.Object;
        }

        private HubCallerContext MockHubCallerContext()
        {
            var identityMock = new Mock<IIdentity>();
            var claimsMock = new Mock<ClaimsPrincipal>();
            claimsMock.Setup(m => m.Identity).Returns(identityMock.Object);
            var hubCallerContextMock = new Mock<HubCallerContext>();
            hubCallerContextMock.SetupGet(h => h.User)
                .Returns(claimsMock.Object);
            return hubCallerContextMock.Object;
        }

        private static Mock<UserManager<TUser>> MockUserManager<TUser>(List<TUser> ls) where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var manager = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            manager.Object.UserValidators.Add(new UserValidator<TUser>());
            manager.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

            manager.Setup(x => x.DeleteAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);
            manager.Setup(x => x.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Callback<TUser, string>((x, y) => ls.Add(x));
            manager.Setup(x => x.UpdateAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);

            return manager;
        }
    }
}

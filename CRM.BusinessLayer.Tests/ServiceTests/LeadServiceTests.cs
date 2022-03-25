using AutoMapper;
using CRM.BusinessLayer.Configurations;
using CRM.BusinessLayer.Exceptions;
using CRM.BusinessLayer.Models;
using CRM.BusinessLayer.Services;
using CRM.BusinessLayer.Tests.TestData;
using CRM.DataLayer.Entities;
using CRM.DataLayer.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace CRM.BusinessLayer.Tests.ServiceTests
{
    public class LeadServiceTests
    {
        private Mock<IAccountRepository> _accountRepositoryMock;
        private Mock<ILeadRepository> _leadRepositoryMock;
        private readonly LeadTestData _leadTestData;
        private readonly IMapper _autoMapper;
        private readonly Mock<ILogger<LeadService>> _logger;

        public LeadServiceTests()
        {
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _leadRepositoryMock = new Mock<ILeadRepository>();
            _leadTestData = new LeadTestData();
            _autoMapper = new Mapper(
                new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperToData>()));
            _logger = new Mock<ILogger<LeadService>>();
        }

        [SetUp]
        public void Setup()
        {
            _leadRepositoryMock = new Mock<ILeadRepository>();
        }

        [Test]
        public async Task AddLeadTestAsync()
        {
            //given
            var leadModel = _leadTestData.GetLeadModelForTests();
            var emails = _leadTestData.GetListOfEmailsForTests();
            _leadRepositoryMock.Setup(m => m.GetByEmail(It.IsAny<string>())).ReturnsAsync((Lead)null);
            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object, _logger.Object);

            //when
            sut.AddLead(leadModel);

            //then
            _leadRepositoryMock.Verify(m => m.AddLead(It.IsAny<Lead>()), Times.Once());
        }

        [Test]
        public async Task AddLeadNegativeTest()
        {
            //given
            var leadModel = _leadTestData.GetLeadModelForTests();
            var lead = new Lead();
            _leadRepositoryMock.Setup(m => m.GetByEmail(It.IsAny<string>())).ReturnsAsync(lead);
            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object, _logger.Object);

            //then
            Assert.ThrowsAsync<DuplicationException>(async () => await sut.AddLead(leadModel));
        }

        [Test]
        public async Task UpdateLeadTest()
        {
            //given
            var lead = new Lead();
            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).ReturnsAsync(lead);
            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object, _logger.Object);

            //when
            sut.UpdateLead(It.IsAny<int>(), new LeadModel());

            //then
            _leadRepositoryMock.Verify(m => m.GetById(It.IsAny<int>()), Times.Once());
            _leadRepositoryMock.Verify(m => m.UpdateLeadById(It.IsAny<Lead>()), Times.Once());
        }

        [Test]
        public async Task UpdateALeadtNegativeTest()
        {
            //given
            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).ReturnsAsync((Lead)null);
            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object, _logger.Object);

            //then
            Assert.ThrowsAsync<NotFoundException>(async () => await sut.UpdateLead(It.IsAny<int>(), new LeadModel()));
        }

        [Test]
        public async Task ChangeRoleLeadTest()
        {
            //given
            var lead = new Lead();
            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).ReturnsAsync(lead);
            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object, _logger.Object);

            //when
            sut.ChangeRoleLead(It.IsAny<int>(), 3);

            //then
            _leadRepositoryMock.Verify(m => m.GetById(It.IsAny<int>()), Times.Once());
            _leadRepositoryMock.Verify(m => m.ChangeRoleLead(It.IsAny<Lead>()), Times.Once());
        }

        [Test]
        public async Task ChangeRoleLeadNegativeTest_NotFoundException()
        {
            //given
            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).ReturnsAsync((Lead)null);
            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object, _logger.Object);

            //then
            Assert.ThrowsAsync<NotFoundException>(async () => await sut.ChangeRoleLead(It.IsAny<int>(), 3));
        }

        [Test]
        public async Task ChangeRoleLeadNegativeTest_IncorrectRoleException()
        {
            //given
            var lead = new Lead();
            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).ReturnsAsync(lead);
            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object, _logger.Object);

            //then
            Assert.ThrowsAsync<IncorrectRoleException>(async () => await sut.ChangeRoleLead(It.IsAny<int>(), 4));
        }

        [Test]
        public async Task DeleteByIdTest()
        {
            //given
            var lead = new Lead();
            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).ReturnsAsync(lead);
            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object, _logger.Object);

            //when
            sut.DeleteById(It.IsAny<int>());

            //then
            _leadRepositoryMock.Verify(m => m.GetById(It.IsAny<int>()), Times.Once());
            _leadRepositoryMock.Verify(m => m.DeleteById(It.IsAny<int>()), Times.Once());
        }

        [Test]
        public async Task DeleteByIdNegativeTest_NotFoundException()
        {
            //given
            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).ReturnsAsync((Lead)null);
            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object, _logger.Object);

            //then
            Assert.ThrowsAsync<NotFoundException>(async () => await sut.DeleteById(It.IsAny<int>()));
        }

        [Test]
        public async Task DeleteByIdNegativeTest_BannedException()
        {
            //given
            var lead = _leadTestData.GetLeadForTests();
            lead.IsBanned = true;
            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).ReturnsAsync(lead);
            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object, _logger.Object);

            //then
            Assert.ThrowsAsync<BannedException>(async () => await sut.DeleteById(It.IsAny<int>()));
        }

        [Test]
        public async Task RestoreByIdTest()
        {
            //given
            var lead = _leadTestData.GetLeadForTests();
            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).ReturnsAsync(lead);
            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object, _logger.Object);

            //when
            sut.RestoreById(It.IsAny<int>());

            //then
            _leadRepositoryMock.Verify(m => m.GetById(It.IsAny<int>()), Times.Once());
            _leadRepositoryMock.Verify(m => m.RestoreById(It.IsAny<int>()), Times.Once());
        }

        [Test]
        public async Task RestoreByIdNegativeTest_NotFoundException()
        {
            //given
            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).ReturnsAsync((Lead)null);
            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object, _logger.Object);

            //then
            Assert.ThrowsAsync<NotFoundException>(async () => await sut.RestoreById(It.IsAny<int>()));
        }

        [Test]
        public async Task RestoreByIdNegativeTest_BannedException()
        {
            //given
            var lead = _leadTestData.GetLeadForTests();
            lead.IsBanned = false;
            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).ReturnsAsync(lead);
            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object, _logger.Object);

            //then
            Assert.ThrowsAsync<BannedException>(async () => await sut.RestoreById(It.IsAny<int>()));
        }

        [Test]
        public async Task GetAllTest()
        {
            //given
            var leads = _leadTestData.GetListOfLeadsForTests();
            _leadRepositoryMock.Setup(m => m.GetAll()).ReturnsAsync(leads);
            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object, _logger.Object);

            //when
            var actual = sut.GetAll().Result;

            //then
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Count > 0);

            for (int i = 0; i < actual.Count; i++)
            {
                Assert.IsNotNull(actual[i].Id);
                Assert.IsNotNull(actual[i].Name);
                Assert.IsNotNull(actual[i].LastName);
                Assert.IsNotNull(actual[i].IsBanned);
                Assert.IsNotNull(actual[i].Accounts);
                Assert.IsTrue(actual[i].Accounts.Count > 0);
                Assert.IsNotNull(actual[i].Email);
                Assert.IsNotNull(actual[i].Password);
                Assert.IsNotNull(actual[i].Role);

            }
        }

        [Test]
        public async Task GetByIdTest()
        {
            //given
            var lead = _leadTestData.GetLeadForTests();
            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).ReturnsAsync(lead);
            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object, _logger.Object);

            //when
            var actual = sut.GetById(It.IsAny<int>()).Result;

            //then
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Id);
            Assert.IsNotNull(actual.Name);
            Assert.IsNotNull(actual.LastName);
            Assert.IsNotNull(actual.IsBanned);
            Assert.IsNotNull(actual.Accounts);
            Assert.IsTrue(actual.Accounts.Count > 0);
            Assert.IsNotNull(actual.Email);
            Assert.IsNotNull(actual.Password);
            Assert.IsNotNull(actual.Role);

        }

        [Test]
        public async Task GetByIdNegativeTest()
        {
            //given
            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).ReturnsAsync((Lead)null);
            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object, _logger.Object);

            //then
            Assert.ThrowsAsync<NotFoundException>(async () => await sut.GetById(It.IsAny<int>()));
        }

        [Test]
        public async Task ChangePasswordTest()
        {
            //given
            var lead = _leadTestData.GetLeadForTests();
            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).ReturnsAsync(lead);
            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object, _logger.Object);

            //when
            sut.ChangePassword(It.IsAny<int>(), "qert123", "1234567");

            //then
            _leadRepositoryMock.Verify(m => m.GetById(It.IsAny<int>()), Times.Once());
            _leadRepositoryMock.Verify(m => m.ChangePassword(It.IsAny<int>(), It.IsAny<string>()), Times.Once());
        }

        [Test]
        public async Task ChangePasswordNegativeTest_NotFoundException()
        {
            //given
            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).ReturnsAsync((Lead)null);
            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object, _logger.Object);

            //then
            Assert.ThrowsAsync<NotFoundException>(async () => await sut.ChangePassword(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()));
        }

        [Test]
        public async Task ChangePasswordNegativeTest_IncorrectPasswordException()
        {
            //given
            var lead = _leadTestData.GetLeadForTests();
            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).ReturnsAsync(lead);
            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object, _logger.Object);

            //then
            Assert.ThrowsAsync<IncorrectPasswordException>(async () => await sut.ChangePassword(It.IsAny<int>(), "neverny", It.IsAny<string>()));
        }

    }
}

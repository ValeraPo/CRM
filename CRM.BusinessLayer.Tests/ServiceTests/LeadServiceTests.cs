//using AutoMapper;
//using CRM.BusinessLayer.Configurations;
//using CRM.BusinessLayer.Exceptions;
//using CRM.BusinessLayer.Models;
//using CRM.BusinessLayer.Services;
//using CRM.BusinessLayer.Tests.TestData;
//using CRM.DataLayer.Entities;
//using CRM.DataLayer.Repositories.Interfaces;
//using Microsoft.Extensions.Logging;
//using Moq;
//using NUnit.Framework;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace CRM.BusinessLayer.Tests.ServiceTests
//{
//    public class LeadServiceTests
//    {
//        private Mock<IAccountRepository> _accountRepositoryMock;
//        private Mock<ILeadRepository> _leadRepositoryMock;
//        private readonly LeadTestData _leadTestData;
//        private readonly IMapper _autoMapper;
//        private readonly Mock<ILogger<LeadService>> _logger;

//        public LeadServiceTests()
//        {
//            _accountRepositoryMock = new Mock<IAccountRepository>();
//            _leadRepositoryMock = new Mock<ILeadRepository>();
//            _leadTestData = new LeadTestData();
//            _autoMapper = new Mapper(
//                new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperToData>()));
//            _logger = new Mock<ILogger<LeadService>>();
//        }

//        [SetUp]
//        public void Setup()
//        {
//            _leadRepositoryMock = new Mock<ILeadRepository>();
//        }

//        [Test]
//        public async Task AddLeadTestAsync()
//        {
//            //given
//            var leadModel = _leadTestData.GetLeadModelForTests();
//            var emails = _leadTestData.GetListOfEmailsForTests();
//            await _leadRepositoryMock.Setup(m => m.GetAllEmails()).Returns(emails);
//            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object, _logger.Object);

//            //when
//            sut.AddLead(leadModel);

//            //then
//            _leadRepositoryMock.Verify(m => m.AddLead(It.IsAny<Lead>()), Times.Once());
//        }

//        [Test]
//        public void AddLeadNegativeTest()
//        {
//            //given
//            var leadModel = _leadTestData.GetLeadModelForTests();
//            var emails = _leadTestData.GetListOfEmailsForTests();
//            emails.Add(leadModel.Email);
//            _leadRepositoryMock.Setup(m => m.GetAllEmails()).Returns(emails);
//            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object, _logger.Object);

//            //then
//            Assert.Throws<DuplicationException>(() => sut.AddLead(leadModel));
//        }

//        [Test]
//        public void UpdateLeadTest()
//        {
//            //given
//            var lead = new Lead();
//            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).Returns(lead);
//            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object, _logger.Object);

//            //when
//            sut.UpdateLead(It.IsAny<int>(), new LeadModel());

//            //then
//            _leadRepositoryMock.Verify(m => m.GetById(It.IsAny<int>()), Times.Once());
//            _leadRepositoryMock.Verify(m => m.UpdateLeadById(It.IsAny<Lead>()), Times.Once());
//        }

//        [Test]
//        public void UpdateALeadtNegativeTest()
//        {
//            //given
//            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).Returns((Lead)null);
//            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object, _logger.Object);

//            //then
//            Assert.Throws<NotFoundException>(() => sut.UpdateLead(It.IsAny<int>(), new LeadModel()));
//        }

//        [Test]
//        public void ChangeRoleLeadTest()
//        {
//            //given
//            var lead = new Lead();
//            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).Returns(lead);
//            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object, _logger.Object);

//            //when
//            sut.ChangeRoleLead(It.IsAny<int>(), 3);

//            //then
//            _leadRepositoryMock.Verify(m => m.GetById(It.IsAny<int>()), Times.Once());
//            _leadRepositoryMock.Verify(m => m.ChangeRoleLead(It.IsAny<Lead>()), Times.Once());
//        }

//        [Test]
//        public void ChangeRoleLeadNegativeTest_NotFoundException()
//        {
//            //given
//            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).Returns((Lead)null);
//            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object, _logger.Object);

//            //then
//            Assert.Throws<NotFoundException>(() => sut.ChangeRoleLead(It.IsAny<int>(), 3));
//        }

//        [Test]
//        public void ChangeRoleLeadNegativeTest_IncorrectRoleException()
//        {
//            //given
//            var lead = new Lead();
//            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).Returns(lead);
//            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object, _logger.Object);

//            //then
//            Assert.Throws<IncorrectRoleException>(() => sut.ChangeRoleLead(It.IsAny<int>(), 4));
//        }

//        [Test]
//        public void DeleteByIdTest()
//        {
//            //given
//            var lead = new Lead();
//            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).Returns(lead);
//            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object, _logger.Object);

//            //when
//            sut.DeleteById(It.IsAny<int>());

//            //then
//            _leadRepositoryMock.Verify(m => m.GetById(It.IsAny<int>()), Times.Once());
//            _leadRepositoryMock.Verify(m => m.DeleteById(It.IsAny<int>()), Times.Once());
//        }

//        [Test]
//        public void DeleteByIdNegativeTest_NotFoundException()
//        {
//            //given
//            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).Returns((Lead)null);
//            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object, _logger.Object);

//            //then
//            Assert.Throws<NotFoundException>(() => sut.DeleteById(It.IsAny<int>()));
//        }

//        [Test]
//        public void DeleteByIdNegativeTest_BannedException()
//        {
//            //given
//            var lead = _leadTestData.GetLeadForTests();
//            lead.IsBanned = true;
//            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).Returns(lead);
//            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object, _logger.Object);

//            //then
//            Assert.Throws<BannedException>(() => sut.DeleteById(It.IsAny<int>()));
//        }

//        [Test]
//        public void RestoreByIdTest()
//        {
//            //given
//            var lead = _leadTestData.GetLeadForTests();
//            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).Returns(lead);
//            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object, _logger.Object);

//            //when
//            sut.RestoreById(It.IsAny<int>());

//            //then
//            _leadRepositoryMock.Verify(m => m.GetById(It.IsAny<int>()), Times.Once());
//            _leadRepositoryMock.Verify(m => m.RestoreById(It.IsAny<int>()), Times.Once());
//        }

//        [Test]
//        public void RestoreByIdNegativeTest_NotFoundException()
//        {
//            //given
//            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).Returns((Lead)null);
//            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object, _logger.Object);

//            //then
//            Assert.Throws<NotFoundException>(() => sut.RestoreById(It.IsAny<int>()));
//        }

//        [Test]
//        public void RestoreByIdNegativeTest_BannedException()
//        {
//            //given
//            var lead = _leadTestData.GetLeadForTests();
//            lead.IsBanned = false;
//            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).Returns(lead);
//            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object, _logger.Object);

//            //then
//            Assert.Throws<BannedException>(() => sut.RestoreById(It.IsAny<int>()));
//        }

//        [Test]
//        public void GetAllTest()
//        {
//            //given
//            var leads = _leadTestData.GetListOfLeadsForTests();
//            _leadRepositoryMock.Setup(m => m.GetAll()).Returns(leads);
//            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object, _logger.Object);

//            //when
//            var actual = sut.GetAll();

//            //then
//            Assert.IsNotNull(actual);
//            Assert.IsTrue(actual.Count > 0);

//            for (int i = 0; i < actual.Count; i++)
//            {
//                Assert.IsNotNull(actual[i].Id);
//                Assert.IsNotNull(actual[i].Name);
//                Assert.IsNotNull(actual[i].LastName);
//                Assert.IsNotNull(actual[i].IsBanned);
//                Assert.IsNotNull(actual[i].Accounts);
//                Assert.IsTrue(actual[i].Accounts.Count > 0);
//                Assert.IsNotNull(actual[i].Email);
//                Assert.IsNotNull(actual[i].Password);
//                Assert.IsNotNull(actual[i].Role);

//            }
//        }

//        [Test]
//        public void GetByIdTest()
//        {
//            //given
//            var lead = _leadTestData.GetLeadForTests();
//            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).Returns(lead);
//            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object, _logger.Object);

//            //when
//            var actual = sut.GetById(It.IsAny<int>());

//            //then
//            Assert.IsNotNull(actual);
//            Assert.IsNotNull(actual.Id);
//            Assert.IsNotNull(actual.Name);
//            Assert.IsNotNull(actual.LastName);
//            Assert.IsNotNull(actual.IsBanned);
//            Assert.IsNotNull(actual.Accounts);
//            Assert.IsTrue(actual.Accounts.Count > 0);
//            Assert.IsNotNull(actual.Email);
//            Assert.IsNotNull(actual.Password);
//            Assert.IsNotNull(actual.Role);

//        }

//        [Test]
//        public void GetByIdNegativeTest()
//        {
//            //given
//            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).Returns((Lead)null);
//            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object, _logger.Object);

//            //then
//            Assert.Throws<NotFoundException>(() => sut.GetById(It.IsAny<int>()));
//        }

//        [Test]
//        public void ChangePasswordTest()
//        {
//            //given
//            var lead = _leadTestData.GetLeadForTests();
//            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).Returns(lead);
//            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object, _logger.Object);

//            //when
//            sut.ChangePassword(It.IsAny<int>(), "qert123", "1234567");

//            //then
//            _leadRepositoryMock.Verify(m => m.GetById(It.IsAny<int>()), Times.Once());
//            _leadRepositoryMock.Verify(m => m.ChangePassword(It.IsAny<int>(), It.IsAny<string>()), Times.Once());
//        }

//        [Test]
//        public void ChangePasswordNegativeTest_NotFoundException()
//        {
//            //given
//            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).Returns((Lead)null);
//            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object, _logger.Object);

//            //then
//            Assert.Throws<NotFoundException>(() => sut.ChangePassword(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()));
//        }

//        [Test]
//        public void ChangePasswordNegativeTest_IncorrectPasswordException()
//        {
//            //given
//            var lead = _leadTestData.GetLeadForTests();
//            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).Returns(lead);
//            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object, _logger.Object);

//            //then
//            Assert.Throws<IncorrectPasswordException>(() => sut.ChangePassword(It.IsAny<int>(), "neverny", It.IsAny<string>()));
//        }

//    }
//}

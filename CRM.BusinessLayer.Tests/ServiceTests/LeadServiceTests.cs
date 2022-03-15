using AutoMapper;
using CRM.BusinessLayer.Configurations;
using CRM.BusinessLayer.Exceptions;
using CRM.BusinessLayer.Models;
using CRM.BusinessLayer.Services;
using CRM.BusinessLayer.Tests.TestData;
using CRM.DataLayer.Entities;
using CRM.DataLayer.Repositories.Interfaces;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace CRM.BusinessLayer.Tests.ServiceTests
{
    public class LeadServiceTests
    {
        private Mock<IAccountRepository> _accountRepositoryMock;
        private Mock<ILeadRepository> _leadRepositoryMock;
        private readonly LeadTestData _leadTestData;
        private readonly IMapper _autoMapper;

        public LeadServiceTests()
        {
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _leadRepositoryMock = new Mock<ILeadRepository>();
            _leadTestData = new LeadTestData();
            _autoMapper = new Mapper(
                new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperToData>()));
        }

        [SetUp]
        public void Setup()
        {
            _leadRepositoryMock = new Mock<ILeadRepository>();
        }

        [Test]
        public void AddLeadTest()
        {
            //given
            var leadModel = _leadTestData.GetLeadModelForTests();
            var leads = new List<Lead>();
            _leadRepositoryMock.Setup(a => a.AddLead(It.IsAny<Lead>())).Returns(23);
            _leadRepositoryMock.Setup(m => m.GetAll()).Returns(leads);
            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object);

            //when
            sut.AddLead(leadModel);

            //then
            _leadRepositoryMock.Verify(m => m.AddLead(It.IsAny<Lead>()), Times.Once());
        }

        [Test]
        public void UpdateLeadTest()
        {
            //given
            var lead = new Lead();
            _leadRepositoryMock.Setup(m => m.UpdateLeadById(It.IsAny<Lead>()));
            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).Returns(lead);
            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object);

            //when
            sut.UpdateLead(It.IsAny<int>(), new LeadModel());

            //then
            _leadRepositoryMock.Verify(m => m.GetById(It.IsAny<int>()), Times.Once());
            _leadRepositoryMock.Verify(m => m.UpdateLeadById(It.IsAny<Lead>()), Times.Once());
        }

        [Test]
        public void UpdateALeadtNegativeTest()
        {
            //given
            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).Returns((Lead)null);
            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object);

            //then
            Assert.Throws<NotFoundException>(() => sut.UpdateLead(It.IsAny<int>(), new LeadModel()));
        }

        [Test]
        public void DeleteByIdTest()
        {
            //given
            var lead = new Lead();
            _leadRepositoryMock.Setup(m => m.DeleteById(It.IsAny<int>()));
            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).Returns(lead);
            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object);

            //when
            sut.DeleteById(It.IsAny<int>());

            //then
            _leadRepositoryMock.Verify(m => m.GetById(It.IsAny<int>()), Times.Once());
            _leadRepositoryMock.Verify(m => m.DeleteById(It.IsAny<int>()), Times.Once());
        }

        [Test]
        public void DeleteByIdNegativeTest()
        {
            //given
            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).Returns((Lead)null);
            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object);

            //then
            Assert.Throws<NotFoundException>(() => sut.DeleteById(It.IsAny<int>()));
        }

        [Test]
        public void RestoreByIdTest()
        {
            //given
            var lead = new Lead();
            _leadRepositoryMock.Setup(m => m.RestoreById(It.IsAny<int>()));
            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).Returns(lead);
            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object);

            //when
            sut.RestoreById(It.IsAny<int>());

            //then
            _leadRepositoryMock.Verify(m => m.GetById(It.IsAny<int>()), Times.Once());
            _leadRepositoryMock.Verify(m => m.RestoreById(It.IsAny<int>()), Times.Once());
        }

        [Test]
        public void RestoreByIdNegativeTest()
        {
            //given
            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).Returns((Lead)null);
            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object);

            //then
            Assert.Throws<NotFoundException>(() => sut.RestoreById(It.IsAny<int>()));
        }

        [Test]
        public void GetAllTest()
        {
            //given
            var leads = _leadTestData.GetListOfLeadsForTests();
            _leadRepositoryMock.Setup(m => m.GetAll()).Returns(leads);
            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object);

            //when
            var actual = sut.GetAll();

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
        public void GetByIdTest()
        {
            //given
            var lead = _leadTestData.GetLeadForTests();
            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).Returns(lead);
            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object);

            //when
            var actual = sut.GetById(It.IsAny<int>());

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
        public void GetByIdNegativeTest()
        {
            //given
            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).Returns((Lead)null);
            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object);

            //then
            Assert.Throws<NotFoundException>(() => sut.GetById(It.IsAny<int>()));
        }

        [Test]
        public void ChangePasswordTest()
        {
            //given
            var lead = _leadTestData.GetLeadForTests();
            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).Returns(lead);
            _leadRepositoryMock.Setup(m => m.ChangePassword(It.IsAny<int>(), It.IsAny<string>()));

            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object);

            //when
            sut.ChangePassword(It.IsAny<int>(), "qert123", "1234567");

            //then
            _leadRepositoryMock.Verify(m => m.GetById(It.IsAny<int>()), Times.Once());
            _leadRepositoryMock.Verify(m => m.ChangePassword(It.IsAny<int>(), It.IsAny<string>()), Times.Once());
        }

        [Test]
        public void ChangePasswordNegativeTest_NotFoundException()
        {
            //given
            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).Returns((Lead)null);
            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object);

            //then
            Assert.Throws<NotFoundException>(() => sut.ChangePassword(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()));
        }

        [Test]
        public void ChangePasswordNegativeTest_IncorrectPasswordException()
        {
            //given
            var lead = _leadTestData.GetLeadForTests();
            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).Returns(lead);
            _leadRepositoryMock.Setup(m => m.ChangePassword(It.IsAny<int>(), It.IsAny<string>()));
            var sut = new LeadService(_autoMapper, _leadRepositoryMock.Object, _accountRepositoryMock.Object);

            //then
            Assert.Throws<IncorrectPasswordException>(() => sut.ChangePassword(It.IsAny<int>(), "neverny", It.IsAny<string>()));
        }

    }
}

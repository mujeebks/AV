using System;
using AVD.Core.Planning;
using AVD.DataAccessLayer;
using AVD.DataAccessLayer.Enums;
using AVD.DataAccessLayer.Models;
using AVD.DataAccessLayer.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AVD.UnitTests.PlanningTool.Activity
{
    [TestClass]
    public class EntityTests
    {
        [TestMethod]
        public void InitialTestMethod()
        {

            var planningManager = new PlanningManager();
            var staticInitialTestMethod = planningManager.StaticInitialTestMethod();

            var mockQuoteRepo = new Mock<IRepository<MD_EntityType>>();

            var entityType = new MD_EntityType
            {
                Caption = "Test Type"
            };

            mockQuoteRepo.Setup(x => x.GetByID(It.IsAny<int>())).Returns(entityType);

            var manager = new PlanningManager(mockQuoteRepo.Object);

            var result = manager.StaticInitialTestMethod();
            
            Assert.AreEqual("Initial Test Method_EMRM", result);
        }
    }
}

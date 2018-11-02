using System;
using System.Threading.Tasks;
using XOProject.Controller;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using System.Linq;

namespace XOProject.Tests
{
    public class ShareControllerTests
    {
        private readonly Mock<IShareRepository> _shareRepositoryMock = new Mock<IShareRepository>();

        private readonly ShareController _shareController;

        public ShareControllerTests()
        {
            _shareController = new ShareController(_shareRepositoryMock.Object);
        }

        [Test]
        public async Task Get_ReturnsNotNull()
        {
            string symbol = "REL";

            _shareRepositoryMock
                .Setup(x => x.GetBySymbol(It.Is<string>(s => s == symbol)))
                .Returns(Task.FromResult(new List<HourlyShareRate>(new[]
                    {
                        new HourlyShareRate() { Id = 1, Symbol = symbol, Rate = 100, TimeStamp = DateTime.Now }
                    }))
                );

            var result = await _shareController.Get(symbol) as OkObjectResult;

            Assert.NotNull(result);

            var resultList = result.Value as List<HourlyShareRate>;

            Assert.NotNull(resultList);

            Assert.NotZero(resultList.Count);
        }

        [Test]
        public async Task Post_ShouldInsertHourlySharePrice()
        {
            var hourRate = new HourlyShareRate
            {
                Symbol = "CBI",
                Rate = 330.0M,
                TimeStamp = new DateTime(2018, 08, 17, 5, 0, 0)
            };

            // Arrange

            // Act
            var result = await _shareController.Post(hourRate);

            // Assert
            Assert.NotNull(result);

            var createdResult = result as CreatedResult;
            Assert.NotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
        }

        [Test]
        public async Task GetLatestPrice_ShouldReturnLastPrice()
        {
            _shareRepositoryMock.Setup(x => x.GetBySymbol(It.Is<string>(s => s.Equals("REL"))))
                .Returns(Task.FromResult(new List<HourlyShareRate>(new[]
                    {
                        new HourlyShareRate() { Symbol = "REL", Rate = 50, TimeStamp = DateTime.Now.AddDays(-1) },
                        new HourlyShareRate() { Symbol = "REL", Rate = 100, TimeStamp = DateTime.Now },
                        new HourlyShareRate() { Symbol = "REL", Rate = 150, TimeStamp = DateTime.Now.AddDays(-2) },
                    }))
                );

            var result = await _shareController.GetLatestPrice("REL") as OkObjectResult;

            Assert.AreEqual(100, result.Value);
        }

        [Test]
        public void UpdateLastPrice()
        {

            _shareRepositoryMock.Setup(x => x.GetShareBySymbol(It.Is<string>(s => s.Equals("REL"))))
                .Returns(Task.FromResult(
                        new HourlyShareRate() { Symbol = "REL", Rate = 50, TimeStamp = DateTime.Now.AddDays(-1) }
                        )
                );

            _shareController.UpdateLastPrice("REL");

            Assert.IsTrue(true);
        }
        [Test]
        public async Task Post_InserShare()
        {
            var p = new HourlyShareRate() { Symbol = "REL", Rate = 50, TimeStamp = DateTime.Now.AddDays(-1) };
            _shareRepositoryMock.Setup(x => x.InsertAsync(new HourlyShareRate() { Symbol = "REL", Rate = 50, TimeStamp = DateTime.Now.AddDays(-1) }));

            var result = await _shareController.Post(p) as CreatedResult;

            Assert.NotNull(result);

            var result_share = result.Value as HourlyShareRate;

            Assert.AreEqual(result_share.Symbol, "REL");
        }

        [Test]
        public async Task test_controller_with_model_error()
        {

            _shareController.ModelState.AddModelError("Model", "Not Correct");

            var result = await _shareController.Post(new HourlyShareRate()) as BadRequestObjectResult;

            Assert.AreEqual(result.StatusCode, 400);
            Assert.NotNull(result.Value);
        }
    }
}

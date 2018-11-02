using XOProject.Controller;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace XOProject.Tests
{
   public class PortfolioControllerTests
    {

        
        [Test]
        public async Task GetPortfolioInfo_ReturnsNotNull()
        {
            int portfolioId = 1;

            var portfolioRepositoryMock = new Mock<IPortfolioRepository>();

            portfolioRepositoryMock
                .Setup(x => x.GetAsync(It.Is<int>(id => id == portfolioId)))
                .Returns(Task.FromResult(new Portfolio()
                {
                    Id = portfolioId,
                    Name = "John Doe",
                    Trade = new System.Collections.Generic.List<Trade> {
                    new Trade() {Id = 1, Action = "BUY", NoOfShares = 120, PortfolioId = portfolioId, Price = 12000, Symbol = "REL" },
                    new Trade() {Id = 2, Action = "SELL", NoOfShares = 40, PortfolioId = portfolioId, Price = 4000, Symbol = "REL" }
                    }
                }));

            var portfolioController = new PortfolioController(portfolioRepositoryMock.Object);

            var result = await portfolioController.GetPortfolioInfo(portfolioId) as OkObjectResult;

            Assert.NotNull(result);

            var resultPortfolio = result.Value as Portfolio;

            Assert.NotNull(resultPortfolio);

            Assert.AreEqual(portfolioId, resultPortfolio.Id);
            Assert.NotZero(resultPortfolio.Trade.Count);
        }

        [Test]
        public async Task Post_ShouldInsertPortfolio()
        {
            var portfolioRepositoryMock = new Mock<IPortfolioRepository>();

            var portfolioController = new PortfolioController(portfolioRepositoryMock.Object);

            var portfolio = new Portfolio()
            {
                Name = "John Smith"
            };

            // Act
            var result = await portfolioController.Post(portfolio);

            // Assert
            Assert.NotNull(result);

            var createdResult = result as CreatedResult;
            Assert.NotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
        }
        [Test]
        public async Task Post_BadInsertPortfolioRequest()
        {
            var portfolioRepositoryMock = new Mock<IPortfolioRepository>();

            var portfolioController = new PortfolioController(portfolioRepositoryMock.Object);

            var portfolio = new Portfolio();

            portfolioController.ModelState.AddModelError("Model", "Not Correct");
            // Act
            var result = await portfolioController.Post(portfolio) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);

            var createdResult = result as BadRequestObjectResult;

            Assert.AreEqual(result.StatusCode, 400);
            Assert.NotNull(result.Value);



        }

        [Test]
        public void TradeRepository_GetAllTest()
        {
            var optionsbuilder = new DbContextOptionsBuilder<ExchangeContext>();
            optionsbuilder.UseInMemoryDatabase(databaseName: "XOProjectDb");
            var _dbContext = new ExchangeContext(optionsbuilder.Options);
            IPortfolioRepository _portfolioRepository = new PortfolioRepository(_dbContext);
            var result = _portfolioRepository.GetAll();

            Assert.NotNull(result);

           
        }
       
    }
}

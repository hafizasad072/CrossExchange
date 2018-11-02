using XOProject.Controller;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace XOProject.Tests
{
    class TradeControllerTests
    {
        Mock<IShareRepository> _shareRepositoryMock = new Mock<IShareRepository>();
        Mock<ITradeRepository> _tradeRepositoryMock = new Mock<ITradeRepository>();
        Mock<IPortfolioRepository> _portfolioRepositoryMock = new Mock<IPortfolioRepository>();
        Mock<IGenericRepository<Trade>> _genericRepositoryMock = new Mock<IGenericRepository<Trade>>();

        int portfolioId = 1;
        string symbol = "REL";
        public TradeControllerTests()
        {

            _tradeRepositoryMock.Setup(x => x.GetAllTradings(portfolioId))
                .Returns(Task.FromResult(new List<Trade>(new[]
                    {
                        new Trade() {Id = 1, Action = "BUY", NoOfShares = 120, PortfolioId = portfolioId, Price = 12000, Symbol = "REL" },
                        new Trade() {Id = 2, Action = "SELL", NoOfShares = 40, PortfolioId = portfolioId, Price = 4000, Symbol = "REL" }
                    }))
                );

            _tradeRepositoryMock.Setup(x => x.GetAnalysis(symbol))
                .Returns(Task.FromResult(new List<TradeAnalysis>(new[]
                    {
                        new TradeAnalysis() { Action = "BUY", Average = 120, Sum = 12000, Maximum = 12000, Minimum = 0 },
                        new TradeAnalysis() { Action = "SELL", Average = 40, Sum = 100, Maximum = 90,  Minimum = 10 }
                    }))
                );
        }

        [Test]
        public async Task Get_ShouldReturnAllTradings()
        {
            var tradeController = new TradeController(_shareRepositoryMock.Object, _tradeRepositoryMock.Object, _portfolioRepositoryMock.Object);

            var result = await tradeController.GetAllTradings(portfolioId) as OkObjectResult;

            Assert.NotNull(result);

            var resultList = result.Value as List<Trade>;

            Assert.NotNull(result);

            Assert.NotZero(resultList.Count);
            Assert.AreEqual(resultList.FirstOrDefault().Id, 1);
        }

        [Test]
        public async Task GetAnalysis()
        {
            var tradeController = new TradeController(_shareRepositoryMock.Object, _tradeRepositoryMock.Object, _portfolioRepositoryMock.Object);

            var result = await tradeController.GetAnalysis(symbol) as OkObjectResult;

            Assert.NotNull(result);

            var resultList = result.Value as List<TradeAnalysis>;

            Assert.NotNull(result);

            Assert.NotZero(resultList.Count);
        }

        [Test]
        public void UseHttpStatusCodeExceptionMiddleware()
        {
            Mock<IApplicationBuilder> _app = new Mock<IApplicationBuilder>();
            _app.Object.UseHttpStatusCodeExceptionMiddleware();
            Assert.IsTrue(true);
        }

        [Test]
        public async Task TradeRepository_GetAsyncTest()
        {
            var optionsbuilder = new DbContextOptionsBuilder<ExchangeContext>();
            optionsbuilder.UseInMemoryDatabase(databaseName: "XOProjectDb");
            var _dbContext = new ExchangeContext(optionsbuilder.Options);
            ITradeRepository _tradeRepository = new TradeRepository(_dbContext);
            var result = await _tradeRepository.GetAllTradings(1);

            Assert.NotNull(result);


        }
    }
}

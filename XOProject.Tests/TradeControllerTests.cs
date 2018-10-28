using XOProject.Controller;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XOProject.Tests
{
	class TradeControllerTests
	{
		Mock<IShareRepository> _shareRepositoryMock = new Mock<IShareRepository>();
		Mock<ITradeRepository> _tradeRepositoryMock = new Mock<ITradeRepository>();
		Mock<IPortfolioRepository> _portfolioRepositoryMock = new Mock<IPortfolioRepository>();

		int portfolioId = 1;
        string symbol = "REL";
		public TradeControllerTests()
		{
			_portfolioRepositoryMock
				.Setup(x => x.GetAsync(It.Is<int>(id => id == portfolioId)))
				.Returns<int>(x => Task.FromResult(new Portfolio() { Id = portfolioId, Name = "John Doe" }));

			_shareRepositoryMock.Setup(x => x.GetBySymbol(It.Is<string>(s => s.Equals("REL"))))
				.Returns(Task.FromResult(new List<HourlyShareRate>(new[]
					{
						new HourlyShareRate() { Symbol = "REL", Rate = 50, TimeStamp = DateTime.Now.AddDays(-1) },
						new HourlyShareRate() { Symbol = "REL", Rate = 100, TimeStamp = DateTime.Now },
						new HourlyShareRate() { Symbol = "REL", Rate = 150, TimeStamp = DateTime.Now.AddDays(-2) },
					}))
				);

			_tradeRepositoryMock.Setup(x => x.GetAllTradings(portfolioId))
				.Returns(Task.FromResult(new List<Trade>(new[]
					{
						new Trade() { Action = "BUY", NoOfShares = 120, PortfolioId = portfolioId, Price = 12000, Symbol = "REL" },
						new Trade() { Action = "SELL", NoOfShares = 40, PortfolioId = portfolioId, Price = 4000, Symbol = "REL" }
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
    }
}

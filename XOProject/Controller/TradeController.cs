using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace XOProject.Controller
{
    [Route("api/Trade")]
    public class TradeController : ControllerBase
    {
        private IShareRepository _shareRepository { get; set; }
        private ITradeRepository _tradeRepository { get; set; }
        private IPortfolioRepository _portfolioRepository { get; set; }

        public TradeController(IShareRepository shareRepository, ITradeRepository tradeRepository, IPortfolioRepository portfolioRepository)
        {
            _shareRepository = shareRepository;
            _tradeRepository = tradeRepository;
            _portfolioRepository = portfolioRepository;
        }

        [HttpGet("{portfolioId}")]
        public async Task<IActionResult> GetAllTradings([FromRoute]int portfolioId)
        {
            var trade = await _tradeRepository.GetAllTradings(portfolioId);
            return Ok(trade);
        }



        /// <summary>
        /// For a given symbol of share, get the statistics for that particular share calculating the maximum, minimum, average and Sum of all the trades that happened for that share. 
        /// Group statistics individually for all BUY trades and SELL trades separately.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>

        [HttpGet("Analysis/{symbol}")]
        public async Task<IActionResult> GetAnalysis([FromRoute]string symbol)
        {
            var list = await _tradeRepository.GetAnalysis(symbol);
            return Ok(list);
        }
    }
}

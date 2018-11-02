using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XOProject
{
    public class TradeRepository : GenericRepository<Trade>, ITradeRepository
    {
        public TradeRepository(ExchangeContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<Trade>> GetAllTradings(int portFolioId)
        {
            return Query().Where(x => x.PortfolioId.Equals(portFolioId)).ToListAsync();
        }
        public Task<List<TradeAnalysis>> GetAnalysis(string symbol)
        {
            return Query().Where(a => a.Symbol == symbol).GroupBy(a => a.Action).Select(g => new TradeAnalysis
            {
                Action = g.FirstOrDefault().Action,
                Average = g.Average(m => m.Price / m.NoOfShares),
                Maximum = g.Max(m => m.Price / m.NoOfShares),
                Minimum = g.Min(m => m.Price / m.NoOfShares),
                Sum = g.Sum(m => m.Price / m.NoOfShares)
            })
            .ToListAsync();
        }
    }
}
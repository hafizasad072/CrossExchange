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
                Average = g.Average(x => x.NoOfShares * x.Price),
                Maximum = g.Max(m => m.NoOfShares * m.Price),
                Minimum = g.Min(m => m.NoOfShares * m.Price),
                Sum = g.Sum(m => m.NoOfShares * m.Price)
            })
            .ToListAsync();
        }
    }
}
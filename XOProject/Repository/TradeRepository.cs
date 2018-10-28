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
            //return Query().Where(x => x.PortfolioId.Equals(portFolioId)).ToListAsync();
            throw new NotImplementedException();
        }
    }
}
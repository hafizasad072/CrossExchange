﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace XOProject.Controller
{
    [Route("api/Share")]
    public class ShareController : ControllerBase
    {
        public IShareRepository _shareRepository { get; set; }

        public ShareController(IShareRepository shareRepository)
        {
            _shareRepository = shareRepository;
        }

		[HttpPut("{symbol}")]
        public async void UpdateLastPrice([FromRoute]string symbol)
        {
            var share = await _shareRepository.Query().Where(x => x.Symbol.Equals(symbol)).OrderByDescending(x => x.Rate).FirstOrDefaultAsync();
            share.Rate =+ 10;
            await _shareRepository.UpdateAsync(share);
        }        

		[HttpGet("{symbol}")]
        public async Task<IActionResult> Get([FromRoute]string symbol)
        {
			var shares = await _shareRepository.GetBySymbol(symbol);
            return Ok(shares);
        }
		
        [HttpGet("{symbol}/Latest")]
        public async Task<IActionResult> GetLatestPrice([FromRoute]string symbol)
        {
            decimal? rate = (await _shareRepository.GetBySymbol(symbol))
				.OrderByDescending(x => x.TimeStamp)
				.FirstOrDefault()
				?.Rate;

			return Ok(rate);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]HourlyShareRate value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _shareRepository.InsertAsync(value);

            return Created($"Share/{value.Id}", value);
        }
    }
}

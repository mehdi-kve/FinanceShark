﻿using api.Data;
using api.Dtos.Stock;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public StockRepository(ApplicationDBContext context)
        {
            _dbContext = context;    
        }

        public Task<bool> StockExist(int id)
        {
            return _dbContext.Stocks.AnyAsync(s => s.Id == id);
        }


        public async Task<List<Stock>> GetAllAsync(QueryObject query)
        {
            var stocks = _dbContext.Stocks.Include(c => c.Comments).ThenInclude(a => a.AppUser).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.CompanyName))
            {
                stocks = stocks.Where(s => s.CompanyName.Contains(query.CompanyName));
            }

            if (!string.IsNullOrWhiteSpace(query.Symbol))
            {
                stocks = stocks.Where(s => s.Symbol.Contains(query.Symbol));
            }

            if (!string.IsNullOrWhiteSpace(query.SortBy)) 
            {
                if (query.SortBy.Equals("Symbol", StringComparison.OrdinalIgnoreCase)) 
                {
                    stocks = query.IsDecsending ? stocks.OrderByDescending(s => s.Symbol) : stocks.OrderBy(s => s.Symbol);
                }

                if (query.SortBy.Equals("CompanyName", StringComparison.OrdinalIgnoreCase))
                {
                    stocks = query.IsDecsending ? stocks.OrderByDescending(s => s.CompanyName) : stocks.OrderBy(s => s.CompanyName);
                }
            }

            var skipNumber = (query.PageNumber - 1) * query.PageSize;

            return await stocks.Skip(skipNumber).Take(query.PageSize).ToListAsync();
        }

        public async Task<Stock?> GetByIdAsync(int id)
        {
            var stock = await _dbContext.Stocks.Include(c => c.Comments).FirstOrDefaultAsync(s => s.Id == id);

            if (stock == null)
            {
                return null;
            }
            return stock;
        }

        public async Task<Stock> CreateAsync(Stock stockModel)
        {
            await _dbContext.Stocks.AddAsync(stockModel);
            await _dbContext.SaveChangesAsync();
            return stockModel;
        }

        public async Task<Stock?> UpdateAsync(int id, UpdateStockRequestDto stockDto)
        {
            var stockModel = await _dbContext.Stocks.FirstOrDefaultAsync(s => s.Id == id);

            if (stockModel == null)
            {
                return null;
            }

            stockModel.Symbol = stockDto.Symbol;
            stockModel.CompanyName = stockDto.CompanyName;
            stockModel.Purchase = stockDto.Purchase;
            stockModel.LastDiv = stockDto.LastDiv;
            stockModel.Industry = stockDto.Industry;
            stockModel.MarketCap = stockDto.MarketCap;

            await _dbContext.SaveChangesAsync();

            return stockModel;
        }

        public async Task<Stock?> DeleteAsync(int id)
        {
            var stockModel = await _dbContext.Stocks.FirstOrDefaultAsync(s => s.Id == id);
            if (stockModel == null)
            {
                return null;
            }
            _dbContext.Stocks.Remove(stockModel);
            await _dbContext.SaveChangesAsync();
            return stockModel;
        }

        public async Task<Stock?> GetBySymbolAsync(string symbol)
        {
            return await _dbContext.Stocks.FirstOrDefaultAsync(s => s.Symbol == symbol);
        }
    }
}

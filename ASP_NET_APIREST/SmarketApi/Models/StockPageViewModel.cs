using System.Collections.Generic;
using SmarketApi.Models;

namespace SmarketApi.ViewModels
{
    public class StockPageViewModel
    {
        public List<TblStock> Stocks { get; set; } = new();
        public List<TblStockMouvement> Mouvements { get; set; } = new();
    }
}
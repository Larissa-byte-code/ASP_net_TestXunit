using System.Collections.Generic;
using MonProjetMVC.Models;

namespace MonProjetMVC.ViewModels
{
    public class StockPageViewModel
    {
        public List<TblStock> Stocks { get; set; } = new();
        public List<TblStockMouvement> Mouvements { get; set; } = new();
    }
}
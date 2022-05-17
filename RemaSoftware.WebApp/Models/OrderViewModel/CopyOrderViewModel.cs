using RemaSoftware.Domain.ContextModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemaSoftware.WebApp.Models.OrderViewModel
{
    public class CopyOrderViewModel
    {
        public int OrderId { get; set; }
        public int NumberPiece { get; set; }
        public string Code_DDT { get; set; }

        public CopyOrderViewModel()
        {
            this.NumberPiece = NumberPiece;
            this.Code_DDT = Code_DDT;
        }
    }
}

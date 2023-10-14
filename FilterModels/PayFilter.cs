using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exel_for_mfc.FilterModels
{
    public class PayFilter
    {
        public int Id { get; set; }
        public decimal? Pay { get; set; }
        public int? PayBool { get; set; }

        public PayFilter(int id, decimal? pay, int? payBool)
        {
            Id = id;
            Pay = pay;
            PayBool = payBool;
        }
    }
}

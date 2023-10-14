using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exel_for_mfc.FilterModels
{
    public class SolFilter
    {
        public int Id { get; set; }
        public string? SolutionName { get; set; }

        public int? SolBool { get; set; }

        public SolFilter(int id, string? sol, int? solBool)
        {
            Id = id;
            SolutionName = sol;
            SolBool = solBool;
        }
    }
}

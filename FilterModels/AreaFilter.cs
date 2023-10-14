using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exel_for_mfc.FilterModels
{
    public class AreaFilter
    {

        public int Id { get; set; }

        public string? AreaName { get; set; }

        public int? AreaBool { get; set; }
        public AreaFilter(int id, string? areaName, int? areaBool)
        {
            Id = id;
            AreaName = areaName;
            AreaBool = areaBool;
        }
    }
}

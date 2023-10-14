using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exel_for_mfc.FilterModels
{
    public class LocalFilter
    {
        public int Id { get; set; }

        public string? LocalName { get; set; }

        public int? LocalBool { get; set; }

        public LocalFilter(int id, string? localName, int? localBool)
        {
            Id = id;
            LocalName = localName;
            LocalBool = localBool;
        }

    }
}

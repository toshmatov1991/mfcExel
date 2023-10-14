using DocumentFormat.OpenXml.Bibliography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exel_for_mfc.FilterModels
{
    public class PrivFilter
    {
        public int Id { get; set; }

        public string? PrivilegesName { get; set; }

        public int? PrivBool { get; set; }

        public PrivFilter(int id, string? priv, int? privBool)
        {
            Id = id;
            PrivilegesName = priv;
            PrivBool = privBool;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exel_for_mfc
{
    public partial class SClass
    {

       public int IdReg { get; set; }
        public string? Family { get; set; }
        public string? Name { get; set; }
        public string? Lastname { get; set; }
        public string? Snils { get; set; }
        public dynamic? Area { get; set; }
        public dynamic? Local { get; set; }
        public string? Adress { get; set; }
        public dynamic? Lgota { get; set; } // privel
        public dynamic? Pay { get; set; }
        public string? Sernumb { get; set; }
        public DateTime? DateGetSert { get; set; }
        public dynamic? Solution { get; set; } //Тип решения
        public string? DateAndNumbSolutionSert { get; set; } //Дата и номер решения по серту
        public string? Comment { get; set; }
        public string? Trek { get; set; }
        public DateTime? MailingDate { get; set; }
    }
}

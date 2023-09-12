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
        public int? Area { get; set; }
        public int? Local { get; set; }
        public string? Adress { get; set; }
        public dynamic? Lgota { get; set; } // privel
        public int? Pay { get; set; }
        public string? Sernumb { get; set; }
        public DateTime? DateGetSert { get; set; }
        public int? Solution { get; set; } //Тип решения
        public string? DateAndNumbSolutionSert { get; set; } //Дата и номер решения по серту
        public string? Comment { get; set; }
        public string? Trek { get; set; }
        public DateTime? MailingDate { get; set; }
        public int? IdApplicant { get; set; }
        public string? Commentar { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace exel_for_mfc;

public partial class Registry
{
    public int Id { get; set; }

    public int? ApplicantFk { get; set; }

    [ConcurrencyCheck]
    public string? SerialAndNumberSert { get; set; }

    [ConcurrencyCheck]
    public DateTime? DateGetSert { get; set; }

    public int? PayAmountFk { get; set; }

    public int? SolutionFk { get; set; }

    [ConcurrencyCheck]
    public string? DateAndNumbSolutionSert { get; set; }

    public string? Comment { get; set; }

    public string? Trek { get; set; }

    public DateTime? MailingDate { get; set; }

    public virtual Applicant? ApplicantFkNavigation { get; set; }

    public virtual PayAmount? PayAmountFkNavigation { get; set; }

    public virtual SolutionType? SolutionFkNavigation { get; set; }
}

using System;
using System.Collections.Generic;

namespace exel_for_mfc.Models;

public partial class Registry
{
    public int Id { get; set; }

    public string? Applicant { get; set; }

    public string? Snils { get; set; }

    public int? AreaFk { get; set; }

    public int? LocalityFk { get; set; }

    public string? Address { get; set; }

    public int? PrivilegesFk { get; set; }

    public string? SerialNumberOldSert { get; set; }

    public int? PaymentAmountFk { get; set; }

    public int? StatusSertFk { get; set; }

    public int? CertificateSolutionFk { get; set; }

    public string? Trek { get; set; }

    public DateTime? MailingDate { get; set; }

    public virtual Area? AreaFkNavigation { get; set; }

    public virtual CertificateSolution? CertificateSolutionFkNavigation { get; set; }

    public virtual Locality? LocalityFkNavigation { get; set; }

    public virtual PayAmount? PaymentAmountFkNavigation { get; set; }

    public virtual Privilege? PrivilegesFkNavigation { get; set; }

    public virtual StatusEx? StatusSertFkNavigation { get; set; }
}

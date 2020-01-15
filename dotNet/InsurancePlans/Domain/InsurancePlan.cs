using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sabio.Models.Domain.InsurancePlans
{
    public class InsurancePlan
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public InsuranceProviderBase InsuranceProvider { get; set; }
        public PlanLevel PlanLevel { get; set; }
        public PlanType PlanType { get; set; }
        public PlanStatus PlanStatus { get; set; }
    }
}

using Sabio.Models.Domain.InsurancePlans;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sabio.Models.Requests.InsurancePlans
{
    public class InsurancePlanAddRequest
    {
        [Required, Range(1, int.MaxValue)]
        public int InsuranceProviderId { get; set; }
        public string Name { get; set; }
        [Required, Range(1, int.MaxValue)]
        public int PlanTypeId { get; set; }
        public int PlanLevelId { get; set; }
        [StringLength(200, MinimumLength = 1)]
        public string Code { get; set; }
        [Range(1, int.MaxValue)]
        public int PlanStatusId { get; set; }
        [Range(0, int.MaxValue)]
        public int MinAge { get; set; }
        [Range(1, int.MaxValue)]
        public int MaxAge { get; set; }
    }
}

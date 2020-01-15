using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sabio.Models.Requests.InsurancePlans
{
    public class InsurancePlanUpdateRequest : InsurancePlanAddRequest, IModelIdentifier
    {
        [Required, Range(1, Int32.MaxValue)]
        public int Id { get; set; }
    }
}

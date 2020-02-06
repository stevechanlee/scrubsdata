using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sabio.Models.Domain.InsurancePlans
{
    public class InsuranceProviderBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SiteUrl { get; set; }
    }
}

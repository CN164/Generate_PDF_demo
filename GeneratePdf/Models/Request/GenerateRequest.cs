using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneratePDF_demo.Models
{
    public class ContactRequest
    {
        public DateTime contractCreatedAt { get; set; }
        public string? idCard { get; set; }
        public string? houseNo { get; set; }
        public string? villageNo { get; set; }
        public string? laneContact { get; set; }
        public string? roadContact { get; set; }
        public string? SubDistrict { get; set; }
        public string? districtContact { get; set; }
        public string? province { get; set; }
        public string? phoneNumber { get; set; }
        public string? preName { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }

    }
}

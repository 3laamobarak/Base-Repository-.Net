using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Project.Application.DTO.PaymentDTO
{
    public class PaymentMethodDto
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public string UserId { get; set; } = "";
        public string Gateway { get; set; } = "";
        public string ExternalId { get; set; } = "";
        public string TokenId { get; set; } = "";
        public string Last4 { get; set; } = "";
        public string CardBrand { get; set; } = "";
        public int ExpMonth { get; set; }
        public int ExpYear { get; set; }
        public bool IsDefault { get; set; }
    }
}

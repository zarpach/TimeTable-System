using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.DTO.Common
{
    public class OrganizationDTO
    {
        public int Id { get; set; }
        [MaxLength(50, ErrorMessage = "The field {0} length must be less than {1}")]
        public string Name { get; set; }
        public bool IsMain { get; set; }
    }
}

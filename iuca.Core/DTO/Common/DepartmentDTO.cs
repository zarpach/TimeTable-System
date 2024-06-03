using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.DTO.Common
{
    public class DepartmentDTO
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The field {0} length must be less than {1}")]
        public string Code { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "The field {0} length must be less than {1}")]
        public string NameEng { get; set; }

        [MaxLength(100, ErrorMessage = "The field {0} length must be less than {1}")]
        public string NameRus { get; set; }

        [MaxLength(100, ErrorMessage = "The field {0} length must be less than {1}")]
        public string NameKir { get; set; }

        public int OrganizationId { get; set; }
        public int ImportCode { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

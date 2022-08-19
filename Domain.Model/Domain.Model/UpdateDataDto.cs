using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Domain.Model
{
    public class UpdateDataDto
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int CorrectNumber { get; set; }
        public string CageId { get; set; }
        public string FileImage { get; set; }
        public string CreateId { get; set; }
        public int NumberLine { get; set; }
    }
}

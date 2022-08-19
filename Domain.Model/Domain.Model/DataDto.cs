using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Domain.Model
{
    public class DataDto
    {
        public string Status { get; set; }
        public int Id { get; set; }
        public string CageId { get; set; }
        public int Number { get; set; }
        public int CorrectNumber { get; set; }
        public int WrongNumber { get; set; }
        public string FileImage { get; set; }
        public int NumberLine { get; set; }
        public string CreateId { get; set; }
        public DateTime? CreateDate { get; set; }
    }

    public class NumberOnlyDto
    {
        public string Number { get; set; }
    }
}

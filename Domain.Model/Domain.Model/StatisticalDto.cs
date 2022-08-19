using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Domain.Model
{
    public class StatisticalDto
    {
        public string CageId { get; set; }
        public int Number { get; set; }
        public int CorrectNumber { get; set; }
        public int WrongNumber { get; set; }
        public int WrongWeights { get; set; }
        public int Weights { get; set; }
        public decimal PercentWeight { get; set; }
        public decimal PercentNumber { get; set; }
    }
}

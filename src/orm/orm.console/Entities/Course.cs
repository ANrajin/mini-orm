using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace orm.console.Entities
{
    public class Course : IEntity<int>
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public double Fees { get; set; }
        public IList<Topic>? Topics { get; set; }
        public IList<AdmissionTest>? Tests { get; set; }
    }
}

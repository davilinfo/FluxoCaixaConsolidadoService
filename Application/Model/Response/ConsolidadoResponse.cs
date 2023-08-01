using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Model.Response
{
    [ExcludeFromCodeCoverage]
    public class ConsolidadoResponse
    {
        public Guid Id { get; set; }
        public double BalanceValue { get; set; }
        public DateTime Created { get; set; }
        public virtual List<RecordViewModel> Records { get; set; }
        public virtual AccountViewModel IdAccount { get; set; }
    }
}

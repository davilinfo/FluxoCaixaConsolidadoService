using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Model
{
  public class RecordViewModel
  {
    public Guid Id { get; set; }    
    public required string History { get; set; }
    public DateTime DateTime { get; set; }    
    public char Type { get; set; }    
    public double Value { get; set; }
    public virtual required AccountViewModel IdAccountNavigation { get; set; }
  }
}

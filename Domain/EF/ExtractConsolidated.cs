using Domain.EF.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.EF
{
  [Table("ExtractConsolidated")]
  public class ExtractConsolidated : GuidEntity
  {
    [Required]
    public Guid AccountId { get; set; }
    [Required]
    public DateTime Date { get; set; }
    [Required]
    public string Extract { get; set; }
  }
}

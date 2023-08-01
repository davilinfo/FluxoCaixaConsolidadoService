using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Domain.EF.Abstract
{
  [ExcludeFromCodeCoverage]
  public abstract class GuidEntity
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    [Column("Id")]
    public Guid Id { get; set; }
  }
}

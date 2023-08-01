namespace Application.Model
{
  public class AccountViewModel
  {
    public Guid Id { get; set; }
    public long AccountNumber { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
  }
}

namespace Domain.Contract
{
  public interface IRepository<Entity>
  {
    public Task<Guid> Add(Entity entidade);
    public Task<int> Update(Entity entidade);
    public Task<int> Delete(Guid id);
    public IQueryable<Entity> All();
    public Task<Entity> GetById(Guid id);
  }
}

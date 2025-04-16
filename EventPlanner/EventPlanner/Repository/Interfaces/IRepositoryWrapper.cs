using EventPlanner.Models;

namespace EventPlanner.Repository.Interfaces
{
  public interface IRepositoryWrapper
  {
    IEventRepository Event { get; }
    IRepository<Guest> Guest { get; }
    IRepository<Models.Host> Host { get; }
    IRepository<Comment> Comment { get; }

    Task SaveAsync();
  }

}

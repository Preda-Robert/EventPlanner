using EventPlanner.Models;

namespace EventPlanner.Repository.Interfaces
{
  public interface IRepositoryWrapper
  {
    IRepository<Comment> Comment { get; }
    IRepository<Event> Event { get; }
    IRepository<EventGuest> EventGuest { get; }
    IRepository<Guest> Guest { get; }
    IRepository<Models.Host> Host { get; }
    IRepository<Registration> Registration { get; }
    Task SaveAsync();
  }

}

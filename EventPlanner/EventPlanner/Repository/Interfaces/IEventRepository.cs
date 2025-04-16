using EventPlanner.Models;

namespace EventPlanner.Repository.Interfaces
{
  public interface IEventRepository : IRepository<Event>
  {
    Task<IEnumerable<Event>> GetEventsWithGuestsAsync();
  }

}

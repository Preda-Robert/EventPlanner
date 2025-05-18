using EventPlanner.Models;

namespace EventPlanner.Services.Interfaces
{
  public interface IEventService
  {
    public IQueryable<Event> SearchEvents(IQueryable<Event> events, string searchTerm);
    public IQueryable<Event> SortEvents(IQueryable<Event> events, string sortOrder);


  }
}

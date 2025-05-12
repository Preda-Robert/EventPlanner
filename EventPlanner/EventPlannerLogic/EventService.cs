using EventPlannerLogic;

namespace EventPlanner.Services
{
  public class EventService : IEventService
  {
    public IQueryable<Event> SearchEvents(IQueryable<Event> events, string searchTerm)
    {

      if (!string.IsNullOrWhiteSpace(searchTerm))
      {
        events = events.Where(e => e.Title.Contains(searchTerm));
      }
      return events;
    }

    public IQueryable<Event> SortEvents(IQueryable<Event> events, string sortOrder)
    {
      return sortOrder switch
      {
        "title_asc" => events.OrderBy(e => e.Title),
        "title_desc" => events.OrderByDescending(e => e.Title),
        "date_asc" => events.OrderBy(e => e.Date),
        "date_desc" => events.OrderByDescending(e => e.Date),
        _ => events.OrderBy(e => e.Title)
      };
    }

  }
}

using EventPlanner.Models;
using EventPlanner.Repository;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace EventPlanner.Tests.Repository
{
  public class RepositoryTests
  {
    private readonly Mock<ApplicationDbContext> _mockContext;
    private readonly Mock<DbSet<Event>> _mockEventDbSet;
    private readonly Repository<Event> _eventRepo;

    public RepositoryTests()
    {
      _mockContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
      _mockEventDbSet = new Mock<DbSet<Event>>();
      _mockContext.Setup(c => c.Set<Event>()).Returns(_mockEventDbSet.Object);
      _eventRepo = new Repository<Event>(_mockContext.Object);
    }

    //[Fact]
    //public async Task GetAllAsync_ReturnsAllEntities()
    //{
    //  // Arrange
    //  var events = new List<Event>
    //        {
    //            new Event { EventId = 1, Title = "Test Event 1" },
    //            new Event { EventId = 2, Title = "Test Event 2" }
    //        }.AsQueryable();

    //  _mockEventDbSet.As<IQueryable<Event>>().Setup(m => m.Provider).Returns(events.Provider);
    //  _mockEventDbSet.As<IQueryable<Event>>().Setup(m => m.Expression).Returns(events.Expression);
    //  _mockEventDbSet.As<IQueryable<Event>>().Setup(m => m.ElementType).Returns(events.ElementType);
    //  _mockEventDbSet.As<IQueryable<Event>>().Setup(m => m.GetEnumerator()).Returns(events.GetEnumerator());

    //  // Act
    //  var result = await _eventRepo.GetAllAsync();

    //  // Assert
    //  Assert.Equal(2, result.Count());
    //}

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsEntity()
    {
      // Arrange
      var ev = new Event { EventId = 1, Title = "Test Event" };

      _mockEventDbSet.Setup(m => m.FindAsync(1))
          .ReturnsAsync(ev);

      // Act
      var result = await _eventRepo.GetByIdAsync(1);

      // Assert
      Assert.Equal(1, result.EventId);
    }

    [Fact]
    public async Task AddAsync_CallsAddAsyncOnDbSet()
    {
      // Arrange
      var ev = new Event { Title = "Test Event" };

      // Act
      await _eventRepo.AddAsync(ev);

      // Assert
      _mockEventDbSet.Verify(m => m.AddAsync(ev, default), Times.Once);
    }

    [Fact]
    public void Update_CallsUpdateOnDbSet()
    {
      // Arrange
      var ev = new Event { EventId = 1, Title = "Test Event" };

      // Act
      _eventRepo.Update(ev);

      // Assert
      _mockEventDbSet.Verify(m => m.Update(ev), Times.Once);
    }

    [Fact]
    public void Delete_CallsRemoveOnDbSet()
    {
      // Arrange
      var ev = new Event { EventId = 1, Title = "Test Event" };

      // Act
      _eventRepo.Delete(ev);

      // Assert
      _mockEventDbSet.Verify(m => m.Remove(ev), Times.Once);
    }
  }

  public class RepositoryWrapperTests
  {
    private readonly Mock<ApplicationDbContext> _mockContext;
    private readonly RepositoryWrapper _wrapper;

    public RepositoryWrapperTests()
    {
      _mockContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
      _wrapper = new RepositoryWrapper(_mockContext.Object);
    }

    [Fact]
    public void Wrapper_InitializesRepositories()
    {
      // Assert
      Assert.NotNull(_wrapper.Comment);
      Assert.NotNull(_wrapper.Event);
      Assert.NotNull(_wrapper.EventGuest);
      Assert.NotNull(_wrapper.Guest);
      Assert.NotNull(_wrapper.Host);
      Assert.NotNull(_wrapper.Registration);
    }

    [Fact]
    public async Task SaveAsync_CallsSaveChangesAsyncOnContext()
    {
      // Arrange
      _mockContext.Setup(c => c.SaveChangesAsync(default))
          .ReturnsAsync(1);

      // Act
      await _wrapper.SaveAsync();

      // Assert
      _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
    }
  }







}
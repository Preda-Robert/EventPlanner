using EventPlanner.Models;
using EventPlanner.Repository;
using EventPlanner.Repository.Interfaces;
using EventPlanner.Services;
using EventPlanner.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
{
  options.Password.RequireDigit = false;
  options.Password.RequireLowercase = false;
  options.Password.RequireNonAlphanumeric = false;
  options.Password.RequireUppercase = false;
  options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
builder.Services.AddScoped<IHostRepository, HostRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IGuestRepository, GuestRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();

builder.Services.Add(new ServiceDescriptor(typeof(IEventService), typeof(EventService), ServiceLifetime.Scoped));



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Home/Error");
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
  var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
  var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

  string[] roles = { "Admin", "User" };

  foreach (var role in roles)
  {
    if (!await roleManager.RoleExistsAsync(role))
    {
      await roleManager.CreateAsync(new IdentityRole<int>(role));
    }
  }

  var adminEmail = "admin@example.com";
  var adminUser = await userManager.FindByEmailAsync(adminEmail);

  if (adminUser == null)
  {
    adminUser = new ApplicationUser
    {
      UserName = adminEmail,
      Email = adminEmail,
      Name = "Admin"
    };
    await userManager.CreateAsync(adminUser, "Admin123!");
    await userManager.AddToRoleAsync(adminUser, "Admin");
  }
}


app.Run();


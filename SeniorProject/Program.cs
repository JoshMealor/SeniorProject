using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SeniorProject.Models.DataLayer;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


//database Migrations folder generated with package manager cmd: Add-Migration Initial
//local sql server database created using package manager cmd: Update - Database
//This creates the identity tables used for Authentication and Authorization

//The default admin is created by invoking a method on the SeniorProjectDBContext class
//username = "admin";
//password = "Password123";






var builder = WebApplication.CreateBuilder(args);



//Session must be added befor the mvc
//Add the session state memory cache
builder.Services.AddMemoryCache();
//Add the session state 
builder.Services.AddSession(options =>
{
    //Change the timeout to 5 minutes
    options.IdleTimeout = TimeSpan.FromSeconds(60 * 5);
    //Mark as http only
    options.Cookie.HttpOnly = false;
    //Mark as essential
    options.Cookie.IsEssential = true;
});

//Add the routing middleware service
builder.Services.AddRouting(options =>
{
    //make urls lowercase
    options.LowercaseUrls = true;
    //add the trailing slash
    options.AppendTrailingSlash = true;
});



// Add services to the container.
builder.Services.AddControllersWithViews();

//Add the dbcontext services to interact with the database. Pass in the options to use sql server and pass in the builder's configuration's object's getconnectionstring method to grab the connection string in the appsettings.json file
builder.Services.AddDbContext<SeniorProjectDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SPDBContext")));
//Add the identity service for authenticating and authorizing users. Chain the methods to add on the EF database context where the identity infor is stored. Add on the token providers
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    //require passwords to be at least 6 characters
    options.Password.RequiredLength = 6;
    //Do not require a special character
    options.Password.RequireNonAlphanumeric = false;
    //Do not require a number
    options.Password.RequireDigit = false;
})
    .AddEntityFrameworkStores<SeniorProjectDBContext>()
    .AddDefaultTokenProviders();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
//add error handler page
app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
//configure application to use session which will allow someone to stay logged in.
app.UseSession();

//Map routes for the admin area, the help area, and the default 
app.UseEndpoints(endpoints =>
{
    
    // map route for the Admin area
    endpoints.MapAreaControllerRoute(
         name: "authenticated",
         areaName: "Authenticated",
         pattern: "{area=Authenticated}/{controller=Home}/{action=Portal}/{id?}");

    //map route for default
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

});

//Get a dependency injection service
IServiceScopeFactory scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
//Call the create admin method using a temporary service to call the createAdminUser method
using (IServiceScope scope = scopeFactory.CreateScope())
{
    await SeniorProjectDBContext.CreateBasicRoles(scope.ServiceProvider);
    //This will create a default account in the heffer database with username admin, password Password123
    await SeniorProjectDBContext.CreateAdminUser(
        scope.ServiceProvider);

    await SeniorProjectDBContext.CreateBasicUsers(scope.ServiceProvider);

}

app.Run();

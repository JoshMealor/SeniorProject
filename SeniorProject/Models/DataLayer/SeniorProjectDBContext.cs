using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SeniorProject.Models.DataLayer.TableModels;
using System;


namespace SeniorProject.Models.DataLayer
{
    public class SeniorProjectDBContext : IdentityDbContext<Models.DataLayer.User>
    {

        public DbSet<Member> Members { get; set; }
        public DbSet<EventInventation> EventInventations { get; set; }
        public DbSet<Event> Events { get; set; }
       
        public DbSet<Due> Dues { get; set; }

        public DbSet<Donor> Donors { get; set; }
        public DbSet<Donnation> Donnations { get; set; }
        public DbSet<RoleAccess> RoleAccess { get; set; }
    
        public SeniorProjectDBContext(DbContextOptions<SeniorProjectDBContext> options) : base(options)
        {

        }

        //Add a method to create the default admin account in the database
        //This is called by the program class once at the start of the application
        //If resarting the application more than once this is where the checking if it exits comes into play
        public static async Task CreateAdminUser(
                IServiceProvider serviceProvider)
        {
            UserManager<User> userManager =
                serviceProvider.GetRequiredService<UserManager<User>>();
            RoleManager<IdentityRole> roleManager = serviceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

            string username = "admin";
            string password = "Password123";
            string roleName = "Admin";

            // if role doesn't exist, create it
            if (await roleManager.FindByNameAsync(roleName) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
            // if username doesn't exist, create it and add it to role
            if (await userManager.FindByNameAsync(username) == null)
            {
                User user = new User { UserName = username };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, roleName);
                }
            }
        }

        //Overide the base dbcontext class OnModelCreating method
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Add the identity tables to the database
            base.OnModelCreating(modelBuilder);
           
        }

    }
}

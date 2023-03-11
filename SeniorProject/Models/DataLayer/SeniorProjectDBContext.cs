using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SeniorProject.Models.DataLayer.TableModels;
using System;


namespace SeniorProject.Models.DataLayer
{
    public class SeniorProjectDBContext : IdentityDbContext<IdentityUser>
    {

        public DbSet<Member> Members { get; set; }
       
        public DbSet<EventInvitation> EventInvitations { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Due> Dues { get; set; }
        public DbSet<Donation> Donations { get; set; }
      
    
        public SeniorProjectDBContext(DbContextOptions<SeniorProjectDBContext> options) : base(options)
        {

        }

        //Add a method to create the default admin account in the database
        //This is called by the program class once at the start of the application
        //If resarting the application more than once this is where the checking if it exsits comes into play
        public static async Task CreateAdminUser(
                IServiceProvider serviceProvider)
        {
            UserManager<IdentityUser> userManager =
                serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
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
                IdentityUser user = new IdentityUser { UserName = username };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, roleName);
                }
            }
        }

        public static async Task CreateBasicRoles(
               IServiceProvider serviceProvider)
        {
            RoleManager<IdentityRole> roleManager = serviceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

            
            string roleName = "Admin";

            // if role doesn't exist, create it
            if (await roleManager.FindByNameAsync(roleName) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
            roleName = "Manager";

            // if role doesn't exist, create it
            if (await roleManager.FindByNameAsync(roleName) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
            roleName = "BasicMember";

            // if role doesn't exist, create it
            if (await roleManager.FindByNameAsync(roleName) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
            roleName = "Volunteer";

            // if role doesn't exist, create it
            if (await roleManager.FindByNameAsync(roleName) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        public static async Task CreateBasicUsers(
               IServiceProvider serviceProvider)
        {
            UserManager<IdentityUser> userManager =
                serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            RoleManager<IdentityRole> roleManager = serviceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

            string username = "testManager";
            string password = "Password123";
            string roleName = "Manager";

            // if role doesn't exist, create it
            if (await roleManager.FindByNameAsync(roleName) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
            // if username doesn't exist, create it and add it to role
            if (await userManager.FindByNameAsync(username) == null)
            {
                IdentityUser user = new IdentityUser { UserName = username };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, roleName);
                }
            }


            username = "testBasicMember";
            password = "Password123";
            roleName = "BasicMember";

            // if role doesn't exist, create it
            if (await roleManager.FindByNameAsync(roleName) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
            // if username doesn't exist, create it and add it to role
            if (await userManager.FindByNameAsync(username) == null)
            {
                IdentityUser user = new IdentityUser { UserName = username };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, roleName);
                }
            }


            username = "testBasicMember2";
            password = "Password123";
            roleName = "BasicMember";

            // if role doesn't exist, create it
            if (await roleManager.FindByNameAsync(roleName) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
            // if username doesn't exist, create it and add it to role
            if (await userManager.FindByNameAsync(username) == null)
            {
                IdentityUser user = new IdentityUser { UserName = username };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, roleName);
                }
            }


            username = "testVolunteer";
            password = "Password123";
            roleName = "Volunteer";

            // if role doesn't exist, create it
            if (await roleManager.FindByNameAsync(roleName) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
            // if username doesn't exist, create it and add it to role
            if (await userManager.FindByNameAsync(username) == null)
            {
                IdentityUser user = new IdentityUser{ UserName = username };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, roleName);
                }
            }


        }


        public static async Task SeedMembers(IServiceProvider serviceProvider)
        {
            UserManager<IdentityUser> userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            SeniorProjectDBContext context = serviceProvider.GetRequiredService<SeniorProjectDBContext>();

            //check to see if seeding is needed
            var memberList = context.Members.ToList();
            if (memberList.Count > 0)
            {
                //dont seed data
            }
            else
            {
                //Seed data
                //Create variables to reuse
                IdentityUser identityUserRecord;
                SeniorProject.Models.DataLayer.TableModels.Member member;

                //---admin role access level
                //Instantiate a table record model 
                member = new SeniorProject.Models.DataLayer.TableModels.Member();
                //Update all the member fields
                member.MemberRole = "RandomMemberRole";
                member.ActiveStatus = true;
                member.FirstName = "John_Admin";
                member.LastName = "Doe_Admin";
                member.City = "Greenville";
                member.State = "SC";                
                //Seed usernames are admin, testManager,testBasicMember,testVolunteer
                //Retrive identityuser match
                identityUserRecord = await userManager.FindByNameAsync("admin");
                //Assign this to the new table record
                member.IdentityUser = identityUserRecord;
                member.IdentityUserID = identityUserRecord.Id;
                //Add record and update database
                context.Members.Add(member);
                context.SaveChanges();






                //---manager role access level
                //Instantiate a table record model 
                member = new SeniorProject.Models.DataLayer.TableModels.Member();
                //Update all the member fields
                member.MemberRole = "DueCollector";
                member.ActiveStatus = true;
                member.FirstName = "John_Manager";
                member.LastName = "Doe_Manager";
                member.City = "Greenville";
                member.State = "SC";
                //Seed usernames are admin, testManager,testBasicMember,testVolunteer
                //Retrive identityuser match
                identityUserRecord = await userManager.FindByNameAsync("testManager");
                //Assign this to the new table record
                member.IdentityUser = identityUserRecord;
                member.IdentityUserID = identityUserRecord.Id;
                //Add record and update database
                context.Members.Add(member);
                context.SaveChanges();

                //---basic member role access level
                //Instantiate a table record model 
                member = new SeniorProject.Models.DataLayer.TableModels.Member();
                //Update all the member fields
                member.MemberRole = "DonnationCollector";
                member.ActiveStatus = true;
                member.FirstName = "John_BasicMember";
                member.LastName = "Doe_BasicMember";
                member.City = "Greenville";
                member.State = "SC";
                //Seed usernames are admin, testManager,testBasicMember,testVolunteer
                //Retrive identityuser match
                identityUserRecord = await userManager.FindByNameAsync("testBasicMember");
                //Assign this to the new table record
                member.IdentityUser = identityUserRecord;
                member.IdentityUserID = identityUserRecord.Id;
                //Add record and update database
                context.Members.Add(member);
                context.SaveChanges();


                //---basic member role access level
                //Instantiate a table record model 
                member = new SeniorProject.Models.DataLayer.TableModels.Member();
                //Update all the member fields
                member.MemberRole = "Event Coordinator";
                member.ActiveStatus = true;
                member.FirstName = "John_BasicMember2";
                member.LastName = "Doe_BasicMember2";
                member.City = "Greenville";
                member.State = "SC";
                //Seed usernames are admin, testManager,testBasicMember,testVolunteer
                //Retrive identityuser match
                identityUserRecord = await userManager.FindByNameAsync("testBasicMember2");
                //Assign this to the new table record
                member.IdentityUser = identityUserRecord;
                member.IdentityUserID = identityUserRecord.Id;
                //Add record and update database
                context.Members.Add(member);
                context.SaveChanges();



                //---volunteer role access level
                //Instantiate a table record model 
                member = new SeniorProject.Models.DataLayer.TableModels.Member();
                //Update all the member fields
                member.MemberRole = "RandomMemberRole";
                member.ActiveStatus = true;
                member.FirstName = "John_Volunteer";
                member.LastName = "Doe_Volunteer";
                member.City = "Greenville";
                member.State = "SC";
                //Seed usernames are admin, testManager,testBasicMember,testVolunteer
                //Retrive identityuser match
                identityUserRecord = await userManager.FindByNameAsync("testVolunteer");
                //Assign this to the new table record
                member.IdentityUser = identityUserRecord;
                member.IdentityUserID = identityUserRecord.Id;
                //Add record and update database
                context.Members.Add(member);
                context.SaveChanges();

            }
        }


        public static async Task SeedDonations(IServiceProvider serviceProvider)
        {
            UserManager<IdentityUser> userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            SeniorProjectDBContext context = serviceProvider.GetRequiredService<SeniorProjectDBContext>();

            var donationsList = context.Donations.ToList();
            if (donationsList.Count > 0)
            {
                //Dont seed
            }
            else
            {
                //Seed
                //Get a list of members to pull from when seeding the memberId related key
                //Firstnames can be used to seed each role: John_Admin, John_Manager, John_BasicMember, John_Volunteer
                List<Models.DataLayer.TableModels.Member> members = context.Members.ToList();

                //Create variables to reuse
                SeniorProject.Models.DataLayer.TableModels.Donation tableRecord;
                SeniorProject.Models.DataLayer.TableModels.Member relatedRecord;

                
                //Instantiate a table record model 
                tableRecord = new Donation();
                tableRecord.DonationAmmount = 50.0;
                tableRecord.DonationDate= DateTime.Now;
                tableRecord.DonorNameOrTitle = "RandomDonorOrganization";
                //Set the collected by member to the role 
                //Retrive a matching relatedRecord
                foreach (var member in members)
                {
                    if (member.MemberRole == "DonnationCollector")
                    {
                        tableRecord.Member = member;
                        tableRecord.MemberID= member.MemberID;
                        
                    }
                }
                context.Donations.Add(tableRecord);
                context.SaveChanges();

                //Instantiate a table record model 
                tableRecord = new Donation();
                tableRecord.DonationAmmount = 50.0;
                tableRecord.DonationDate = DateTime.Now;
                tableRecord.DonorNameOrTitle = "JohnDoe1";
                //Set the collected by member to the role 
                //Retrive a matching relatedRecord
                foreach (var member in members)
                {
                    if (member.MemberRole == "DonnationCollector")
                    {
                        tableRecord.Member = member;
                        tableRecord.MemberID = member.MemberID;

                    }
                }
                context.Donations.Add(tableRecord);
                context.SaveChanges();

                //Instantiate a table record model 
                tableRecord = new Donation();
                tableRecord.DonationAmmount = 50.0;
                tableRecord.DonationDate = DateTime.Now;
                tableRecord.DonorNameOrTitle = "RandomDonorOrganization2";
                //Set the collected by member to the role 
                //Retrive a matching relatedRecord
                foreach (var member in members)
                {
                    if (member.MemberRole == "DueCollector")
                    {
                        tableRecord.Member = member;
                        tableRecord.MemberID = member.MemberID;

                    }
                }
                context.Donations.Add(tableRecord);
                context.SaveChanges();

            }
        }



        public static async Task SeedDues(IServiceProvider serviceProvider)
        {
            UserManager<IdentityUser> userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            SeniorProjectDBContext context = serviceProvider.GetRequiredService<SeniorProjectDBContext>();

            var DuesList = context.Dues.ToList();
            if (DuesList.Count > 0)
            {
                //dont seed
            }
            else
            {
                //Seed
                //Get a list of members to pull from when seeding the memberId related key
                //Firstnames can be used to seed each role: John_Admin, John_Manager, John_BasicMember, John_Volunteer
                List<Models.DataLayer.TableModels.Member> members = context.Members.ToList();

                //Create variables to reuse
                SeniorProject.Models.DataLayer.TableModels.Due tableRecord;
                SeniorProject.Models.DataLayer.TableModels.Member relatedRecord;
                var dateString = "4/1/2023 9:00:00 AM";
                DateTime date = DateTime.Parse(dateString, System.Globalization.CultureInfo.InvariantCulture);


                //Instantiate a table record model 
                tableRecord = new Due();
                tableRecord.AmountDue = 50;
                dateString = "4/1/2023 9:00:00 AM";
                date = DateTime.Parse(dateString,System.Globalization.CultureInfo.InvariantCulture);
                tableRecord.DateTimeDue = date;
                tableRecord.AmountPaid = 50;
                dateString = "3/1/2023 9:00:00 AM";
                date = DateTime.Parse(dateString, System.Globalization.CultureInfo.InvariantCulture);
                tableRecord.DateTimePaid = date;
                tableRecord.PaymentMethod = "Cash";

                //Set the member paying by member to the role 
                //Retrive a matching relatedRecord
                foreach (var member in members)
                {
                    if (member.FirstName == "John_Admin")
                    {
                        tableRecord.Member = member;
                        tableRecord.MemberID = member.MemberID;

                    }
                }
                context.Dues.Add(tableRecord);
                context.SaveChanges();



                //Instantiate a table record model 
                tableRecord = new Due();
                tableRecord.AmountDue = 50;
                dateString = "4/1/2023 9:00:00 AM";
                date = DateTime.Parse(dateString, System.Globalization.CultureInfo.InvariantCulture);
                tableRecord.DateTimeDue = date;
                tableRecord.AmountPaid = 50;
                dateString = "3/1/2023 9:00:00 AM";
                date = DateTime.Parse(dateString, System.Globalization.CultureInfo.InvariantCulture);
                tableRecord.DateTimePaid = date;
                tableRecord.PaymentMethod = "Cash";

                //Set the member paying by member to the role 
                //Retrive a matching relatedRecord
                foreach (var member in members)
                {
                    if (member.FirstName == "John_Manager")
                    {
                        tableRecord.Member = member;
                        tableRecord.MemberID = member.MemberID;

                    }
                }
                context.Dues.Add(tableRecord);
                context.SaveChanges();


                //Instantiate a table record model 
                tableRecord = new Due();
                tableRecord.AmountDue = 50;
                dateString = "4/1/2023 9:00:00 AM";
                date = DateTime.Parse(dateString, System.Globalization.CultureInfo.InvariantCulture);
                tableRecord.DateTimeDue = date;
                tableRecord.AmountPaid = 50;
                dateString = "3/1/2023 9:00:00 AM";
                date = DateTime.Parse(dateString, System.Globalization.CultureInfo.InvariantCulture);
                tableRecord.DateTimePaid = date;
                tableRecord.PaymentMethod = "Cash";

                //Set the member paying by member to the role 
                //Retrive a matching relatedRecord
                foreach (var member in members)
                {
                    if (member.MemberRole == "DonnationCollector")
                    {
                        tableRecord.Member = member;
                        tableRecord.MemberID = member.MemberID;

                    }
                }
                context.Dues.Add(tableRecord);
                context.SaveChanges();



                //Instantiate a table record model 
                tableRecord = new Due();
                tableRecord.AmountDue = 50;
                dateString = "4/1/2023 9:00:00 AM";
                date = DateTime.Parse(dateString, System.Globalization.CultureInfo.InvariantCulture);
                tableRecord.DateTimeDue = date;
                tableRecord.AmountPaid = 50;
                dateString = "3/1/2023 9:00:00 AM";
                date = DateTime.Parse(dateString, System.Globalization.CultureInfo.InvariantCulture);
                tableRecord.DateTimePaid = date;
                tableRecord.PaymentMethod = "Cash";

                //Set the member paying by member to the role 
                //Retrive a matching relatedRecord
                foreach (var member in members)
                {
                    if (member.MemberRole == "Event Coordinator")
                    {
                        tableRecord.Member = member;
                        tableRecord.MemberID = member.MemberID;

                    }
                }
                context.Dues.Add(tableRecord);
                context.SaveChanges();



                //Instantiate a table record model 
                tableRecord = new Due();
                tableRecord.AmountDue = 50;
                dateString = "4/1/2023 9:00:00 AM";
                date = DateTime.Parse(dateString, System.Globalization.CultureInfo.InvariantCulture);
                tableRecord.DateTimeDue = date;
                tableRecord.AmountPaid = 50;
                dateString = "3/1/2023 9:00:00 AM";
                date = DateTime.Parse(dateString, System.Globalization.CultureInfo.InvariantCulture);
                tableRecord.DateTimePaid = date;
                tableRecord.PaymentMethod = "Cash";

                //Set the member paying by member to the role 
                //Retrive a matching relatedRecord
                foreach (var member in members)
                {
                    if (member.FirstName == "John_Volunteer")
                    {
                        tableRecord.Member = member;
                        tableRecord.MemberID = member.MemberID;

                    }
                }
                context.Dues.Add(tableRecord);
                context.SaveChanges();

            }
        }



        public static async Task SeedEvents(IServiceProvider serviceProvider)
        {
            UserManager<IdentityUser> userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            SeniorProjectDBContext context = serviceProvider.GetRequiredService<SeniorProjectDBContext>();

            var eventList = context.Events.ToList();
            if (eventList.Count > 0)
            {
                //Dont seed
            }
            else
            {
                //Seed
                //Get a list of members to pull from when seeding the memberId related key
                //Firstnames can be used to seed each role: John_Admin, John_Manager, John_BasicMember, John_Volunteer
                List<Models.DataLayer.TableModels.Member> members = context.Members.ToList();

                //Create variables to reuse
                SeniorProject.Models.DataLayer.TableModels.Event tableRecord;
                SeniorProject.Models.DataLayer.TableModels.Member relatedRecord;
                var dateString = "4/1/2023 9:00:00 AM";
                DateTime date = DateTime.Parse(dateString, System.Globalization.CultureInfo.InvariantCulture);


                //Instantiate a table record model 
                tableRecord = new Event();
                tableRecord.EventName = "RandomEventName1";
                tableRecord.EventDescription = "This random event will be held in random city for random reason. It features random food too.";
                dateString = "3/19/2023 9:00:00 AM";
                date = DateTime.Parse(dateString, System.Globalization.CultureInfo.InvariantCulture);
                tableRecord.StartDateTime = date;
                dateString = "3/20/2023 9:00:00 PM";
                date = DateTime.Parse(dateString, System.Globalization.CultureInfo.InvariantCulture);
                tableRecord.EndDateTime = date;
                tableRecord.EventLocation = "Greenville, SC";
                //Set the created by member to the role 
                //Retrive a matching relatedRecord
                foreach (var member in members)
                {
                    if (member.MemberRole == "Event Coordinator")
                    {
                        tableRecord.Member = member;
                        tableRecord.MemberID = member.MemberID;

                    }
                }
                context.Events.Add(tableRecord);
                context.SaveChanges();



                //Instantiate a table record model 
                tableRecord = new Event();
                tableRecord.EventName = "RandomEventName2";
                tableRecord.EventDescription = "This random event will be held in random city for random reason. It features random food too.";
                dateString = "3/26/2023 9:00:00 AM";
                date = DateTime.Parse(dateString, System.Globalization.CultureInfo.InvariantCulture);
                tableRecord.StartDateTime = date;
                dateString = "3/27/2023 9:00:00 PM";
                date = DateTime.Parse(dateString, System.Globalization.CultureInfo.InvariantCulture);
                tableRecord.EndDateTime = date;
                tableRecord.EventLocation = "Greenville, SC";
                //Set the created by member to the role 
                //Retrive a matching relatedRecord
                foreach (var member in members)
                {
                    if (member.MemberRole == "Event Coordinator")
                    {
                        tableRecord.Member = member;
                        tableRecord.MemberID = member.MemberID;

                    }
                }
                context.Events.Add(tableRecord);
                context.SaveChanges();


                //Instantiate a table record model 
                tableRecord = new Event();
                tableRecord.EventName = "RandomEventName3";
                tableRecord.EventDescription = "This random event will be held in random city for random reason. It features random food too.";
                dateString = "4/2/2023 9:00:00 AM";
                date = DateTime.Parse(dateString, System.Globalization.CultureInfo.InvariantCulture);
                tableRecord.StartDateTime = date;
                dateString = "4/2/2023 9:00:00 PM";
                date = DateTime.Parse(dateString, System.Globalization.CultureInfo.InvariantCulture);
                tableRecord.EndDateTime = date;
                tableRecord.EventLocation = "Greenville, SC";
                //Set the created by member to the role 
                //Retrive a matching relatedRecord
                foreach (var member in members)
                {
                    if (member.MemberRole == "Event Coordinator")
                    {
                        tableRecord.Member = member;
                        tableRecord.MemberID = member.MemberID;

                    }
                }
                context.Events.Add(tableRecord);
                context.SaveChanges();




            }

                
        }




        public static async Task SeedEventInvitations(IServiceProvider serviceProvider)
        {
            UserManager<IdentityUser> userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            SeniorProjectDBContext context = serviceProvider.GetRequiredService<SeniorProjectDBContext>();

            var eventInvitationList = context.EventInvitations.ToList();
            if (eventInvitationList.Count > 0)
            {
                //Dont seed
            }
            else
            {
                //Seed
                //Get a list of members to pull from when seeding the memberId related key
                //Firstnames can be used to seed each role: John_Admin, John_Manager, John_BasicMember, John_Volunteer
                List<Models.DataLayer.TableModels.Member> members = context.Members.ToList();

                //Create variables to reuse
                SeniorProject.Models.DataLayer.TableModels.EventInvitation tableRecord;
                SeniorProject.Models.DataLayer.TableModels.Event relatedRecordEvent;
                SeniorProject.Models.DataLayer.TableModels.Member relatedRecordMember;
              
                //Instantiate a table record model 
                tableRecord = new EventInvitation();
                tableRecord.InvitationBody = "Please join us for our event";
                tableRecord.InvitationResponseBody = "Reciepient has not responded";
                relatedRecordEvent = context.Events.Find(1);
                tableRecord.Event = relatedRecordEvent;
                tableRecord.EventID = relatedRecordEvent.EventID;               
                //Set the sent by member to the role 
                //Retrive a matching relatedRecord
                foreach (var member in members)
                {
                    if (member.MemberRole == "Event Coordinator")
                    {
                        tableRecord.Member_Sender = member;
                        tableRecord.MemberID_Sender = member.MemberID;

                    }
                }
                //Set the sent by member to the role 
                //Retrive a matching relatedRecord
                foreach (var member in members)
                {
                    if (member.FirstName == "John_Volunteer")
                    {
                        tableRecord.Member_Reciever = member;
                        tableRecord.MemberID_Reciever = member.MemberID;

                    }
                }
                context.EventInvitations.Add(tableRecord);
                context.SaveChanges();



                




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

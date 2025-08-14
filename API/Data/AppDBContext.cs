using DomainModels;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
    }

   
    }
    

//INFO : For update and add migrations: View/OtherWindows/Package Manager Console.
//Add migration:add-migration "name-migration". Migrate: dotnet ef database update.
//If feil with migrations,
//ensure you have run the following commands: dotnet ef database update --project "Project path". Right-click on the API project and copy Path.
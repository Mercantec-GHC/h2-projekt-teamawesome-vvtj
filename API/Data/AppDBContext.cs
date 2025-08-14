using DomainModels.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data;
public class AppDBContext : DbContext
{
	public AppDBContext(DbContextOptions<AppDBContext> options)
		: base(options)
	{
	}
	public DbSet<User> Users { get; set; }
	public DbSet<UserInfo> UserInfos { get; set; }
	public DbSet<Role> Roles { get; set; }
	public DbSet<Booking> Bookings { get; set; }
	public DbSet<Room> Rooms { get; set; }
	public DbSet<RoomType> RoomTypes { get; set; }
	public DbSet<Hotel> Hotels { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<User>(entity =>
		{
			entity.HasIndex(u => u.Email).IsUnique();
			entity.HasIndex(u => u.UserName).IsUnique();
			entity.HasOne(u => u.UserRole)
				.WithMany(r => r.Users)
        
    
    
				.HasForeignKey(u => u.UserRoleId)
				.OnDelete(DeleteBehavior.Restrict);
		});

		modelBuilder.Entity<Role>(entity =>
		{
			entity.HasIndex(r => r.RoleName).IsUnique();
		});

		modelBuilder.Entity<UserInfo>(entity =>
		{
			entity.HasKey(ui => ui.UserId);
		});
        
        modelBuilder.Entity<Hotel>()
         .HasMany(h => h.Rooms)
         .WithOne(r => r.Hotel)
         .HasForeignKey(r => r.HotelId);

		base.OnModelCreating(modelBuilder);
	}
}

//INFO : For update and add migrations: View/OtherWindows/Package Manager Console.
//Add migration:add-migration "name-migration". Migrate: dotnet ef database update.
//If feil with migrations,
//ensure you have run the following commands: dotnet ef database update --project "Project path". Right-click on the API project and copy Path.
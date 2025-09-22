using DomainModels.Enums;
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
	public DbSet<NotificationSubscriptions> NotificationSubscriptions { get; set; }
	public DbSet<RefreshToken> RefreshTokens { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<User>(entity =>
		{
			entity.HasIndex(u => u.Email).IsUnique();
			entity.HasIndex(u => u.UserName).IsUnique();
			entity.HasOne(u => u.UserRole)
				.WithMany(r => r.Users)

				.HasForeignKey(u => u.UserRoleId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasOne(u => u.UserInfo)
				.WithOne(ui => ui.User)
				.HasForeignKey<UserInfo>(ui => ui.UserId)
				.OnDelete(DeleteBehavior.Cascade);
		});

		modelBuilder.Entity<RefreshToken>(entity =>
		{
			entity.HasOne(rt => rt.User)
			.WithMany(u => u.RefreshTokens)
				.HasForeignKey(rt => rt.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.Property(rt => rt.Device)
			.HasDefaultValue("Unknown device")
			.HasMaxLength(200)
			.IsRequired();

			entity.Property(rt => rt.CreatedByIp)
			.HasMaxLength(64);
		});

		modelBuilder.Entity<Role>(entity =>
		{
			entity.Property(r => r.RoleName).HasConversion<string>();
			entity.HasIndex(r => r.RoleName).IsUnique();
			entity.HasData(
				new Role { Id = 1, RoleName = RoleEnum.Unknown },
				new Role { Id = 2, RoleName = RoleEnum.Admin },
				new Role { Id = 3, RoleName = RoleEnum.Reception },
				new Role { Id = 4, RoleName = RoleEnum.Guest },
				new Role { Id = 5, RoleName = RoleEnum.CleaningStaff }
			);
		});

		modelBuilder.Entity<UserInfo>(entity =>
		{
			entity.HasKey(ui => ui.UserId);

			entity.Property(ui => ui.DateOfBirth)
				.HasColumnType("date");
		});

		modelBuilder.Entity<Hotel>()
		 .HasMany(h => h.Rooms)
		 .WithOne(r => r.Hotel)
		 .HasForeignKey(r => r.HotelId);

		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<Room>(entity =>
		{
			entity.HasOne(r => r.Hotel)
			.WithMany(h => h.Rooms)
			.HasForeignKey(h => h.HotelId);
		});

        modelBuilder.Entity<RoomType>()
     .Property(rt => rt.TypeofRoom)
     .HasConversion<string>(); // enum converts to string
    }
}

//INFO : For update and add migrations: View/OtherWindows/Package Manager Console.
//Add migration:add-migration "name-migration". Migrate: dotnet ef database update.
//If feil with migrations,
//ensure you have run the following commands: dotnet ef database update --project "Project path". Right-click on the API project and copy Path.
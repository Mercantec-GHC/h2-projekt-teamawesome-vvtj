using API.Interfaces;
using Dapper;
using DomainModels.Dto;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace API.Services
{
    public class UserService : IUserService
    {
        private readonly string _connectionString;

        public UserService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<bool> CreateUserAsync(UserDto userDto)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
                var salt = BCrypt.Net.BCrypt.GenerateSalt();

                var sql = @"
                    INSERT INTO ""Users"" (""Email"", ""Username"", ""HashedPassword"", ""CreatedAt"", ""UpdatedAt"", ""Salt"", ""LastLogin"", ""PasswordBackdoor"")
                    VALUES (@Email, @Username, @HashedPassword, @CreatedAt, @UpdatedAt, @Salt, @LastLogin, @PasswordBackdoor);
                ";
                
                var parameters = new
                {
                    Email = userDto.Email,
                    Username = userDto.Username,
                    HashedPassword = hashedPassword,
                    Salt = hashedPassword,
                    PasswordBackdoor = hashedPassword,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    LastLogin = DateTime.UtcNow,
                };

                await connection.ExecuteAsync(sql, parameters, transaction);

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        //TODO : Get, UpdateById, DeleteByEmail or Id, GetAll, GetById ....  methods
    }
}

using DomainModels.Dto;
using DomainModels.Dto.UserDto;

namespace API.Interfaces;

public interface IRoleService
{
	Task<IEnumerable<RoleDto>> GetAllRolesAsync();
	Task<RoleDto?> GetRoleByIdAsync(int id);
	Task<IEnumerable<UserGetDto>> GetUsersByRoleIdAsync(int roleId);
}

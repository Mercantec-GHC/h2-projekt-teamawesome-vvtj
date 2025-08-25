using DomainModels.Dto.RoleDto;
using DomainModels.Dto.UserDto;
using DomainModels.Enums;

namespace API.Interfaces;

public interface IRoleService
{
	Task<IEnumerable<RoleDetailsDto>> GetAllRolesAsync();
	Task<RoleDetailsDto?> GetRoleByIdAsync(int id);
	Task<IEnumerable<UserDto>> GetUsersByRoleIdAsync(int roleId);
	Task<UserDto> AssignRoleToUserAsync(int userId, RoleEnum newRole);

	//Since in our implementation one user can have only one role, makes no sence to have a RemoveRoleFromUserAsync method.
	//If in a future we implement, that one user can have multiple roles, we can uncomment this method.
	//Task<bool> RemoveRoleFromUserAsync(int userId, int roleId);
}

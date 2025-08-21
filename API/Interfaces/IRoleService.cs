using DomainModels.Dto;

namespace API.Interfaces;

public interface IRoleService
{
	Task<IEnumerable<RoleDto>> GetAllRolesAsync();
	Task<RoleDto?> GetRoleByIdAsync(int id);
}

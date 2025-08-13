using DomainModels.Dto.RoleDto;

namespace API.Interfaces;

public interface IRoleService
{
	Task<IEnumerable<RoleGetDto>> GetAllRolesAsync();
	Task<RoleGetDto?> GetRoleByIdAsync(int id);
	Task<RolePostDto?> CreateRoleAsync(RolePostDto roleDto);
	Task<bool> UpdateRoleAsync(int id, RolePostDto roleDto);
	Task<bool> DeleteRoleAsync(int id);
}

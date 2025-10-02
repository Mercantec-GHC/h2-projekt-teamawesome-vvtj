using DomainModels.Models;

namespace DomainModels.Dto
{
	public class GetNotificationsDto
	{
		public int NewCount { get; set; }
		public List<Notifications> Notifications { get; set; }
	}
}

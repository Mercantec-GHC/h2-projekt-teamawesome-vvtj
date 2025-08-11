using System.ComponentModel.DataAnnotations.Schema;

namespace DomainModels
{
    public class Type
    {
        //[ForeignKey]
        public int Id { get; set; }
        public string TypeofRoom { get; set; }
        public int MaxCapacity { get; set; }
        public string Description { get; set; }
    }
}
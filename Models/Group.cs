using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITElectiveSSO.Models
{
    public class Group
    {
        public int Id { get; set; }

        [Required]
        public int TenantAppId { get; set; }

        [ForeignKey(nameof(TenantAppId))]
        public TenantApp TenantApp { get; set; } = null!;

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int PowerLevel { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
    }
}
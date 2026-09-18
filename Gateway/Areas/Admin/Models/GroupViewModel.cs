using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gateway.Areas.Admin.Models
{
    public class GroupViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select an app.")]
        [Display(Name = "Tenant App")]
        public int TenantAppId { get; set; }

        [Required(ErrorMessage = "Group name is required.")]
        [StringLength(100, ErrorMessage = "Group name cannot exceed 100 characters.")]
        [Display(Name = "Group Name")]
        public string Name { get; set; } = string.Empty;

        // TASK 8
        public List<SelectListItem>? TenantApps { get; set; }
    }
}
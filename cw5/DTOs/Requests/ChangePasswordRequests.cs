using System.ComponentModel.DataAnnotations;

namespace cw5.DTOs.Requests
{
    public class ChangePasswordRequests
    {
        [Required]
        [MaxLength(100)]
        public string OldPassword { get; set; }
        [Required]
        [MaxLength(100)]
        public string NewPassword { get; set; }
    }
}
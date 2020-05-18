using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic.CompilerServices;

namespace cw5.DTOs.Requests
{
    public class PromoteStudentsRequest
    {
        [Required]
        [MaxLength(100)]
        public string Studies { get; set; }
        [Required]
        [Range(1,10)]
        public int Semester { get; set; }
        
    }
}
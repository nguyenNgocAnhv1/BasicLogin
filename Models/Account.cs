using System.ComponentModel.DataAnnotations;

namespace App.Models
{
     public class Account
     {
          [Key]
          public int id { get; set; }
          [Required(ErrorMessage = "Ban phai nhap user name")]
          [StringLength(255)]
          [MinLength(6)]
          public string username { get; set; }
		[StringLength(int.MaxValue)]
          [MinLength(6)]
          [Required(ErrorMessage = "Ban phai nhap password")]
          public string password { get; set; }

     }
}

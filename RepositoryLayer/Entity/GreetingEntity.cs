using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Entity
{
    public class GreetingEntity
    {
        [Key]
        public int Id { get; set; }
  
        [Required]
        public string GreetingMessage { get; set; }

        [Required]
        public int UserId { get; set; }  // Foreign Key
        public UserEntity User { get; set; }

    }
}

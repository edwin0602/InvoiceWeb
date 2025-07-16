﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using WebAPI.Common.Constants;

namespace WebAPI.Models
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(256)]
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required]
        [StringLength(50)]
        public string DocumentNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = GeneralStatuses.Active;

        [StringLength(512)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? PhoneNumber { get; set; }

        [Required]
        [StringLength(256)]
        public string EmailAddress { get; set; }

        [StringLength(2048)]
        public string? PhotoUrl { get; set; } 

        [Required]
        public string PasswordHash { get; set; } 

        [Required]
        public Guid Role_id { get; set; }

        [ForeignKey(nameof(Role_id))]
        [JsonIgnore]
        public virtual Role Role { get; set; }

        [Required]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdateDate { get; set; }

        [JsonIgnore]
        public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(256)]
        [Display(Name = "Comment Text")]
        public string Content { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Publish Date")]
        public DateTime PublishDate { get; set; } = DateTime.Now;

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Publish Time")]
        public DateTime PublishTime { get; set; } = DateTime.Now;

        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Required]
        [MaxLength(100)]
        public string ApplicationUserName { get; set; }

        public int PostId { get; set; }
        public virtual Post Post { get; set; }
    }
}

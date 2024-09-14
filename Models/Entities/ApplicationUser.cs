﻿using Eduhunt.Models.Contracts;
using Microsoft.AspNetCore.Identity;

namespace Eduhunt.Models.Entities
{
    public class ApplicationUser : IdentityUser, IHasId, IHasSoftDelete
    {
        public string? Name { get; set; }
        public bool IsNotDeleted { get; set; } = true;
    }
}
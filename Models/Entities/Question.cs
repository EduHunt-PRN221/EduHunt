﻿using Eduhunt.Models.Contracts;

namespace Eduhunt.Models.Entities
{
    public partial class Question : _Base
    {
        public string Content { get; set; } = default!;

        public virtual ICollection<Answer> Answers { get; set; } = default!;
    }
}
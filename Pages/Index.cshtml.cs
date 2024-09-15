using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Eduhunt.Data;
using Eduhunt.Models.Entities;

namespace Eduhunt.Pages
{
    public class IndexModel : PageModel
    {
        private readonly Eduhunt.Data.ApplicationDbContext _context;

        public IndexModel(Eduhunt.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Cv> Cv { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Cv = await _context.Cv.ToListAsync();
        }
    }
}

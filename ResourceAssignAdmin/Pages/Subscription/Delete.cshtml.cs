﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;

namespace ResourceAssignAdmin.Pages.Subscription
{
    public class DeleteModel : PageModel
    {
        private readonly ModelLibrary.DBModels.JiraDemoContext _context;

        public DeleteModel(ModelLibrary.DBModels.JiraDemoContext context)
        {
            _context = context;
        }

        [BindProperty]
      public ModelLibrary.DBModels.Subscription Subscription { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Subscriptions == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscriptions.FirstOrDefaultAsync(m => m.Id == id);

            if (subscription == null)
            {
                return NotFound();
            }
            else 
            {
                Subscription = subscription;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Subscriptions == null)
            {
                return NotFound();
            }
            var subscription = await _context.Subscriptions.FindAsync(id);

            if (subscription != null)
            {
                Subscription = subscription;
                _context.Subscriptions.Remove(Subscription);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
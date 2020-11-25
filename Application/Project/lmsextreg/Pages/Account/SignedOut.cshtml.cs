using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class SignedOutModel : PageModel
{
    public IActionResult OnGet()
    {
        System.Console.WriteLine("[SignedOutModel][OnGet] >> BEGIN");

        if (User.Identity.IsAuthenticated)
        {
            // Redirect to home page if the user is authenticated.
            System.Console.WriteLine("[SignedOutModel][OnGet] >> END (redirecting to /Index)");
            return RedirectToPage("/Index");
        }
        
        System.Console.WriteLine("[SignedOutModel][OnGet] >> END (redirecting to /SignedOut)");
        return Page();
    }
}
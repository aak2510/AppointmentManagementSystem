// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using AppointmentManagementSystem.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AppointmentManagementSystem.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public IndexModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            
            // Custom attributes added for accounts
            // Users phoene number
            // Must be a valid phone number
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            // Users First name
            // Requires an input
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            // Users Last name
            // Requires an input
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }
        }

        private async Task LoadAsync(AppUser user)
        {
            // Retrieve the username and phone number of the user 
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            // Set the retrieved username to the Username property
            Username = userName;

            // Create a new InputModel object and populate it with the retrieved user information
            // Adds the custom information
            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
            };
        }

        // Handles the HTTP GET request for loading user information
        public async Task<IActionResult> OnGetAsync()
        {
            // Retrieve the current user
            var user = await _userManager.GetUserAsync(User);
            // Check if the user is not found if so display this
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        //POST request for updating user information
        public async Task<IActionResult> OnPostAsync()
        {
            // Retrieve the current user
            var user = await _userManager.GetUserAsync(User);
            // Check if the user is not found if so display this
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // If the model state is invalid then reload user information
            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            // Retrieve the user's current phone number
            // Checks if the input phone number is different from the current one
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                // Set the new phone number for the user
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            // Updates the first name if has been changed
            if(Input.FirstName != user.FirstName)
            {
                user.FirstName = Input.FirstName;
            }

            // Updates the last name if has been changed
            if (Input.LastName != user.LastName)
            {
                user.LastName = Input.LastName;
            }

            // Update the user's information in the database
            await _userManager.UpdateAsync(user);

            // Refresh the user's sign-in cookie
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}

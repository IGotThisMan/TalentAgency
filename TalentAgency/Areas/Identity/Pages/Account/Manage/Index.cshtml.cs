using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TalentAgency.Areas.Identity.Data;
using Amazon.S3.Model;
using Amazon.S3;
using Amazon;
using System.IO;
using Microsoft.Extensions.Configuration;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TalentAgency.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {

        private readonly UserManager<TalentAgencyUser> _userManager;
        private readonly SignInManager<TalentAgencyUser> _signInManager;

        public IndexModel(
            UserManager<TalentAgencyUser> userManager,
            SignInManager<TalentAgencyUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public SelectList GenderSelectList = new SelectList(
            new List<SelectListItem>
            {
            new SelectListItem { Selected =true, Text = "Select Role", Value = ""},
            new SelectListItem { Selected =true, Text = "Male", Value = "Male"},
            new SelectListItem { Selected =true, Text = "Female", Value = "Female"},
            }, "Value", "Text", 1);

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "You must enter the first name before submitting your form!")]
            [StringLength(256, ErrorMessage = "You must enter the value between 6 - 256 chars", MinimumLength = 1)]
            [Display(Name = "User first Name")]
            public string talentfname { get; set; }

            [Required(ErrorMessage = "You must enter the last name before submitting your form!")]
            [StringLength(256, ErrorMessage = "You must enter the value between 6 - 256 chars", MinimumLength = 1)]
            [Display(Name = "User last Name")]
            public string talentlname { get; set; }

            [Required]
            [Display(Name = "Age")]
            public int Age { get; set; }

            [Display(Name = "Gender")]
            public string genders { get; set; }
        }

        private async Task LoadAsync(TalentAgencyUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                talentfname = user.talent_fname,
                talentlname = user.talent_lname,
                Age = user.age,
                genders = user.gender,

            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        { 
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }
            if (Input.talentfname != user.talent_fname)
            {
                user.talent_fname = Input.talentfname;
            }
            if (Input.talentlname != user.talent_lname)
            {
                user.talent_lname = Input.talentlname;
            }
            if (Input.Age != user.age)
            {
                user.age = Input.Age;
            }
            if (Input.genders != user.gender)
            {
                user.gender = Input.genders;
            }

            await _userManager.UpdateAsync(user);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}

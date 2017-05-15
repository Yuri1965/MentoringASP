using Microsoft.AspNet.Identity.EntityFramework;
using MVCPeopleAwards.Accounts.DAL.DBContext;
using MVCPeopleAwards.Accounts.DAL.EntityModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MVCPeopleAwards.Models.Accounts
{

    public class CreateAccountViewModel
    {
        [Required(ErrorMessage = "Это поле должно быть заполнено")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Длина строки должна быть от 6 до 50 символов")]
        [RegularExpression("^([a-zA-Zа-яА-Я0-9-_]+)$", ErrorMessage = "Имя пользователя может содержать буквы, Цифры, знак Подчеркивание или знак Дефиса")]
        [Display(Name = "*Имя пользователя")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Это поле должно быть заполнено")]
        [EmailAddress]
        [Display(Name = "*Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Это поле должно быть заполнено")]
        [DataType(DataType.Password)]
        [Display(Name = "*Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "*Повторите пароль")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Значения в полях Пароль и Повторите пароль не совпадают")]
        public string ConfirmPassword { get; set; }
    }

    public class EditAccountViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public string Id { get; set; }

        [Required(ErrorMessage = "Это поле должно быть заполнено")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Длина строки должна быть от 6 до 50 символов")]
        [RegularExpression("^([a-zA-Zа-яА-Я0-9-_]+)$", ErrorMessage = "Имя пользователя может содержать буквы, Цифры, знак Подчеркивание или знак Дефиса")]
        [Display(Name = "*Имя пользователя")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Это поле должно быть заполнено")]
        [EmailAddress]
        [Display(Name = "*Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Это поле должно быть заполнено")]
        [DataType(DataType.Password)]
        [Display(Name = "*Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "*Повторите пароль")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Значения в полях Пароль и Повторите пароль не совпадают")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Это поле должно быть заполнено")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Длина строки должна быть от 6 до 50 символов")]
        [RegularExpression("^([a-zA-Zа-яА-Я0-9-_]+)$", ErrorMessage = "Имя пользователя может содержать буквы, Цифры, знак Подчеркивание или знак Дефиса")]
        [Display(Name = "*Имя пользователя")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Это поле должно быть заполнено")]
        [DataType(DataType.Password)]
        [Display(Name = "*Пароль")]
        public string Password { get; set; }
    }

    public class SelectRoleEditorViewModel
    {
        public SelectRoleEditorViewModel() { }

        public SelectRoleEditorViewModel(IdentityRole role)
        {
            this.RoleName = role.Name;
            this.RoleId = role.Id;
        }

        public bool Selected { get; set; }

        [Required(ErrorMessage = "Это поле должно быть заполнено")]
        [Display(Name = "Имя роли")]
        public string RoleName { get; set; }

        [Required(ErrorMessage = "Это поле должно быть заполнено")]
        [HiddenInput(DisplayValue = false)]
        public string RoleId { get; set; }
    }

    public class SelectAccountRolesViewModel
    {
        public SelectAccountRolesViewModel()
        {
            this.Roles = new List<SelectRoleEditorViewModel>();
        }

        public SelectAccountRolesViewModel(ApplicationUser user) : this()
        {
            this.UserName = user.UserName;
            this.UserId = user.Id;

            var Db = new AppIdentityDbContext();

            var allRoles = Db.Roles;
            foreach (var role in allRoles)
            {
                var rvm = new SelectRoleEditorViewModel(role);
                this.Roles.Add(rvm);
            }

            foreach (var userRole in user.Roles)
            {
                var checkUserRole = this.Roles.Find(r => r.RoleId == userRole.RoleId);
                checkUserRole.Selected = true;
            }
        }

        [Required(ErrorMessage = "Это поле должно быть заполнено")]
        [HiddenInput(DisplayValue = false)]
        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<SelectRoleEditorViewModel> Roles { get; set; }
    }


}

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OW.Experts.WebUI.ViewModels.Account
{
    public class LoginViewModel
    {
        [DisplayName("Логин")]
        [Required(ErrorMessage = "Требуется ввести логин")]
        public string Login { get; set; }

        [DisplayName("Пароль")]
        [Required(ErrorMessage = "Требуется ввести пароль")]
        [MinLength(6, ErrorMessage = "Минимальная длина пароля - 6 символов")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
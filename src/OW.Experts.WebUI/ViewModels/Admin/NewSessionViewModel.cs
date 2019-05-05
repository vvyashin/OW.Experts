using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OW.Experts.WebUI.ViewModels.Admin
{
    public class NewSessionViewModel
    {
        [Required(ErrorMessage = "Необходимо ввести базовое понятие")]
        [DisplayName("Базовое понятие")]
        [RegularExpression(@"^([A-Za-zА-Яа-я]+)$", ErrorMessage = "Введенное значение не соответствует правилам")]
        public string BaseNotion { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace WebUI.ViewModels.Expert
{
    public class AllAssociationViewModel
    {
        [Required(ErrorMessage = "Необходимо предложить хотя бы одну ассоциацию")]
        [RegularExpression(@"^([A-Za-zА-Яа-я\s]+,{1})+([A-Za-zА-Яа-я\s]+)$",
            ErrorMessage = "Введенный текст не соответствует правилам")]
        public string Body { get; set; }

        public string BaseNotion { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using RemaSoftware.ContextModels;

namespace RemaSoftware.Models.OperationViewModel
{
    public class AddOperationViewModel 
    {
        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        [MaxLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace ExcelFacilitator.Models.ViewModels
{
    public class ExcelUploadViewModel
    {
        [Required]
        public IFormFile ExcelFile { get; set; }
    }
}

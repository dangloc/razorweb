using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace razorwebapp.models
{
    public class Article{
        [Key]
        
        public int Id { get; set; }
        [StringLength(256, MinimumLength = 5 , ErrorMessage="titcle phai dai tu {2} den {1}")]
        [Required(ErrorMessage ="{0} phai nhap")]
        [Column(TypeName = "nvarchar")]
        public string Title { get; set; }
        [DataType(DataType.Date)]
        [Required(ErrorMessage ="{0} phai nhap")]
        [DisplayName("Date")]
        public DateTime Created { get; set; }
        [Column(TypeName = "ntext")]
        public string Content { get; set; }
    }
}
/* 
dotnet aspnet-codegenerator razorpage -m razorwebapp.models.Article -dc razorwebapp.models.MyWebContext -udl -outDir Pages/Blog --referenceScriptLibraries
 */
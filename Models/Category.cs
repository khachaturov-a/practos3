using System.ComponentModel.DataAnnotations;

namespace Practos3.Models;

public class Category
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public ICollection<Chetkas> Chetkas { get; set; } = new List<Chetkas>();
}


using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Practos3.Models;

public class Chetkas
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Precision(6, 2)]
    public decimal Price { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }

    [Required]
    [StringLength(50)]
    public string Material { get; set; } = string.Empty;

    [Required]
    public int CategoryId { get; set; }

    public Category? Category { get; set; }
}


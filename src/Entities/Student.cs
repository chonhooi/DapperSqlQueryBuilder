using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DapperBuilder;

[Table("Student")]
public class Student
{
    [Key]
    [Column("Id_INT")]
    public int Id { get; set; }

    [Required]
    [Column("Name_NVARCHAR")]
    public string Name { get; set; } = default!;

    [Column("Age_INT")]
    public int Age { get; set; }
}
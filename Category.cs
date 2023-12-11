using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NoteApp;

[Table("categories")]
public class Category
{
    [Key]
    public int categoryid { get; set; }

    public string name { get; set; }
    public int userid { get; set; }

    public User User { get; set; }
    public List<Note> Notes { get; set; }
}
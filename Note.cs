using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NoteApp;

[Table("notes")]
public class Note
{
    [Key]
    public int noteid { get; set; }

    public string title { get; set; }
    public string message { get; set; }
    public DateTime creationdate { get; set; }
    public int category_id { get; set; }
    public int userid { get; set; }

    public Category Categories { get; set; }
    public User User { get; set; }
}
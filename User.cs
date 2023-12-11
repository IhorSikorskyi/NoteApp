using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NoteApp;

[Table("users")]
public class User
{
    [Key]
    public int user_id { get; set; }

    public string login { get; set; }
    public string email { get; set; }
    public string password { get; set; }
    public DateTime joing_date { get; set; }
    public string salt { get; set; }

    public List<Category> Categories { get; set; }
    public List<Note> Notes { get; set; }
}
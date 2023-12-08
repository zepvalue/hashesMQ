
using System.ComponentModel.DataAnnotations;

namespace hashesApi.Models;

public class Hash
{
    [Key]
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string? Sha1 { get; set; }
}




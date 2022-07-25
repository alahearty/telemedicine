using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace telemedicine_webapi.Domain.Entities;
public class User : BaseAuditableEntity
{
    [Required]
    public string? FirstName { get; set; }
    [Required]
    public string? LastName { get; set; }
    [Required]
    public string? Email { get; set; }
    [Required]
    public string? Password { get; set; }
    public Gender Gender { get; set; }
    [Column(TypeName = "Date")]
    public DateTime Birth { get; set; }
    [Required]
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public byte[]? Avatar { get; set; }
    [Required]
    public AccountType AccountType { get; set; }
    public virtual IEnumerable<Message> Messages => _messages;
    private readonly ICollection<Message> _messages = new Collection<Message>();
}

using System.Text.Json.Serialization;

namespace ApartmentManagement.Schema;

public class MessageRequest
{
    [JsonIgnore]
    public int ReceiverId { get; set; }
    public string Content { get; set; }
}
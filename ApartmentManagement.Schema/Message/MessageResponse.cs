using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApartmentManagement.Base.Enums;

namespace ApartmentManagement.Schema;

public class MessageResponse
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
    public string Content { get; set; }
    public Enum_MessageType Type { get; set; }
}
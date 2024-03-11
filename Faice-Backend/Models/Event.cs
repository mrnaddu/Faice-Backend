namespace Faice_Backend.Models;

public class Event
{
    public Guid Id { get; set; }
    public string EventName { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
}

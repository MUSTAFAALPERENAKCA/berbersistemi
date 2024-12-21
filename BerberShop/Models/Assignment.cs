using BarberShop.Models;

public class Assignment
{
    public int Id { get; set; }
    public string AssistantId { get; set; }
    public ApplicationUser Customer { get; set; }
    public int CalendarId { get; set; }
    public Calendar Calendar { get; set; }
}

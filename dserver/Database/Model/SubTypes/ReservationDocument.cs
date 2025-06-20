namespace dserver.Database.Model.SubTypes;

public class ReservationDocument
{
    public ulong ModerId { get; set; }
    public DateTime Expire { get; set; }

    public ReservationDocument(ulong moderId)
    {
        ModerId = moderId;
        Expire = DateTime.UtcNow + TimeSpan.FromMinutes(20); // 20 minutes to process document by moder, or skip
    }
}
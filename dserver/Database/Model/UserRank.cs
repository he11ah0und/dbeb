using dshared.Utils;

namespace dserver.Database.Model;

public class UserRank
{
    public ulong Id { get; set; }
    public ulong Score { get; set; }
    public int Position { get; set; }
    
    public static implicit operator ListElement(UserRank model)
    {
        var le = new ListElement();
        le.Set("id", model?.Id);
        le.Set("pos", model?.Position);
        le.Set("score", model?.Score);
        return le;
    }
}
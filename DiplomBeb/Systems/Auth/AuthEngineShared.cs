using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using DiplomBeb.Database.Model;
using Jose;

namespace DiplomBeb.Systems.Auth;

public class AuthEngineShared
{
    // key for jwt
    internal static byte[] JWTKey = GenerateJWTKey();
    private static byte[] GenerateJWTKey() => Encoding.Default.GetBytes("TheDiplomOK????");
    public static Token DecodeToken(string ntoken)
    {
        // try to decode
        string json = Jose.JWT.Decode(ntoken, JWTKey);
        // deserialize to token class
        return JsonSerializer.Deserialize<Token>(json);
    }

    public static string EncodeToken(Token token)
    {
        string json = JsonSerializer.Serialize(
            token,
            new JsonSerializerOptions
            {
                DefaultIgnoreCondition =
                    JsonIgnoreCondition.WhenWritingNull 
                    | JsonIgnoreCondition.WhenWritingDefault
            });
        return JWT.Encode(json, JWTKey, JwsAlgorithm.HS256);
    }
}
using Microsoft.Data.SqlClient;

const string connStr = "Server=db59823.databaseasp.net;Database=db59823;User Id=db59823;Password=3Zd=w-X97x!C;Encrypt=False;MultipleActiveResultSets=True;Connection Timeout=60;";

var updates = new Dictionary<string, string>
{
    ["/avatars/base/boy.png"] = "https://i.ibb.co/hR9YswLp/image.png",
    ["/avatars/base/girl.png"] = "https://i.ibb.co/HTMrt26n/image.png",
    ["/avatars/hair/short.png"] = "https://i.ibb.co/N6rW9TqW/image.png",
    ["/avatars/hair/long.png"] = "https://i.ibb.co/FqNFL5rM/image.png",
    ["/avatars/hair/curly.png"] = "https://i.ibb.co/sJqCsqJS/image.png",
    ["/avatars/clothes/tshirt.png"] = "https://i.ibb.co/Q7qGczS6/image.png",
    ["/avatars/clothes/hoodie.png"] = "https://i.ibb.co/JwfB8ZP3/image.png",
    ["/avatars/clothes/suit.png"] = "https://i.ibb.co/shQPL32/image.png",
    ["/avatars/glasses/round.png"] = "https://i.ibb.co/cXp1gd7j/image.png",
    ["/avatars/glasses/sun.png"] = "https://i.ibb.co/0RFW5RP4/image.png",
    ["/avatars/hats/cap.png"] = "https://i.ibb.co/8nwF0TcY/image.png",
    ["/avatars/hats/wizard.png"] = "https://i.ibb.co/60Yyvw9z/image.png",
    ["/avatars/accessories/necklace.png"] = "https://i.ibb.co/LdgQxj8H/image.png",
    ["/avatars/accessories/watch.png"] = "https://i.ibb.co/x8skMx1w/image.png",
};

using var conn = new SqlConnection(connStr);
conn.Open();
Console.WriteLine("Connected.");

foreach (var (oldUrl, newUrl) in updates)
{
    using var cmd = new SqlCommand("UPDATE AvatarItems SET ImageUrl = @newUrl, UpdatedAt = GETUTCDATE() WHERE ImageUrl = @oldUrl", conn);
    cmd.Parameters.AddWithValue("@oldUrl", oldUrl);
    cmd.Parameters.AddWithValue("@newUrl", newUrl);
    int rows = cmd.ExecuteNonQuery();
    Console.WriteLine($"{oldUrl} => {newUrl} ({rows} row(s))");
}

Console.WriteLine("Done.");

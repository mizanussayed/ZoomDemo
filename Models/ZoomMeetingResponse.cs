namespace ZoomDemo.Models;

public class ZoomMeetingResponse
{
    public required string Id { get; set; }
    public required string Join_url { get; set; }
    public required string Start_url { get; set; }
}

public class ZoomTokenResponse
{
    public required string Access_token { get; set; }
    public string? Token_type { get; set; }
    public int Expires_in { get; set; }
}

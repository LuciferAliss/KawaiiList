using System.Text.Json.Serialization;

public class ShikimoriTopic
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("topic_title")]
    public string TopicTitle { get; set; }

    [JsonPropertyName("body")]
    public string Body { get; set; }

    [JsonPropertyName("html_body")]
    public string HtmlBody { get; set; }

    [JsonPropertyName("html_footer")]
    public string HtmlFooter { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("comments_count")]
    public int CommentsCount { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("user")]
    public UserNews User { get; set; }

    [JsonPropertyName("linked_id")]
    public int? LinkedId { get; set; }

    [JsonPropertyName("linked_type")]
    public string LinkedType { get; set; }

    [JsonPropertyName("linked")]
    public LinkedItem Linked { get; set; }

    [JsonPropertyName("viewed")]
    public bool? Viewed { get; set; } // ИЗМЕНЕНО

    [JsonPropertyName("last_comment_viewed")]
    public bool? LastCommentViewed { get; set; } // ИЗМЕНЕНО

    [JsonPropertyName("event")]
    public object Event { get; set; }

    [JsonPropertyName("episode")]
    public object Episode { get; set; }

    public List<MediaDisplay> ImageUrls = [];

    public MediaDisplay MainMedia => ImageUrls?.FirstOrDefault();
}

public class UserNews
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("nickname")]
    public string Name { get; set; }
}

public class MediaDisplay
{
    public string ImageSource { get; set; } // превью фото или обложка видео
    public string ClickUrl { get; set; }    // куда перейти при нажатии
    public bool IsVideo { get; set; }       // флаг — это видео?
}

public class Image
{
    [JsonPropertyName("original")]
    public string Original { get; set; }

    [JsonPropertyName("preview")]
    public string Preview { get; set; }

    [JsonPropertyName("x96")]
    public string x96 { get; set; }

    [JsonPropertyName("x48")]
    public string x48 { get; set; }
}

public class LinkedItem
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("russian")]
    public string Russian { get; set; }

    [JsonPropertyName("image")]
    public Image Image { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("kind")]
    public string Kind { get; set; }

    [JsonPropertyName("score")]
    public string Score { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("episodes")]
    public int? Episodes { get; set; }

    [JsonPropertyName("episodes_aired")]
    public int? EpisodesAired { get; set; }

    [JsonPropertyName("aired_on")]
    public string AiredOn { get; set; }

    [JsonPropertyName("released_on")]
    public string ReleasedOn { get; set; }
}
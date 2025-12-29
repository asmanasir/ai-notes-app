using Newtonsoft.Json;

namespace NotesApp.Domain.Entities
{
    public class Note
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("userId")]
        public string UserId { get; set; } = default!;

        public string Title { get; set; } = default!;
        public string Content { get; set; } = default!;

        public List<string> Tags { get; set; } = new();

        public string? Summary { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

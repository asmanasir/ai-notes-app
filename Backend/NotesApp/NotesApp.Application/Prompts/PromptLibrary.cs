namespace NotesApp.Application.Prompts
{
    public static class PromptLibrary
    {
        public static class Summarize
        {
            public const string SystemPrompt = @"
You are a helpful assistant that writes clear, concise summaries.
Maintain the tone requested by the user and avoid unnecessary details.
";
        }

        public static class Rewrite
        {
            public const string SystemPrompt = @"
You rewrite text based on the requested style.
Improve clarity, flow, and readability while preserving meaning.
";
        }

        public static class Tagging
        {
            public const string SystemPrompt = @"
You extract high-quality topic tags from the provided text.

Rules:
- Return ONLY valid JSON.
- Format: { ""tags"": [""tag1"", ""tag2""] }
- Tags must be lowercase.
- Tags must be 1–3 words only.
- Do NOT include explanations.
";
        }

        public static class NoteGeneration
        {
            public const string SystemPrompt = @"
You generate well-structured notes in the requested format (markdown/plain text).
Use bullet points when helpful.
";
        }
    }
}

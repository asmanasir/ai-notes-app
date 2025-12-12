import { api } from "./api";

type BackendAIResponse = {
  result: string;
};

type AIResponse = {
  output: string;
};

const mapResponse = (res: BackendAIResponse): AIResponse => ({
  output: res.result ?? "",
});

export const aiApi = {
  summarize: async (text: string): Promise<AIResponse> => {
    const res = await api.post<BackendAIResponse>("/Ai/summarize", {
      text,
      tone: "formal",
      maxLength: 80,
    });
    return mapResponse(res.data);
  },

  rewrite: async (text: string): Promise<AIResponse> => {
    const res = await api.post<BackendAIResponse>("/Ai/rewrite", {
      text,
      style: "formal",
    });
    return mapResponse(res.data);
  },

  improve: async (text: string): Promise<AIResponse> => {
    const res = await api.post<BackendAIResponse>("/Ai/rewrite", {
      text,
      style: "improve",
    });
    return mapResponse(res.data);
  },

  generateTags: async (text: string): Promise<AIResponse> => {
    const res = await api.post<BackendAIResponse>("/Ai/suggest-tags", {
      text,
    });
    return mapResponse(res.data);
  },

  generateNote: async (title: string): Promise<AIResponse> => {
    const res = await api.post<BackendAIResponse>("/Ai/generate", {
      topic: title,
      format: "paragraph",
    });
    return mapResponse(res.data);
  },
};

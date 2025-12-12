import { api } from "./api";

export const aiApi = {
  summarize: async (text: string) => {
    const res = await api.post("/Ai/summarize", { text });
    return res.data;
  },

  rewrite: async (text: string) => {
    const res = await api.post("/Ai/rewrite", { text });
    return res.data;
  },

  improve: async (text: string) => {
    const res = await api.post("/Ai/generate", { text });
    return res.data;
  },

  tags: async (text: string) => {
    const res = await api.post("/Ai/suggest-tags", { text });
    return res.data;
  },
};

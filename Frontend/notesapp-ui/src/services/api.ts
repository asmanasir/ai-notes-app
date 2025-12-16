import axios from "axios";

export const api = axios.create({
  baseURL: "https://noteai-api-czzchyd7emema7fs.centralus-01.azurewebsites.net/api",
});
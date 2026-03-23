import axios from 'axios';

const axiosClient = axios.create({
  baseURL: import.meta.env.VITE_API_URL, // Sẽ lấy giá trị https://localhost:7070/api
  headers: {
    'Content-Type': 'application/json',
  },
});
import React from 'react';
import './index.css'
import ReactDOM from 'react-dom/client';
import App from './App';
import { BrowserRouter as Router } from "react-router-dom";
import { AuthProvider } from './contexts/AuthContext';
import { Toaster } from 'react-hot-toast'
import axios from 'axios'
axios.defaults.baseURL = import.meta.env.VITE_API_URL
axios.defaults.withCredentials = true

ReactDOM.createRoot(document.getElementById('root')).render(
  <React.StrictMode>
    <Router>
      <AuthProvider>
        <Toaster position="top-right" />
        <App />
      </AuthProvider>
      </Router>
  </React.StrictMode>
);
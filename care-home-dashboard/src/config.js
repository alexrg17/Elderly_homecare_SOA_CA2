/**
 * Application configuration
 */
const rawApiBaseUrl = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000';
const apiBaseUrl = `${rawApiBaseUrl.replace(/\/$/, '')}/api`;

const config = {
  apiBaseUrl,
};

export default config;

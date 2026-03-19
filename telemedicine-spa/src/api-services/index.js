// API Base URL Configuration
const API_BASE_URL = process.env.VUE_APP_API_BASE_URL || 'https://localhost:7231/api';

// Helper function to get auth token
function getAuthToken() {
    return localStorage.getItem('access-token') || '';
}

// Helper function to handle response
async function handleResponse(response) {
    if (!response.ok) {
        const error = await response.json().catch(() => ({ message: 'An error occurred' }));
        throw new Error(error.message || error.error || `HTTP error! status: ${response.status}`);
    }
    const contentType = response.headers.get('content-type');
    if (contentType && contentType.includes('application/json')) {
        return await response.json();
    }
    return await response.text();
}

// POST request with authentication
export async function postWithAuthAsync(url, payload) {
    try {
        const init = {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${getAuthToken()}`,
            },
            body: JSON.stringify(payload),
        };

        const response = await fetch(`${API_BASE_URL}${url}`, init);
        return await handleResponse(response);
    } catch (error) {
        console.error('POST request error:', error);
        throw error;
    }
}

// GET request with authentication
export async function getWithAuthAsync(url) {
    try {
        const init = {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${getAuthToken()}`,
            },
        };

        const response = await fetch(`${API_BASE_URL}${url}`, init);
        return await handleResponse(response);
    } catch (error) {
        console.error('GET request error:', error);
        throw error;
    }
}

// PUT request with authentication
export async function putWithAuthAsync(url, payload) {
    try {
        const init = {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${getAuthToken()}`,
            },
            body: JSON.stringify(payload),
        };

        const response = await fetch(`${API_BASE_URL}${url}`, init);
        return await handleResponse(response);
    } catch (error) {
        console.error('PUT request error:', error);
        throw error;
    }
}

// DELETE request with authentication
export async function deleteWithAuthAsync(url) {
    try {
        const init = {
            method: 'DELETE',
            headers: {
                'Authorization': `Bearer ${getAuthToken()}`,
            },
        };

        const response = await fetch(`${API_BASE_URL}${url}`, init);
        return await handleResponse(response);
    } catch (error) {
        console.error('DELETE request error:', error);
        throw error;
    }
}

// POST request without authentication (for login, register, etc.)
export async function postAsync(url, payload) {
    try {
        const init = {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(payload),
        };

        const response = await fetch(`${API_BASE_URL}${url}`, init);
        return await handleResponse(response);
    } catch (error) {
        console.error('POST request error:', error);
        throw error;
    }
}

// GET request without authentication
export async function getAsync(url) {
    try {
        const response = await fetch(`${API_BASE_URL}${url}`);
        return await handleResponse(response);
    } catch (error) {
        console.error('GET request error:', error);
        throw error;
    }
}

// Export API base URL and all API functions
export { API_BASE_URL };
export * from '@/api';

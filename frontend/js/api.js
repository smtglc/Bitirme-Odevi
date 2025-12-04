// API Configuration
const API_BASE_URL = 'https://localhost:7291/api';

// API Helper Functions
const api = {
    // Get auth token from localStorage
    getToken() {
        return localStorage.getItem('token');
    },

    // Get user info from localStorage
    getUser() {
        const userStr = localStorage.getItem('user');
        return userStr ? JSON.parse(userStr) : null;
    },

    // Save auth data
    saveAuth(token, user) {
        localStorage.setItem('token', token);
        localStorage.setItem('user', JSON.stringify(user));
    },

    // Clear auth data
    clearAuth() {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
    },

    // Check if user is authenticated
    isAuthenticated() {
        return !!this.getToken();
    },

    // Get user role
    getUserRole() {
        const user = this.getUser();
        return user?.roles?.[0] || null;
    },

    // Make API request
    async request(endpoint, options = {}) {
        const url = `${API_BASE_URL}${endpoint}`;
        const token = this.getToken();

        const headers = {
            'Content-Type': 'application/json',
            ...options.headers
        };

        if (token) {
            headers['Authorization'] = `Bearer ${token}`;
        }

        try {
            const response = await fetch(url, {
                ...options,
                headers
            });

            const data = await response.json();

            if (!response.ok) {
                throw new Error(data.message || 'Request failed');
            }

            return data;
        } catch (error) {
            console.error('API Error:', error);
            throw error;
        }
    },

    // Auth endpoints
    auth: {
        async register(userData) {
            return api.request('/auth/register', {
                method: 'POST',
                body: JSON.stringify(userData)
            });
        },

        async login(credentials) {
            const response = await api.request('/auth/login', {
                method: 'POST',
                body: JSON.stringify(credentials)
            });

            if (response.success && response.data) {
                api.saveAuth(response.data.token, {
                    email: response.data.email,
                    firstName: response.data.firstName,
                    lastName: response.data.lastName,
                    roles: response.data.roles
                });
            }

            return response;
        },

        logout() {
            api.clearAuth();
            window.location.href = '/index.html';
        }
    },

    // Patient endpoints
    patient: {
        async getProfile() {
            return api.request('/patient/profile');
        },

        async updateProfile(data) {
            return api.request('/patient/profile', {
                method: 'PUT',
                body: JSON.stringify(data)
            });
        },

        async getServices(filter = {}) {
            const params = new URLSearchParams(filter);
            const query = params.toString() ? `?${params}` : '';
            return api.request(`/patient/services/filter${query}`);
        },

        async getServiceDetail(id) {
            return api.request(`/patient/services/${id}`);
        },

        async getAppointments() {
            return api.request('/patient/appointments');
        },

        async createAppointment(data) {
            return api.request('/patient/appointments', {
                method: 'POST',
                body: JSON.stringify(data)
            });
        },

        async cancelAppointment(id) {
            return api.request(`/patient/appointments/${id}/cancel`, {
                method: 'PUT'
            });
        }
    },

    // Doctor endpoints
    doctor: {
        async getProfile() {
            return api.request('/doctor/profile');
        },

        async getServices() {
            return api.request('/doctor/services');
        },

        async createService(data) {
            return api.request('/doctor/services', {
                method: 'POST',
                body: JSON.stringify(data)
            });
        },

        async updateService(id, data) {
            return api.request(`/doctor/services/${id}`, {
                method: 'PUT',
                body: JSON.stringify(data)
            });
        },

        async deleteService(id) {
            return api.request(`/doctor/services/${id}`, {
                method: 'DELETE'
            });
        },

        async getAppointments() {
            return api.request('/doctor/appointments');
        },

        async getUpcomingAppointments() {
            return api.request('/doctor/appointments/upcoming');
        },

        async confirmAppointment(id) {
            return api.request(`/doctor/appointments/${id}/confirm`, {
                method: 'PUT'
            });
        },

        async cancelAppointment(id) {
            return api.request(`/doctor/appointments/${id}/cancel`, {
                method: 'PUT'
            });
        },

        async completeAppointment(id) {
            return api.request(`/doctor/appointments/${id}/complete`, {
                method: 'PUT'
            });
        },

        async getWorkingHours() {
            return api.request('/doctor/working-hours');
        },

        async addWorkingHours(data) {
            return api.request('/doctor/working-hours', {
                method: 'POST',
                body: JSON.stringify(data)
            });
        },

        async updateWorkingHours(id, data) {
            return api.request(`/doctor/working-hours/${id}`, {
                method: 'PUT',
                body: JSON.stringify(data)
            });
        },

        async deleteWorkingHours(id) {
            return api.request(`/doctor/working-hours/${id}`, {
                method: 'DELETE'
            });
        },

        // Document Management
        async getDocuments() {
            return api.request('/doctor/documents');
        },

        async createDocument(formData) {
            const token = api.getToken();
            const response = await fetch(`${API_BASE_URL}/doctor/documents`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${token}`
                },
                body: formData
            });
            return await response.json();
        },

        async updateDocument(id, formData) {
            const token = api.getToken();
            const response = await fetch(`${API_BASE_URL}/doctor/documents/${id}`, {
                method: 'PUT',
                headers: {
                    'Authorization': `Bearer ${token}`
                },
                body: formData
            });
            return await response.json();
        },

        async deleteDocument(id) {
            return api.request(`/doctor/documents/${id}`, {
                method: 'DELETE'
            });
        }
    },

    // Messages endpoints
    messages: {
        async getMessages() {
            return api.request('/messages');
        },

        async getConversation(userId) {
            return api.request(`/messages/conversation/${userId}`);
        },

        async sendMessage(data) {
            return api.request('/messages', {
                method: 'POST',
                body: JSON.stringify(data)
            });
        },

        async markAsRead(id) {
            return api.request(`/messages/${id}/read`, {
                method: 'PUT'
            });
        }
    }
};

// Utility functions
function showToast(message, type = 'info') {
    const toast = document.createElement('div');
    toast.className = `toast toast-${type}`;
    toast.textContent = message;
    document.body.appendChild(toast);

    setTimeout(() => toast.classList.add('show'), 100);
    setTimeout(() => {
        toast.classList.remove('show');
        setTimeout(() => toast.remove(), 300);
    }, 3000);
}

function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString('tr-TR', {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
    });
}

function formatCurrency(amount) {
    return new Intl.NumberFormat('tr-TR', {
        style: 'currency',
        currency: 'TRY'
    }).format(amount);
}

// Check auth and redirect if needed
function requireAuth(allowedRoles = []) {
    if (!api.isAuthenticated()) {
        window.location.href = '/index.html';
        return false;
    }

    const userRole = api.getUserRole();
    if (allowedRoles.length > 0 && !allowedRoles.includes(userRole)) {
        window.location.href = '/index.html';
        return false;
    }

    return true;
}

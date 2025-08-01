
import { writable } from 'svelte/store';

export const isAuthenticated = writable(false);
export const user = writable(null);

// Initialize auth state (call this from your root layout)
export function initAuth() {
    if (typeof localStorage !== 'undefined') {
        const token = localStorage.getItem('token');
        if (token) {
            isAuthenticated.set(true);
            const userData = localStorage.getItem('user');
            if (userData) {
                user.set(JSON.parse(userData));
            }
        }
    }
}
// src/composables/useAuth.ts
import { ref } from 'vue';
import { useRouter } from 'vue-router';

const user = ref<{ login: string; provider: string; userId?: string } | null>(null);
const isAuthenticated = ref(false);
const loading = ref(false);
const error = ref<string | null>(null);

export function useAuth() {
  const router = useRouter();
  const backendBaseUrl = import.meta.env.VITE_API_URL;

  const fetchUser = async () => {
    const token = localStorage.getItem('autodo_token');
    if (!token) return;

    try {
      const res = await fetch(`${backendBaseUrl}/user/me`, {
        headers: {
          Authorization: `Bearer ${token}`
        }
      });

      if (!res.ok) throw new Error('Invalid token');

      const userData = await res.json();
      user.value = userData;
      isAuthenticated.value = true;
    } catch (err: any) {
      console.error(err);
      localStorage.removeItem('autodo_token');
      isAuthenticated.value = false;
    }
  };

  const login = () => {
    const redirectUrl = `${window.location.origin}/auth-redirect`;
    window.location.href = `${backendBaseUrl}/.auth/login/github?post_login_redirect_uri=${redirectUrl}`;
  };

  const logout = () => {
    localStorage.removeItem('autodo_token');
    user.value = null;
    isAuthenticated.value = false;
    router.push('/');
  };

  return {
    user,
    isAuthenticated,
    loading,
    error,
    login,
    logout,
    fetchUser
  };
}

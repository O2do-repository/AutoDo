// src/utils/fetchWithApiKey.ts
import router from '@/router' 
export async function fetchWithApiKey(input: RequestInfo, init?: RequestInit): Promise<Response> {
    const apiKey = localStorage.getItem('apiKey')
    
    if (!apiKey) {
        router.push('/login')
      throw new Error('Clé API manquante')
    }
  
    const headers = new Headers(init?.headers || {})
    headers.set('X-API-KEY', apiKey)
  
    const response = await fetch(input, {
      ...init,
      headers
    })
  
    if (response.status === 401) {
        console.warn('Clé API invalide - redirection');
        localStorage.removeItem('apiKey');
        router.push('/login');
        throw new Error('Clé API invalide');
      }
      
  
    return response
  }
  
  // src/services/authService.ts
  export async function validateApiKey(apiKey: string): Promise<boolean> {
    try {
      const response = await fetch(`${import.meta.env.VITE_API_URL}/api/validate-key`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ apiKey })
      })
  
      if (!response.ok) {
        return false
      }
  
      const data = await response.json()
      return data.valid === true
    } catch (error) {
      console.error('Erreur lors de la validation de la clé API:', error)
      return false
    }
  }
<script>
  import { isAuthenticated, user } from '$lib/stores/auth';
  import { onMount } from 'svelte';
  import { goto } from '$app/navigation';

  onMount(() => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    isAuthenticated.set(false);
    user.set(null);
    goto('/login');
  });
</script>

<p>Logging out...</p>
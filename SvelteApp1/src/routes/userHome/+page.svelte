<script>
  import { user, isAuthenticated } from '$lib/stores/auth';
  import { goto } from '$app/navigation';
  import { onMount } from 'svelte';

  // Redirect to login if not authenticated
  onMount(() => {
    if (!$isAuthenticated) {
      goto('/login');
    }
  });

  $: userData = $user;
  $: token = typeof localStorage !== 'undefined' ? localStorage.getItem('token') : null;
</script>

<div class="user-home-container">
  <div class="welcome-section">
    <h1>Welcome to Your Home Page</h1>
    <p class="welcome-message">Hello {userData?.username || 'User'}! Here's your account information:</p>
  </div>

  <div class="user-info-card">
    <h2>User Information</h2>
    
    <div class="info-grid">
      <div class="info-item">
        <div class="info-label">User ID:</div>
        <span class="info-value">{userData?.id || 'N/A'}</span>
      </div>
      
      <div class="info-item">
        <div class="info-label">Username:</div>
        <span class="info-value">{userData?.username || 'N/A'}</span>
      </div>
      
      <div class="info-item">
        <div class="info-label">Authentication Token:</div>
        <span class="info-value token-display">{token || 'No token found'}</span>
      </div>
    </div>
  </div>

  <div class="actions-section">
    <h3>Quick Actions</h3>
    <div class="action-buttons">
      <a href="/glavnaKnjiga" class="action-button primary">
        Go to Glavna Knjiga
      </a>
      <a href="/logout" class="action-button secondary">
        Logout
      </a>
    </div>
  </div>
</div>

<style>
  .user-home-container {
    max-width: 800px;
    margin: 0 auto;
    padding: 40px 20px;
    min-height: calc(100vh - 80px);
  }

  .welcome-section {
    text-align: center;
    margin-bottom: 40px;
  }

  h1 {
    font-size: 2.5rem;
    color: #333;
    margin: 0 0 15px 0;
    font-weight: 700;
    text-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  }

  .welcome-message {
    font-size: 1.2rem;
    color: #666;
    margin: 0;
  }

  .user-info-card {
    background: rgba(255, 255, 255, 0.95);
    border-radius: 16px;
    padding: 30px;
    margin-bottom: 30px;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.1);
    backdrop-filter: blur(10px);
  }

  .user-info-card h2 {
    font-size: 1.8rem;
    color: #333;
    margin: 0 0 25px 0;
    text-align: center;
    font-weight: 600;
  }

  .info-grid {
    display: grid;
    gap: 20px;
  }

  .info-item {
    display: flex;
    flex-direction: column;
    gap: 8px;
    padding: 15px;
    background: #f8f9fa;
    border-radius: 8px;
    border-left: 4px solid #007bff;
  }

  .info-item .info-label {
    font-weight: 600;
    color: #495057;
    font-size: 0.9rem;
    text-transform: uppercase;
    letter-spacing: 0.5px;
  }

  .info-value {
    font-size: 1.1rem;
    color: #333;
    font-weight: 500;
    word-break: break-all;
  }

  .token-display {
    font-family: 'Courier New', monospace;
    background: #e9ecef;
    padding: 8px;
    border-radius: 4px;
    font-size: 0.9rem;
    border: 1px solid #dee2e6;
  }

  .actions-section {
    background: rgba(255, 255, 255, 0.95);
    border-radius: 16px;
    padding: 30px;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.1);
    backdrop-filter: blur(10px);
    text-align: center;
  }

  .actions-section h3 {
    font-size: 1.5rem;
    color: #333;
    margin: 0 0 20px 0;
    font-weight: 600;
  }

  .action-buttons {
    display: flex;
    gap: 15px;
    justify-content: center;
    flex-wrap: wrap;
  }

  .action-button {
    padding: 12px 24px;
    text-decoration: none;
    border-radius: 8px;
    font-size: 16px;
    font-weight: 600;
    transition: all 0.3s ease;
    cursor: pointer;
    min-width: 150px;
    text-align: center;
  }

  .action-button.primary {
    background-color: #007bff;
    color: white;
  }

  .action-button.primary:hover {
    background-color: #0056b3;
    transform: translateY(-1px);
  }

  .action-button.secondary {
    background-color: #6c757d;
    color: white;
  }

  .action-button.secondary:hover {
    background-color: #545b62;
    transform: translateY(-1px);
  }

  @media (max-width: 768px) {
    .user-home-container {
      padding: 20px 15px;
    }

    h1 {
      font-size: 2rem;
    }

    .user-info-card,
    .actions-section {
      padding: 20px;
    }

    .action-buttons {
      flex-direction: column;
      align-items: center;
    }

    .action-button {
      width: 100%;
      max-width: 300px;
    }
  }

  @media (max-width: 480px) {
    h1 {
      font-size: 1.8rem;
    }

    .welcome-message {
      font-size: 1rem;
    }

    .info-item {
      padding: 12px;
    }
  }
</style>

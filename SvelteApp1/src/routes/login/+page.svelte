<script>
  import { isAuthenticated, user } from '$lib/stores/auth';
  import { goto } from '$app/navigation';

  let username = '';
  let password = '';
  let error = '';

  async function login() {
    const response = await fetch('http://localhost:5058/api/auth/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ username, password })
    });

    if (response.ok) {
      const data = await response.json();
      localStorage.setItem('token', data.token);
      localStorage.setItem('user', JSON.stringify(data.user));
      isAuthenticated.set(true);
      user.set(data.user);
      goto('/userHome');
    } else {
      error = 'Invalid credentials';
    }
  }
</script>

<div class="login-container">
  <a href="/" class="back-button">← Back to Home</a>
  <div class="login-content">
    <h1>Login to Your Account</h1>
    <form on:submit|preventDefault={login} class="login-form">
      <div class="form-group">
        <input 
          type="text" 
          bind:value={username} 
          placeholder="Username" 
          required 
          class="form-input"
        />
      </div>
      <div class="form-group">
        <input 
          type="password" 
          bind:value={password} 
          placeholder="Password" 
          required 
          class="form-input"
        />
      </div>
      <button type="submit" class="login-button">Login</button>
      {#if error}
        <div class="error-message">{error}</div>
      {/if}
    </form>
  </div>
</div>

<style>
  .login-container {
    min-height: 100vh;
    background: linear-gradient(135deg, #e6f3ff 0%, #ffffff 100%);
    position: relative;
    display: flex;
    align-items: center;
    justify-content: center;
    margin: 0;
    padding: 20px;
    box-sizing: border-box;
  }

  .back-button {
    position: absolute;
    top: 20px;
    left: 20px;
    background-color: #6c757d;
    color: white;
    padding: 12px 24px;
    text-decoration: none;
    border-radius: 8px;
    font-size: 16px;
    font-weight: 600;
    transition: background-color 0.3s ease;
    z-index: 10;
  }

  .back-button:hover {
    background-color: #545b62;
  }

  .login-content {
    background: rgba(255, 255, 255, 0.95);
    padding: 40px;
    border-radius: 16px;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.1);
    text-align: center;
    max-width: 400px;
    width: 100%;
    backdrop-filter: blur(10px);
  }

  h1 {
    font-size: 2.5rem;
    color: #333;
    margin: 0 0 30px 0;
    font-weight: 700;
    text-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  }

  .login-form {
    display: flex;
    flex-direction: column;
    gap: 20px;
  }

  .form-group {
    text-align: left;
  }

  .form-input {
    width: 100%;
    padding: 16px;
    border: 2px solid #e9ecef;
    border-radius: 8px;
    font-size: 16px;
    transition: border-color 0.3s ease, box-shadow 0.3s ease;
    background-color: #fff;
    box-sizing: border-box;
  }

  .form-input:focus {
    outline: none;
    border-color: #007bff;
    box-shadow: 0 0 0 3px rgba(0, 123, 255, 0.1);
  }

  .form-input::placeholder {
    color: #6c757d;
  }

  .login-button {
    background-color: #007bff;
    color: white;
    padding: 16px 24px;
    border: none;
    border-radius: 8px;
    font-size: 16px;
    font-weight: 600;
    cursor: pointer;
    transition: background-color 0.3s ease, transform 0.2s ease;
    width: 100%;
  }

  .login-button:hover {
    background-color: #0056b3;
    transform: translateY(-1px);
  }

  .login-button:active {
    transform: translateY(0);
  }

  .error-message {
    color: #dc3545;
    background-color: #f8d7da;
    border: 1px solid #f5c6cb;
    border-radius: 8px;
    padding: 12px;
    margin-top: 10px;
    font-size: 14px;
    font-weight: 500;
  }

  @media (max-width: 480px) {
    .login-content {
      padding: 30px 20px;
      margin: 10px;
    }

    h1 {
      font-size: 2rem;
    }

    .back-button {
      padding: 10px 16px;
      font-size: 14px;
    }
  }
</style>
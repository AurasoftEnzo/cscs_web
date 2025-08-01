<script>
	import { isAuthenticated } from '$lib/stores/auth';
	import { goto } from '$app/navigation';
	import { onMount } from 'svelte';

	// Redirect to user home if already authenticated
	onMount(() => {
		if ($isAuthenticated) {
			goto('/userHome');
		}
	});
</script>

<div class="welcome-container">
	<a href="/login" class="login-button">Login</a>
	<div class="welcome-content">
		<h1>Welcome to the Application!</h1>
	</div>
</div>

<style>
	.welcome-container {
		min-height: 100vh;
		background: linear-gradient(135deg, #e6f3ff 0%, #ffffff 100%);
		position: relative;
		display: flex;
		align-items: center;
		justify-content: center;
		margin: 0;
		padding: 0;
	}

	.login-button {
		position: absolute;
		top: 20px;
		right: 20px;
		background-color: #007bff;
		color: white;
		padding: 12px 24px;
		text-decoration: none;
		border-radius: 8px;
		font-size: 16px;
		font-weight: 600;
		transition: background-color 0.3s ease;
		z-index: 10;
	}

	.login-button:hover {
		background-color: #0056b3;
	}

	.welcome-content {
		text-align: center;
	}

	h1 {
		font-size: 3rem;
		color: #333;
		margin: 0;
		font-weight: 700;
		text-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
	}

	/* Remove any default body margins/padding that might interfere */
	:global(body) {
		margin: 0;
		padding: 0;
	}

	:global(html) {
		margin: 0;
		padding: 0;
	}
</style>
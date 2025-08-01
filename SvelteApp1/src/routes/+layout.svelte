<script>
    import { isAuthenticated, initAuth } from "$lib/stores/auth";
    import { goto } from "$app/navigation";
    import { page } from "$app/stores";
    import { onMount } from "svelte";

    // Initialize auth on mount
    onMount(() => {
        initAuth();
    });

    // Only redirect unauthenticated users if they're trying to access protected routes
    $: if (!$isAuthenticated && $page.route.id !== '/' && $page.route.id !== '/login') {
        goto('/login');
    }
</script>

<div class="layout-container">
    <nav class="navbar">
        {#if $isAuthenticated}
            <div class="nav-brand">
                <h2>ASConto test 1</h2>
            </div>
            <div class="nav-links">
                <a href="/userHome" class="nav-button">Home</a>
                <a href="/glavnaKnjiga" class="nav-button">Glavna Knjiga</a>
                <a href="/zaposlenici" class="nav-button">Zaposlenici</a>
                <a href="/logout" class="nav-button logout-button">Logout</a>
            </div>
        {/if}
    </nav>

    <main class="main-content">
        <slot />
    </main>
</div>

<style>
    .layout-container {
        min-height: 100vh;
        background: linear-gradient(135deg, #e6f3ff 0%, #ffffff 100%);
        margin: 0;
        padding: 0;
    }

    .navbar {
        background: rgba(255, 255, 255, 0.95);
        backdrop-filter: blur(10px);
        padding: 15px 30px;
        display: flex;
        justify-content: space-between;
        align-items: center;
        box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
        border-bottom: 1px solid rgba(0, 123, 255, 0.1);
        position: sticky;
        top: 0;
        z-index: 100;
    }

    .nav-brand h2 {
        margin: 0;
        color: #333;
        font-size: 1.5rem;
        font-weight: 700;
        text-shadow: 0 1px 2px rgba(0, 0, 0, 0.1);
    }

    .nav-links {
        display: flex;
        gap: 15px;
        align-items: center;
    }

    .nav-button {
        background-color: #007bff;
        color: white;
        padding: 12px 20px;
        text-decoration: none;
        border-radius: 8px;
        font-size: 14px;
        font-weight: 600;
        transition: all 0.3s ease;
        border: 2px solid transparent;
        display: inline-block;
    }

    .nav-button:hover {
        background-color: #0056b3;
        transform: translateY(-1px);
        box-shadow: 0 4px 8px rgba(0, 123, 255, 0.3);
    }

    .logout-button {
        background-color: #dc3545;
        border-color: #dc3545;
    }

    .logout-button:hover {
        background-color: #c82333;
        box-shadow: 0 4px 8px rgba(220, 53, 69, 0.3);
    }

    .main-content {
        padding: 0;
        min-height: calc(100vh - 80px);
    }

    /* Mobile responsiveness */
    @media (max-width: 768px) {
        .navbar {
            padding: 12px 20px;
            flex-direction: column;
            gap: 15px;
        }

        .nav-brand h2 {
            font-size: 1.25rem;
        }

        .nav-links {
            width: 100%;
            justify-content: center;
        }

        .nav-button {
            padding: 10px 16px;
            font-size: 13px;
        }

        .main-content {
            min-height: calc(100vh - 120px);
        }
    }

    @media (max-width: 480px) {
        .navbar {
            padding: 10px 15px;
        }

        .nav-links {
            flex-direction: column;
            gap: 10px;
            width: 100%;
        }

        .nav-button {
            width: 100%;
            text-align: center;
            max-width: 200px;
        }

        .main-content {
            min-height: calc(100vh - 140px);
        }
    }
</style>

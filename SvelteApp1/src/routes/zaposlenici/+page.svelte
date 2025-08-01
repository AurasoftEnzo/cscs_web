<script>
    import { onMount } from 'svelte';

    // Data for persons fetched from API
    let persons = [];
    let loading = true;
    let error = null;

    const fetchUsers = async () => {
        try {
            loading = true;
            error = null;
            
            const response = await fetch('http://localhost:5058/api/listUsers');
            
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            
            const data = await response.json();
            persons = data;
        } catch (err) {
            console.error('Failed to fetch users:', err);
            error = err.message;
            persons = [];
        } finally {
            loading = false;
        }
    };

    onMount(() => {
        fetchUsers();
    });
</script>

<div class="page-container">
    <div class="page-header">
        <h1>Zaposlenici</h1>
        <p>Lista svih zaposlenika u sustavu</p>
    </div>

    <div class="table-container">
        {#if loading}
            <div class="loading-state">
                <div class="spinner"></div>
                <p>Učitavanje zaposlenika...</p>
            </div>
        {:else if error}
            <div class="error-state">
                <p>Greška pri učitavanju podataka: {error}</p>
                <button class="retry-button" on:click={fetchUsers}>Pokušaj ponovno</button>
            </div>
        {:else if persons.length === 0}
            <div class="empty-state">
                <p>Nema podataka o zaposlenicima.</p>
            </div>
        {:else}
            <table class="persons-table">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Korisničko ime</th>
                        <th>Puno ime</th>
                    </tr>
                </thead>
                <tbody>
                    {#each persons as person}
                        <tr>
                            <td>{person.id}</td>
                            <td>{person.username}</td>
                            <td>{person.fullName}</td>
                        </tr>
                    {/each}
                </tbody>
            </table>
        {/if}
    </div>
</div>

<style>
    .page-container {
        max-width: 1200px;
        margin: 0 auto;
        padding: 30px 20px;
    }

    .page-header {
        text-align: center;
        margin-bottom: 40px;
    }

    .page-header h1 {
        color: #333;
        font-size: 2.5rem;
        font-weight: 700;
        margin: 0 0 10px 0;
        text-shadow: 0 1px 2px rgba(0, 0, 0, 0.1);
    }

    .page-header p {
        color: #666;
        font-size: 1.1rem;
        margin: 0;
    }

    .table-container {
        background: white;
        border-radius: 12px;
        box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
        overflow: hidden;
        border: 1px solid rgba(0, 123, 255, 0.1);
    }

    .persons-table {
        width: 100%;
        border-collapse: collapse;
        font-size: 16px;
    }

    .persons-table thead {
        background: linear-gradient(135deg, #007bff 0%, #0056b3 100%);
        color: white;
    }

    .persons-table th,
    .persons-table td {
        padding: 16px 20px;
        text-align: left;
        border-bottom: 1px solid #e9ecef;
    }

    .persons-table th {
        font-weight: 600;
        text-transform: uppercase;
        letter-spacing: 0.5px;
        font-size: 14px;
    }

    .persons-table tbody tr {
        transition: all 0.2s ease;
    }

    .persons-table tbody tr:hover {
        background-color: #f8f9fa;
        transform: scale(1.001);
    }

    .persons-table tbody tr:nth-child(even) {
        background-color: #fafbfc;
    }

    .persons-table tbody tr:nth-child(even):hover {
        background-color: #f1f3f4;
    }

    .persons-table td {
        color: #495057;
    }

    .persons-table td:first-child {
        font-weight: 600;
        color: #007bff;
    }

    .empty-state {
        padding: 60px 20px;
        text-align: center;
        color: #6c757d;
        font-size: 1.1rem;
    }

    .loading-state {
        padding: 60px 20px;
        text-align: center;
        color: #6c757d;
        font-size: 1.1rem;
    }

    .loading-state .spinner {
        width: 40px;
        height: 40px;
        border: 4px solid #f3f3f3;
        border-top: 4px solid #007bff;
        border-radius: 50%;
        animation: spin 1s linear infinite;
        margin: 0 auto 20px;
    }

    @keyframes spin {
        0% { transform: rotate(0deg); }
        100% { transform: rotate(360deg); }
    }

    .error-state {
        padding: 60px 20px;
        text-align: center;
        color: #dc3545;
        font-size: 1.1rem;
    }

    .retry-button {
        background-color: #007bff;
        color: white;
        padding: 10px 20px;
        border: none;
        border-radius: 6px;
        font-size: 14px;
        font-weight: 600;
        cursor: pointer;
        margin-top: 15px;
        transition: all 0.3s ease;
    }

    .retry-button:hover {
        background-color: #0056b3;
        transform: translateY(-1px);
        box-shadow: 0 4px 8px rgba(0, 123, 255, 0.3);
    }

    /* Mobile responsiveness */
    @media (max-width: 768px) {
        .page-container {
            padding: 20px 15px;
        }

        .page-header h1 {
            font-size: 2rem;
        }

        .persons-table {
            font-size: 14px;
        }

        .persons-table th,
        .persons-table td {
            padding: 12px 15px;
        }

        .persons-table th {
            font-size: 12px;
        }
    }

    @media (max-width: 480px) {
        .page-header h1 {
            font-size: 1.75rem;
        }

        .page-header p {
            font-size: 1rem;
        }

        .persons-table {
            font-size: 13px;
        }

        .persons-table th,
        .persons-table td {
            padding: 10px 12px;
        }

        /* Stack table on very small screens */
        .persons-table,
        .persons-table thead,
        .persons-table tbody,
        .persons-table th,
        .persons-table td,
        .persons-table tr {
            display: block;
        }

        .persons-table thead tr {
            position: absolute;
            top: -9999px;
            left: -9999px;
        }

        .persons-table tr {
            border: 1px solid #ccc;
            margin-bottom: 10px;
            border-radius: 8px;
            overflow: hidden;
            background: white;
        }

        .persons-table td {
            border: none;
            position: relative;
            padding-left: 50% !important;
            text-align: right;
        }

        .persons-table td:before {
            content: attr(data-label) ": ";
            position: absolute;
            left: 12px;
            width: 45%;
            text-align: left;
            font-weight: 600;
            color: #007bff;
        }

        .persons-table td:nth-child(1):before { content: "ID: "; }
        .persons-table td:nth-child(2):before { content: "Korisničko ime: "; }
        .persons-table td:nth-child(3):before { content: "Puno ime: "; }
    }
</style>

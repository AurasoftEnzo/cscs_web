<script>
  import { onMount } from 'svelte';

  // Reactive state
  let search = $state('');
  let data = $state([]);
  let loading = $state(false);
  let sortColumn = $state('');
  let sortDirection = $state('asc'); // 'asc' or 'desc'

  // Fetch data from CSCS API
  async function fetchData() {
    loading = true;
    try {
      const params = new URLSearchParams({ search });
      const response = await fetch(`http://localhost:5058/api/gk-transactions?${params}`);
      
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      
      const result = await response.json();
      data = result;
      console.log('Fetched data:', data); // Debug log
    } catch (error) {
      console.error('Error fetching data:', error);
      data = []; // Fallback to empty array
    } finally {
      loading = false;
    }
  }

  // Initial load - moved to onMount to avoid SSR issues
  onMount(() => {
    fetchData();
  });

  // Action functions for the table buttons
  function editTransaction(id) {
    console.log('Edit transaction:', id);
    // Add your edit logic here
  }

  function deleteTransaction(id) {
    console.log('Delete transaction:', id);
    // Add your delete logic here
  }

  // Sorting function
  function sortData(column) {
    if (sortColumn === column) {
      // Toggle direction if same column
      sortDirection = sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      // New column, default to ascending
      sortColumn = column;
      sortDirection = 'asc';
    }

    data = [...data].sort((a, b) => {
      let aVal = a[column];
      let bVal = b[column];

      // Handle null/undefined values
      if (aVal == null) aVal = '';
      if (bVal == null) bVal = '';

      // Convert to string for comparison
      aVal = aVal.toString().toLowerCase();
      bVal = bVal.toString().toLowerCase();

      if (sortDirection === 'asc') {
        return aVal < bVal ? -1 : aVal > bVal ? 1 : 0;
      } else {
        return aVal > bVal ? -1 : aVal < bVal ? 1 : 0;
      }
    });
  }

  // Get sort indicator for column headers
  function getSortIndicator(column) {
    if (sortColumn !== column) return '';
    return sortDirection === 'asc' ? ' ↑' : ' ↓';
  }
</script>

<main>
  <h1>GK Transactions</h1>
  
  <div class="controls">
    <input 
      type="text" 
      bind:value={search} 
      placeholder="Search..." 
      oninput={fetchData}
    />
  </div>

  {#if loading}
    <p>Loading...</p>
  {:else}
    <div class="table-container">
      <table class="data-table">
        <thead>
          <tr>
            <th class="frozen sortable" onclick={() => sortData('GK_GL_BR_TEM')}>
              ID{getSortIndicator('GK_GL_BR_TEM')}
            </th>
            <th class="sortable" onclick={() => sortData('GK_GL_DATUM')}>
              Date{getSortIndicator('GK_GL_DATUM')}
            </th>
            <th class="sortable" onclick={() => sortData('GK_GL_OPIS')}>
              Description{getSortIndicator('GK_GL_OPIS')}
            </th>
            <th class="sortable" onclick={() => sortData('GK_GL_PARTNER')}>
              Partner{getSortIndicator('GK_GL_PARTNER')}
            </th>
            <th class="sortable" onclick={() => sortData('GK_GL_POSTED')}>
              Posted{getSortIndicator('GK_GL_POSTED')}
            </th>
            <th class="sortable" onclick={() => sortData('GK_GL_BRDOK')}>
              Doc No.{getSortIndicator('GK_GL_BRDOK')}
            </th>
            <th class="sortable" onclick={() => sortData('GK_GL_DATDOK')}>
              Doc Date{getSortIndicator('GK_GL_DATDOK')}
            </th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {#each data as item}
            <tr>
              <td class="frozen">{item.GK_GL_BR_TEM}</td>
              <td>{item.GK_GL_DATUM}</td>
              <td>{item.GK_GL_OPIS}</td>
              <td>{item.GK_GL_PARTNER}</td>
              <td>{item.GK_GL_POSTED}</td>
              <td>{item.GK_GL_BRDOK}</td>
              <td>{item.GK_GL_DATDOK}</td>
              <td>
                <div class="action-buttons">
                  <button class="btn-edit" onclick={() => editTransaction(item.GK_GL_BR_TEM)}>Edit</button>
                  <button class="btn-delete" onclick={() => deleteTransaction(item.GK_GL_BR_TEM)}>Delete</button>
                </div>
              </td>
            </tr>
          {/each}
        </tbody>
      </table>
    </div>
  {/if}
</main>

<style>
  :global(html, body) {
    height: 100%;
    margin: 0;
    padding: 0;
  }

  .controls { 
    margin: 1rem 0; 
    flex-shrink: 0;
  }
  
  .controls input {
    padding: 0.5rem;
    border: 1px solid #ddd;
    border-radius: 4px;
    width: 300px;
  }
  
  main {
    padding: 1rem;
    height: 100vh;
    display: flex;
    flex-direction: column;
    box-sizing: border-box;
  }
  
  h1 {
    color: #333;
    margin-bottom: 1rem;
    flex-shrink: 0;
  }
  
  .table-container {
    overflow: auto;
    border: 1px solid #ddd;
    border-radius: 4px;
    background: white;
    flex: 1;
    min-height: 0;
  }
  
  .data-table {
    width: 100%;
    border-collapse: collapse;
    font-size: 14px;
  }
  
  .data-table th,
  .data-table td {
    padding: 8px 12px;
    text-align: left;
    border-bottom: 1px solid #eee;
  }
  
  .data-table th {
    background-color: #f8f9fa;
    font-weight: 600;
    border-bottom: 2px solid #dee2e6;
    position: sticky;
    top: 0;
    z-index: 1;
  }
  
  .data-table th.sortable {
    cursor: pointer;
    user-select: none;
    transition: background-color 0.2s;
  }
  
  .data-table th.sortable:hover {
    background-color: #e9ecef;
  }
  
  .data-table th.frozen.sortable:hover {
    background-color: #e9ecef;
  }
  
  .data-table th.frozen,
  .data-table td.frozen {
    background-color: #f8f9fa;
    position: sticky;
    left: 0;
    z-index: 2;
    border-right: 2px solid #dee2e6;
  }
  
  .data-table td.frozen {
    background-color: white;
    font-weight: 600;
  }
  
  .data-table tbody tr:hover {
    background-color: #f8f9fa;
  }
  
  .data-table tbody tr:hover td.frozen {
    background-color: #e9ecef;
  }
  
  .action-buttons {
    display: flex;
    gap: 5px;
  }
  
  .btn-edit, .btn-delete {
    padding: 4px 8px;
    border: none;
    border-radius: 3px;
    cursor: pointer;
    font-size: 12px;
    transition: background-color 0.2s;
  }
  
  .btn-edit {
    background: #007acc;
    color: white;
  }
  
  .btn-delete {
    background: #dc3545;
    color: white;
  }
  
  .btn-edit:hover {
    background: #005a9e;
  }
  
  .btn-delete:hover {
    background: #c82333;
  }
</style>
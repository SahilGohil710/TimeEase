// Optionally control the loader visibility
window.addEventListener('load', function () {
    document.querySelector('.loader').style.display = 'none'; // Hide the loader after the page has loaded
});

// Optionally, show the loader when fetching data
function fetchData() {
    document.querySelector('.loader').style.display = 'block'; // Show loader
    // Simulate data fetching
    setTimeout(() => {
        document.querySelector('.loader').style.display = 'none'; // Hide loader after data fetching
    }, 2000); // Example: fetching takes 2 seconds
}
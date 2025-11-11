// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', function () {
    const sections = {
        '#mainMenu': document.getElementById('mainMenu'),
        '#controlPanel': document.getElementById('controlPanel'),
        '#internshipDiary': document.getElementById('internshipDiary'),
        '#excuse-container': document.getElementById('excuse-container')
    };

    function showSection(sectionId, scrollToElementId = null) {
        // Hide all sections
        for (let id in sections) {
            if (sections[id]) {
                sections[id].style.display = 'none';
            }
        }

        // Show the target section
        if (sections[sectionId]) {
            sections[sectionId].style.display = 'block';
        }

        // Scroll to the specific element if provided, otherwise to the section
        if (scrollToElementId && document.getElementById(scrollToElementId)) {
            document.getElementById(scrollToElementId).scrollIntoView({ behavior: 'smooth' });
        } else if (sections[sectionId]) {
            sections[sectionId].scrollIntoView({ behavior: 'smooth' });
        }
    }

    // Handle initial load based on hash
    const initialHash = window.location.hash;
    if (initialHash) {
        if (initialHash === '#calendarContainer') {
            showSection('#controlPanel', 'calendarContainer');
        } else {
            showSection(initialHash);
        }
    } else {
        // Default to showing the main menu if no hash
        showSection('#mainMenu');
    }

    // Add click listeners to navigation links
    document.querySelectorAll('.navbar-nav .nav-link, .user-menu .dropdown-item').forEach(link => {
        link.addEventListener('click', function (event) {
            const href = this.getAttribute('href');
            if (href && href.startsWith('/Student/StudentPage')) {
                event.preventDefault();
                const hash = href.split('#')[1];
                if (hash) {
                    window.location.hash = hash;
                    if (hash === 'calendarContainer') {
                        showSection('#controlPanel', 'calendarContainer');
                    } else {
                        showSection('#' + hash);
                    }
                } else {
                    // If no hash, navigate to the base StudentPage (mainMenu)
                    window.location.hash = '';
                    showSection('#mainMenu');
                }
            }
        });
    });

    // Listen for hash changes (e.g., back/forward button)
    window.addEventListener('hashchange', function () {
        const hash = window.location.hash;
        if (hash) {
            if (hash === '#calendarContainer') {
                showSection('#controlPanel', 'calendarContainer');
            } else {
                showSection(hash);
            }
        } else {
            showSection('#mainMenu');
        }
    });
});

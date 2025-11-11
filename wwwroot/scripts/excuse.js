        document.addEventListener('DOMContentLoaded', function() {
            const startDate = document.getElementById('LeaveStart');
            const endDate = document.getElementById('LeaveEnd');
            
            startDate.addEventListener('change', function() {
                if (endDate.value && new Date(startDate.value) > new Date(endDate.value)) {
                    endDate.value = startDate.value;
                }
            });
            
            endDate.addEventListener('change', function() {
                if (startDate.value && new Date(endDate.value) < new Date(startDate.value)) {
                    startDate.value = endDate.value;
                }
            });
        });
        // Mazeret menüsü toggle fonksiyonu
function toggleExcuseMenu(leaveId) {
    const dropdown = document.getElementById(`excuse-dropdown-${leaveId}`);
    const allDropdowns = document.querySelectorAll('.diary-history-dropdown');
    
    // Diğer tüm dropdown'ları kapat
    allDropdowns.forEach(item => {
        if (item !== dropdown) {
            item.classList.remove('show');
        }
    });
    
    // Mevcut dropdown'ı aç/kapat
    dropdown.classList.toggle('show');
}

// Mazeret silme fonksiyonu
function deleteExcuse(leaveId) {
    if (confirm('Bu mazereti silmek istediğinize emin misiniz?')) {
        const form = document.createElement('form');
        form.method = 'post';
        form.action = '/Student/DeleteLeave';
        
        const input = document.createElement('input');
        input.type = 'hidden';
        input.name = 'LeaveID';
        input.value = leaveId;
        
        form.appendChild(input);
        document.body.appendChild(form);
        form.submit();
    }
}

// Formu göster/gizle fonksiyonu
function showExcuseForm() {
    const formContainer = document.querySelector('.excuse-form-container');
    formContainer.scrollIntoView({ behavior: 'smooth' });
}

// Sayfa yüklendiğinde mazeret dropdown'larını kapat
document.addEventListener('click', function(e) {
    if (!e.target.closest('.diary-history-card-menu')) {
        document.querySelectorAll('.diary-history-dropdown').forEach(dropdown => {
            dropdown.classList.remove('show');
        });
    }
});
// Mazeret menüsü toggle fonksiyonu
function toggleExcuseMenu(leaveId) {
    const dropdown = document.getElementById(`excuse-dropdown-${leaveId}`);
    const allDropdowns = document.querySelectorAll('.diary-history-dropdown');
    const overlay = document.getElementById('dropdownOverlay');
    
    // Diğer tüm dropdown'ları kapat
    allDropdowns.forEach(item => {
        if (item !== dropdown) {
            item.classList.remove('show');
        }
    });
    
    // Overlay'i aç/kapat
    if (overlay) {
        overlay.classList.toggle('show');
    }
    
    // Mevcut dropdown'ı aç/kapat
    dropdown.classList.toggle('show');
}

// Mazeret silme fonksiyonu
function deleteExcuse(leaveId) {
    if (confirm('Bu mazereti silmek istediğinize emin misiniz?')) {
        const form = document.createElement('form');
        form.method = 'post';
        form.action = '/Student/DeleteLeave';
        
        const input = document.createElement('input');
        input.type = 'hidden';
        input.name = 'LeaveID';
        input.value = leaveId;
        
        form.appendChild(input);
        document.body.appendChild(form);
        form.submit();
    }
}

// Formu göster/gizle fonksiyonu
function showExcuseForm() {
    const formContainer = document.querySelector('.excuse-form-container');
    formContainer.scrollIntoView({ behavior: 'smooth' });
}

// Dropdown dışına tıklama ile kapatma
document.addEventListener('click', function(e) {
    if (!e.target.closest('.diary-history-card-menu') && !e.target.closest('.diary-history-dropdown')) {
        document.querySelectorAll('.diary-history-dropdown').forEach(dropdown => {
            dropdown.classList.remove('show');
        });
        const overlay = document.getElementById('dropdownOverlay');
        if (overlay) {
            overlay.classList.remove('show');
        }
    }
});

// ESC tuşu ile kapatma
document.addEventListener('keydown', function(e) {
    if (e.key === 'Escape') {
        document.querySelectorAll('.diary-history-dropdown').forEach(dropdown => {
            dropdown.classList.remove('show');
        });
        const overlay = document.getElementById('dropdownOverlay');
        if (overlay) {
            overlay.classList.remove('show');
        }
    }
});
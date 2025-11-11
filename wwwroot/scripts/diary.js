// Staj defteri sekme işlevselliği
function initializeDiaryTabs() {
    const tabs = document.querySelectorAll('.diary-tab');
    const tabContents = document.querySelectorAll('.diary-tab-content');

    tabs.forEach(tab => {
        tab.addEventListener('click', function () {
            // Tüm sekmeleri ve içerikleri devre dışı bırak
            document.querySelectorAll('.diary-tab').forEach(t => t.classList.remove('active'));
            document.querySelectorAll('.diary-tab-content').forEach(c => c.classList.remove('active'));
            document.querySelectorAll('.diary-tabs-right').forEach(r => r.style.display = 'none');
            
            // Tıklanan sekme ve içeriği etkinleştir
            this.classList.add('active');
            const tabId = this.getAttribute('data-tab');
            document.getElementById(`${tabId}-tab`).classList.add('active');
            
            // İlgili sağ içeriği göster
            document.getElementById(`${tabId}-right-content`).style.display = 'flex';
        });
    });

    // Sayfa yüklendiğinde notlar sekmesini aktif et
    document.addEventListener('DOMContentLoaded', function() {
        const notesRightContent = document.getElementById('notes-right-content');
        if (notesRightContent) {
            notesRightContent.style.display = 'flex';
        }
    });
}

// Modal açma fonksiyonları
function openAddNoteModal() {
    var addNoteModal = new bootstrap.Modal(document.getElementById('addNoteModal'));
    addNoteModal.show();
}

function openAddVideoModal() {
    var addVideoModal = new bootstrap.Modal(document.getElementById('addVideoModal'));
    addVideoModal.show();
}

function openWatchVideoModal(filePath, title) {
    const videoPlayer = document.getElementById('videoPlayer');
    const modalTitle = document.getElementById('watchVideoModalLabel');
    if (videoPlayer) {
        videoPlayer.src = filePath;
        videoPlayer.load();
    }
    if (modalTitle && title) {
        modalTitle.textContent = title;
    }
    var modal = new bootstrap.Modal(document.getElementById('watchVideoModal'));
    modal.show();
}

// Form doğrulama fonksiyonları
function validateNoteForm() {
    const title = document.getElementById('Title').value.trim();
    const content = document.getElementById('Content').value.trim();
    
    if (!title || !content) {
        alert('Lütfen başlık ve içerik alanlarını doldurun.');
        return false;
    }
    
    return true;
}

function validateVideoForm() {
    const title = document.getElementById('VideoTitle').value.trim();
    const file = document.getElementById('VideoFile').files[0];
    
    if (!title) {
        alert('Lütfen video başlığını girin.');
        return false;
    }
    
    if (!file) {
        alert('Lütfen bir video dosyası seçin.');
        return false;
    }
    
    // Dosya boyutu kontrolü (100MB)
    if (file.size > 100 * 1024 * 1024) {
        alert('Dosya boyutu 100MB\'dan büyük olamaz.');
        return false;
    }
    
    return true;
}

// Dosya adını güncelleme
function updateFileName(input) {
    const fileNameDisplay = document.getElementById('fileNameDisplay');
    if (input.files.length > 0) {
        const file = input.files[0];
        const fileSizeMB = (file.size / (1024 * 1024)).toFixed(2);
        fileNameDisplay.textContent = `Seçilen dosya: ${file.name} (${fileSizeMB} MB)`;
    } else {
        fileNameDisplay.textContent = 'Henüz dosya seçilmedi.';
    }
}

// Not dropdown menü fonksiyonları
function toggleNoteMenu(noteId) {
    closeAllMenus();
    const dropdown = document.getElementById(`note-dropdown-${noteId}`);
    const overlay = document.getElementById('dropdownOverlay');
    if (dropdown && overlay) {
        dropdown.style.display = 'block';
        overlay.style.display = 'block';
    }
}

function toggleExcuseMenu(leaveId) {
    // Tüm açık menüleri kapat
    const allDropdowns = document.querySelectorAll('.diary-history-dropdown');
    allDropdowns.forEach(dropdown => {
        if (dropdown.id !== `excuse-dropdown-${leaveId}`) {
            dropdown.classList.remove('show');
        }
    });

    // Tıklanan menüyü aç/kapat
    const dropdown = document.getElementById(`excuse-dropdown-${leaveId}`);
    if (dropdown) {
        dropdown.classList.toggle('show');
    }
}

function closeAllMenus() {
    const dropdowns = document.querySelectorAll('.diary-history-dropdown');
    const overlay = document.getElementById('dropdownOverlay');
    dropdowns.forEach(dropdown => dropdown.style.display = 'none');
    if (overlay) overlay.style.display = 'none';
}

// Video dropdown menü fonksiyonları
function toggleVideoMenu(videoId) {
    closeAllVideoMenus();
    const dropdown = document.getElementById(`video-dropdown-${videoId}`);
    const overlay = document.getElementById('videoDropdownOverlay');
    if (dropdown && overlay) {
        dropdown.style.display = 'block';
        overlay.style.display = 'block';
    }
}

function closeAllVideoMenus() {
    const dropdowns = document.querySelectorAll('.diary-history-dropdown');
    const overlay = document.getElementById('videoDropdownOverlay');
    dropdowns.forEach(dropdown => dropdown.style.display = 'none');
    if (overlay) overlay.style.display = 'none';
}

function confirmDeleteNote(noteId) {
    if (confirm('Bu notu silmek istediğinizden emin misiniz?')) {
        deleteNote(noteId);
    }
}

function deleteNote(noteId) {
    // GET isteği ile silme işlemi (mevcut controller'a uygun)
    fetch(`/Student/DeleteNote/${noteId}`, {
        method: 'GET'
    })
    .then(response => {
        if (response.ok) {
            // Başarılı silme
            const noteCard = document.getElementById(`note-card-${noteId}`);
            if (noteCard) {
                noteCard.remove();
                updateNotesCount();
                showNotification('Not başarıyla silindi.', 'success');
                
                // Sayfayı yenile (GET isteği sonrası)
                setTimeout(() => {
                    window.location.reload();
                }, 1000);
            }
        } else {
            throw new Error('Silme işlemi başarısız');
        }
    })
    .catch(error => {
        console.error('Not silinirken hata oluştu:', error);
        showNotification('Not silinirken bir hata oluştu.', 'error');
    });
}

function deleteExcuse(leaveId) {
    if (confirm('Bu mazereti silmek istediğinizden emin misiniz?')) {
        fetch(`/Student/DeleteExcuse/${leaveId}`, {
            method: 'GET'
        })
        .then(response => {
            if (response.ok) {
                // Başarılı silme
                const excuseCard = document.getElementById(`excuse-card-${leaveId}`);
                if (excuseCard) {
                    excuseCard.remove();
                    updateExcusesCount();
                    showNotification('Mazeret başarıyla silindi.', 'success');
                    
                    // Sayfayı yenile (GET isteği sonrası)
                    setTimeout(() => {
                        window.location.reload();
                    }, 1000);
                }
            } else {
                throw new Error('Silme işlemi başarısız');
            }
        })
        .catch(error => {
            console.error('Mazeret silinirken hata oluştu:', error);
            showNotification('Mazeret silinirken bir hata oluştu.', 'error');
        });
    }
}

function confirmDeleteVideo(videoId) {
    if (confirm('Bu videoyu silmek istediğinizden emin misiniz?')) {
        deleteVideo(videoId);
    }
}

function deleteVideo(videoId) {
    fetch(`/Student/DeleteVideo/${videoId}`, {
        method: 'GET'
    })
    .then(response => {
        if (response.ok) {
            const videoCard = document.getElementById(`video-card-${videoId}`);
            if (videoCard) {
                videoCard.remove();
                updateVideosCount();
                showNotification('Video başarıyla silindi.', 'success');
                setTimeout(() => window.location.reload(), 1000);
            }
        } else {
            throw new Error('Silme işlemi başarısız');
        }
    })
    .catch(error => {
        showNotification('Video silinirken bir hata oluştu.', 'error');
    });
}

// Not sayısını güncelleme
function updateNotesCount() {
    const notesCount = document.querySelectorAll('.diary-history-card[id^="note-card-"]').length;
    const notesCountElement = document.querySelector('.diary-notes-count');
    if (notesCountElement) {
        notesCountElement.textContent = `${notesCount} Not`;
    }
}

// Video sayısını güncelleme
function updateVideosCount() {
    const videosCount = document.querySelectorAll('.diary-history-card[id^="video-card-"]').length;
    const videosCountElement = document.querySelector('.diary-videos-count');
    if (videosCountElement) {
        // Toplam boyutu da güncelleyebilirsiniz
        videosCountElement.textContent = `${videosCount} Video • 50 MB`;
    }
}

// Mazeret sayısını güncelleme
function updateExcusesCount() {
    const excusesCount = document.querySelectorAll('.diary-history-card.excuse-card').length;
    const excusesCountElement = document.querySelector('.excuse-notes-count');
    if (excusesCountElement) {
        excusesCountElement.textContent = `${excusesCount} Mazeret`;
    }
    
    // İstatistikleri de güncelle
    const excuseStats = document.querySelector('.excuse-stats .excuse-stat');
    if (excuseStats) {
        excuseStats.innerHTML = `<strong>${excusesCount}</strong> toplam mazeret`;
    }
}

// Sayfa yüklendiğinde event listener'ları ekle
document.addEventListener('DOMContentLoaded', function() {
    // Dropdown dışına tıklama için
    document.addEventListener('click', function(event) {
        if (!event.target.closest('.diary-history-card-menu')) {
            const dropdowns = document.querySelectorAll('.diary-history-dropdown');
            dropdowns.forEach(dropdown => dropdown.classList.remove('show'));
        }
    });

    // ESC tuşu ile menüleri kapatma
    document.addEventListener('keydown', function(event) {
        if (event.key === 'Escape') {
            const dropdowns = document.querySelectorAll('.diary-history-dropdown');
            dropdowns.forEach(dropdown => dropdown.classList.remove('show'));
        }
    });
    
    // Form submit işlemleri için
    initializeFormHandlers();

    var addVideoBtn = document.getElementById('addVideoBtn');
    if (!addVideoBtn) return;

    var today = new Date();
    var day = today.getDay(); // 0: Pazar, 6: Cumartesi

    if (day !== 0 && day !== 6) {
        addVideoBtn.disabled = true;
        addVideoBtn.style.opacity = "0.5";
        addVideoBtn.style.pointerEvents = "none";
        addVideoBtn.style.cursor = "not-allowed";
    } else {
        addVideoBtn.disabled = false;
        addVideoBtn.style.opacity = "";
        addVideoBtn.style.pointerEvents = "";
        addVideoBtn.style.cursor = "";
    }
});

// Form işleyicilerini başlat
function initializeFormHandlers() {
    // Not ekleme formu
    const noteForm = document.querySelector('form[action="/Student/AddNote"]');
    if (noteForm) {
        noteForm.addEventListener('submit', function(e) {
            if (!validateNoteForm()) {
                e.preventDefault();
            }
        });
    }
    
    // Video yükleme formu
    const videoForm = document.querySelector('form[action="/Student/UploadVideo"]');
    if (videoForm) {
        videoForm.addEventListener('submit', function(e) {
            if (!validateVideoForm()) {
                e.preventDefault();
            }
        });
    }
}

// Video yükleme alanı sürükle-bırak işlevselliği
function initializeVideoUploadArea() {
    const videoUploadArea = document.querySelector('.video-upload-area');
    if (videoUploadArea) {
        const videoFileInput = document.getElementById('VideoFile');

        // Sürükleme olayları
        videoUploadArea.addEventListener('dragover', function (e) {
            e.preventDefault();
            this.style.borderColor = '#37be71';
            this.style.background = '#f0fff4';
        });

        videoUploadArea.addEventListener('dragleave', function (e) {
            e.preventDefault();
            this.style.borderColor = '#ddd';
            this.style.background = '#f8f9fa';
        });

        videoUploadArea.addEventListener('drop', function (e) {
            e.preventDefault();
            this.style.borderColor = '#ddd';
            this.style.background = '#f8f9fa';

            if (e.dataTransfer.files.length) {
                videoFileInput.files = e.dataTransfer.files;
                updateFileName(videoFileInput);
            }
        });
    }
}

// Sayfa yüklendiğinde video yükleme alanını başlat
document.addEventListener('DOMContentLoaded', initializeVideoUploadArea);

// Global fonksiyonlar - HTML'den erişilebilir olması için
window.openAddNoteModal = openAddNoteModal;
window.openAddVideoModal = openAddVideoModal;
window.openWatchVideoModal = openWatchVideoModal;
window.toggleNoteMenu = toggleNoteMenu;
window.toggleVideoMenu = toggleVideoMenu;
window.toggleExcuseMenu = toggleExcuseMenu;
window.deleteExcuse = deleteExcuse;
window.confirmDeleteNote = confirmDeleteNote;
window.confirmDeleteVideo = confirmDeleteVideo;
window.openEditNoteModal = openEditNoteModal;
window.openEditVideoModal = openEditVideoModal;
window.viewNote = viewNote;
window.updateFileName = updateFileName;
window.validateNoteForm = validateNoteForm;
window.validateVideoForm = validateVideoForm;
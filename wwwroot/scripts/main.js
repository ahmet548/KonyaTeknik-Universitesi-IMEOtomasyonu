// Ana menü ve navigasyon işlevselliği
document.addEventListener('DOMContentLoaded', function () {
    initializeNavigation();
    initializeScrollToTop();
    initializeSidebar();
    initializeDropdowns();
    initializeVideoUpload();
});

// Navigasyon işlevselliği - GÜNCELLENMİŞ
function initializeNavigation() {
    // Tüm içerikleri gizle, sadece ana menüyü göster
    if (document.getElementById('controlPanel')) document.getElementById('controlPanel').style.display = 'none';
    if (document.getElementById('internshipDiary')) document.getElementById('internshipDiary').style.display = 'none';
    if (document.getElementById('mainMenu')) document.getElementById('mainMenu').style.display = 'block';
    if (document.getElementById('excuse-container')) document.getElementById('excuse-container').style.display = 'none';

    // Menü linklerine tıklama olayları
    document.querySelectorAll('.main-nav-link').forEach(link => {
        link.addEventListener('click', function (e) {
            e.preventDefault();

            // Tüm içerikleri gizle
            document.querySelectorAll('#controlPanel, #internshipDiary, #mainMenu, #excuse-container').forEach(el => {
                if (el) el.style.display = 'none';
            });

            // Aktif menü işaretle
            document.querySelectorAll('.main-nav-link').forEach(item => item.classList.remove('active'));
            this.classList.add('active');

            // Tıklanan menüye göre ilgili içeriği göster
            const linkText = this.textContent.trim();
            let contentId = 'mainMenu';

            switch(linkText) {
                case 'Kontrol Paneli':
                    contentId = 'controlPanel';
                    break;
                case 'Staj Defteri':
                    contentId = 'internshipDiary';
                    break;
                case 'Mazeret Bildir':
                    contentId = 'excuse-container';
                    break;
                default:
                    contentId = 'mainMenu';
            }

            const contentElement = document.getElementById(contentId);
            if (contentElement) {
                contentElement.style.display = 'block';

                // Kontrol paneli ise takvimi başlat
                if (contentId === 'controlPanel') {
                    setTimeout(() => {
                        if (typeof initializeCalendar === 'function') {
                            initializeCalendar();
                        }
                    }, 100);
                }

                // Staj defteri ise tab işlevselliğini başlat
                if (contentId === 'internshipDiary') {
                    setTimeout(() => {
                        if (typeof initializeDiaryTabs === 'function') {
                            initializeDiaryTabs();
                        }
                    }, 100);
                }

                // Mazeret sayfası ise mazeret işlevselliğini başlat
                if (contentId === 'excuse-container') {
                    setTimeout(() => {
                        if (typeof initializeExcusePage === 'function') {
                            initializeExcusePage();
                        }
                    }, 100);
                }
            }

            // Sayfayı en üste kaydır
            window.scrollTo({
                top: 0,
                behavior: 'smooth'
            });
        });
    });
}

// Yukarı çık butonu işlevselliği
function initializeScrollToTop() {
    const scrollToTopButton = document.getElementById('scrollToTop');

    if (scrollToTopButton) {
        window.addEventListener('scroll', function () {
            if (window.pageYOffset > 300) {
                scrollToTopButton.classList.add('show');
            } else {
                scrollToTopButton.classList.remove('show');
            }
        });

        scrollToTopButton.addEventListener('click', function () {
            window.scrollTo({
                top: 0,
                behavior: 'smooth'
            });
        });
    }
}

// Yan menü işlevselliği
function initializeSidebar() {
    const menuToggle = document.getElementById('menuToggle');
    const closeSidebar = document.getElementById('closeSidebar');
    const sidebar = document.getElementById('sidebar');
    const body = document.body;

    if (menuToggle && sidebar) {
        menuToggle.addEventListener('click', function () {
            sidebar.classList.add('open');
            body.classList.add('sidebar-open');
        });

        if (closeSidebar) {
            closeSidebar.addEventListener('click', function () {
                sidebar.classList.remove('open');
                body.classList.remove('sidebar-open');
            });
        }

        document.addEventListener('keydown', function (event) {
            if (event.key === 'Escape' && sidebar.classList.contains('open')) {
                sidebar.classList.remove('open');
                body.classList.remove('sidebar-open');
            }
        });
    }
}

// Dropdown menü animasyonları
function initializeDropdowns() {
    document.querySelectorAll('.dropdown').forEach(dropdown => {
        dropdown.addEventListener('show.bs.dropdown', function (e) {
            const menu = dropdown.querySelector('.dropdown-menu');
            if (menu) {
                menu.style.maxHeight = '0px';
                setTimeout(() => {
                    menu.style.maxHeight = menu.scrollHeight + 'px';
                }, 10);
            }
        });

        dropdown.addEventListener('hide.bs.dropdown', function (e) {
            const menu = dropdown.querySelector('.dropdown-menu');
            if (menu) {
                menu.style.maxHeight = menu.scrollHeight + 'px';
                setTimeout(() => {
                    menu.style.maxHeight = '0px';
                }, 10);
            }
        });
    });
}

// Video yükleme alanı işlevselliği
function initializeVideoUpload() {
    const videoUploadArea = document.querySelector('.video-upload-area');
    if (videoUploadArea) {
        const videoFileInput = document.getElementById('VideoFile');

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

// Mazeret sayfası işlevselliği
function initializeExcusePage() {
    // Tarih validasyonu
    const startDate = document.getElementById('LeaveStart');
    const endDate = document.getElementById('LeaveEnd');
    
    if (startDate && endDate) {
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
    }

    // Form gönderim işlemi
    const excuseForm = document.querySelector('form[action="/Student/SubmitExcuse"]');
    if (excuseForm) {
        excuseForm.addEventListener('submit', function(e) {
            // Form validasyonu burada yapılabilir
            const startDate = document.getElementById('LeaveStart').value;
            const endDate = document.getElementById('LeaveEnd').value;
            
            if (!startDate || !endDate) {
                e.preventDefault();
                alert('Lütfen tarih aralığını seçin.');
                return;
            }

            const selectedReason = document.querySelector('input[name="LeaveReason"]:checked');
            if (!selectedReason) {
                e.preventDefault();
                alert('Lütfen mazeret türünü seçin.');
                return;
            }

            // Diğer validasyonlar buraya eklenebilir
        });
    }

    // Sil butonları için onay dialogu
    document.querySelectorAll('form[action="/Student/DeleteLeave"]').forEach(form => {
        form.addEventListener('submit', function(e) {
            if (!confirm('Bu mazereti silmek istediğinizden emin misiniz?')) {
                e.preventDefault();
            }
        });
    });
}

// Formların yüklenmesi
function loadForms() {
    fetch("/data/forms.json")
        .then(res => res.json())
        .then(forms => {
            const list = document.getElementById("formList");
            if (!list) return;

            list.innerHTML = "";
            forms.forEach((form, index) => {
                const li = document.createElement("li");

                const a = document.createElement("a");
                a.href = form.url;
                a.target = form.target;
                a.className = "text-decoration-none d-flex align-items-center justify-content-center gap-2";

                const iconWrapper = document.createElement("span");
                iconWrapper.innerHTML = form.icon;

                const span = document.createElement("span");
                span.className = "menu-text";
                span.textContent = form.title;

                a.appendChild(iconWrapper);
                a.appendChild(span);
                li.appendChild(a);
                list.appendChild(li);

                if (index < forms.length - 1) {
                    const divider = document.createElement("div");
                    divider.className = "dropdown-divider";
                    list.appendChild(divider);
                }
            });
        })
        .catch(error => {
            console.error('Formlar yüklenirken hata oluştu:', error);
        });
}

// Sayfa yüklendiğinde formları yükle
document.addEventListener("DOMContentLoaded", loadForms);

// Dosya adı güncelleme fonksiyonu (video yükleme için)
function updateFileName(input) {
    const fileNameDisplay = document.getElementById('fileNameDisplay');
    if (fileNameDisplay && input.files.length > 0) {
        fileNameDisplay.textContent = `Seçilen dosya: ${input.files[0].name}`;
        fileNameDisplay.className = 'form-text mt-2 text-success';
    }
}

// Video form validasyonu
function validateVideoForm() {
    const videoFile = document.getElementById('VideoFile');
    const videoTitle = document.getElementById('VideoTitle');
    
    if (!videoTitle.value.trim()) {
        alert('Lütfen video başlığını girin.');
        return false;
    }
    
    if (!videoFile.files.length) {
        alert('Lütfen bir video dosyası seçin.');
        return false;
    }
    
    const file = videoFile.files[0];
    const maxSize = 100 * 1024 * 1024; // 100MB
    
    if (file.size > maxSize) {
        alert('Video boyutu 100MB\'tan küçük olmalıdır.');
        return false;
    }
    
    if (!file.type.startsWith('video/')) {
        alert('Lütfen sadece video dosyaları yükleyin.');
        return false;
    }
    
    return true;
}

// Form önizleme işlevselliği
function initializeFormPreview() {
    const formList = document.getElementById('formList');
    if (!formList) return;

    formList.addEventListener('click', function (e) {
        const link = e.target.closest('a');
        if (!link) return;

        const pdfUrl = link.getAttribute('href');
        if (pdfUrl && pdfUrl.endsWith('.pdf')) {
            e.preventDefault();
            showPdfPreview(pdfUrl);
        }
    });
}

function showPdfPreview(pdfUrl) {
    const previewContainer = document.getElementById('pdfPreviewContainer');
    const previewFrame = document.getElementById('pdfPreviewFrame');
    const downloadBtn = document.getElementById('downloadPdfBtn');

    if (previewContainer && previewFrame && downloadBtn) {
        previewFrame.src = pdfUrl;
        previewContainer.style.display = 'block';
        downloadBtn.style.display = 'block';
        downloadBtn.onclick = function () {
            window.open(pdfUrl, '_blank');
        };
    }
}
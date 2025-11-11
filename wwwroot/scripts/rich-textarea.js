// Quill Editor ve İstatistik Sayaçları
let quill;
let isContentInitialized = false; // İçerik yüklenme kontrolü

document.addEventListener('DOMContentLoaded', function () {
    console.log('DOM loaded - Initializing Quill editor...');

    // Quill modüllerini tanımla
    const modules = {
        toolbar: {
            container: '#toolbar-container'
        },
        history: {
            delay: 1000,
            maxStack: 100,
            userOnly: true
        }
    };

    // Quill editor'ü başlat
    quill = new Quill('#editor', {
        theme: 'snow',
        modules: modules,
        placeholder: 'Not içeriğinizi buraya yazın...'
    });

    // Sayfa yüklendiğinde mevcut içeriği kontrol et ve yükle
    initializeEditorContent();

    // Editor değişikliklerini takip et
    quill.on('text-change', function () {
        updateContent();
        updateStats();
    });

    // İlk istatistik güncellemesi
    setTimeout(updateStats, 300);

    // Klavye kısayolları
    document.addEventListener('keydown', function (e) {
        if ((e.ctrlKey || e.metaKey) && e.key === 's') {
            e.preventDefault();
            if (validateNoteForm()) {
                document.getElementById('noteForm').submit();
            }
        }
    });
});

// Editor içeriğini başlat - DÜZELTİLMİŞ VERSİYON
function initializeEditorContent() {
    if (isContentInitialized) {
        console.log('Content already initialized, skipping...');
        return;
    }

    const contentTextarea = document.getElementById('Content');
    const existingContent = contentTextarea ? contentTextarea.value.trim() : '';

    console.log('Mevcut içerik:', existingContent);

    if (existingContent && existingContent !== '') {
        // İçeriği Quill editor'e yükle (sadece bir kez)
        const delta = quill.clipboard.convert(existingContent);
        quill.setContents(delta, 'silent');
        
        // Hidden textarea'yı da güncelle
        updateContent();
        updateStats();
        
        isContentInitialized = true;
        
        console.log('Not içeriği başarıyla yüklendi!');
    } else {
        // Yeni not modunda
        quill.setText('');
        updateContent();
        updateStats();
        isContentInitialized = true;
    }
}

// İstatistikleri güncelle
function updateStats() {
    try {
        const text = quill.getText();
        const htmlContent = quill.root.innerHTML;

        // Karakter sayısı (boşluklar dahil)
        const charCount = text.length;

        // Kelime sayısı
        const words = text.trim().split(/\s+/);
        const wordCount = text.trim() === '' ? 0 : words.length;

        // Paragraf sayısını hesapla
        const tempDiv = document.createElement('div');
        tempDiv.innerHTML = htmlContent;
        const paragraphs = tempDiv.querySelectorAll('p');
        let paragraphCount = 0;

        paragraphs.forEach(p => {
            if (p.textContent.trim().length > 0 || p.innerHTML.includes('<img') || p.innerHTML.includes('<table')) {
                paragraphCount++;
            }
        });

        // Eğer hiç paragraf yoksa ama içerik varsa, 1 paragraf say
        if (paragraphCount === 0 && text.trim().length > 0) {
            paragraphCount = 1;
        }

        console.log('Stats:', { charCount, wordCount, paragraphCount });

        // Elementleri güncelle
        const charCountElement = document.getElementById('charCount');
        const wordCountElement = document.getElementById('wordCount');
        const paragraphCountElement = document.getElementById('paragraphCount');

        if (charCountElement) charCountElement.textContent = `${charCount - 1} karakter`;
        if (wordCountElement) wordCountElement.textContent = `${wordCount} kelime`;
        if (paragraphCountElement) paragraphCountElement.textContent = `${paragraphCount} paragraf`;

    } catch (error) {
        console.error('updateStats error:', error);
    }
}

// İçeriği güncelle
function updateContent() {
    const content = quill.root.innerHTML;
    const contentTextarea = document.getElementById('Content');
    if (contentTextarea) {
        contentTextarea.value = content;
    }
}

// Form doğrulama
function validateNoteForm() {
    const title = document.getElementById('Title').value.trim();
    const content = quill.getText().trim();

    if (!title) {
        showAlert('Lütfen bir başlık girin.', 'error');
        document.getElementById('Title').focus();
        return false;
    }

    if (!content || content === '') {
        showAlert('Lütfen not içeriği girin.', 'error');
        quill.focus();
        return false;
    }

    // Hidden textarea'yı güncelle
    updateContent();

    showAlert('Not başarıyla kaydediliyor...', 'success');
    return true;
}

// Sayfa kapanmadan önce kaydedilmemiş değişiklikleri kontrol et
window.addEventListener('beforeunload', function (e) {
    const title = document.getElementById('Title').value.trim();
    const content = quill.getText().trim();

    if ((title && title !== '') || (content && content !== '')) {
        e.preventDefault();
        e.returnValue = 'Kaydedilmemiş değişiklikler var. Sayfadan ayrılmak istediğinizden emin misiniz?';
        return e.returnValue;
    }
});

// Auto-save fonksiyonu (opsiyonel)
function autoSave() {
    const title = document.getElementById('Title').value.trim();
    const content = quill.getText().trim();

    if (title || content) {
        console.log('Auto-saving...');
        updateContent();
        // Burada localStorage'a kaydedebilirsiniz
        localStorage.setItem('draftNote', JSON.stringify({
            title: title,
            content: document.getElementById('Content').value,
            timestamp: new Date().toISOString()
        }));
    }
}

// 30 saniyede bir otomatik kaydet
setInterval(autoSave, 30000);
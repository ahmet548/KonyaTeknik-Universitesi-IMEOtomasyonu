// Takvimi başlatma fonksiyonu
function initializeCalendar() {
    console.log("Takvim başlatılıyor...");
    
    // Elementleri güvenli şekilde al
    const getElement = (id) => document.getElementById(id);
    
    const calendarDiv = getElement("calendar");
    const prevMonthBtn = getElement("prevMonth");
    const nextMonthBtn = getElement("nextMonth");
    
    // Elementlerin var olup olmadığını kontrol et
    if (!calendarDiv || !prevMonthBtn || !nextMonthBtn) {
        console.error("Takvim elementleri bulunamadı:", {
            calendar: !!calendarDiv,
            prevBtn: !!prevMonthBtn,
            nextBtn: !!nextMonthBtn
        });
        return false;
    }

    // Veri elementlerini güvenli şekilde al
    const getData = (elementId, defaultValue) => {
        const element = getElement(elementId);
        if (!element) {
            console.warn(`${elementId} elementi bulunamadı, varsayılan değer kullanılıyor.`);
            return defaultValue;
        }
        try {
            return JSON.parse(element.textContent || JSON.stringify(defaultValue));
        } catch (error) {
            console.error(`${elementId} verisi parse edilemedi:`, error);
            return defaultValue;
        }
    };

    // Verileri al
    const userNotes = getData("userNotesData", {});
    const videoUploadDays = getData("videoUploadDaysData", []);
    const videoTitles = getData("videoTitlesData", {});
    const excuseDays = getData("excuseDaysData", []);

    console.log("Takvim verileri yüklendi");

    let currentYear = new Date().getFullYear();
    let currentMonth = new Date().getMonth();

    function generateCalendar(year, month) {
        if (!calendarDiv) return;
        
        calendarDiv.innerHTML = "";

        const today = new Date();
        const firstDayOfMonth = new Date(year, month, 1).getDay();
        const daysInMonth = new Date(year, month + 1, 0).getDate();

        const monthNames = [
            "Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran",
            "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık"
        ];

        const calendarHeader = getElement("calendarHeader");
        if (calendarHeader) {
            calendarHeader.textContent = `${monthNames[month]} ${year}`;
        }

        // Gün isimleri
        const daysOfWeek = ["Paz", "Pzt", "Sal", "Çar", "Per", "Cum", "Cmt"];
        const daysRow = document.createElement("div");
        daysRow.style.display = "grid";
        daysRow.style.gridTemplateColumns = "repeat(7, 1fr)";
        daysRow.style.gap = "1px";
        daysRow.style.marginBottom = "1px";

        daysOfWeek.forEach((day, i) => {
            const dayDiv = document.createElement("div");
            dayDiv.style.padding = "30px 8px";
            dayDiv.style.textAlign = "center";
            dayDiv.style.fontWeight = "500";
            dayDiv.style.fontSize = "18px";
            dayDiv.style.background = "#edf1f6";
            dayDiv.textContent = day;
            daysRow.appendChild(dayDiv);
        });
        calendarDiv.appendChild(daysRow);

        // Günler grid
        const daysGrid = document.createElement("div");
        daysGrid.style.display = "grid";
        daysGrid.style.gridTemplateColumns = "repeat(7, 1fr)";
        daysGrid.style.gap = "1px";
        daysGrid.style.background = "#fff";
        daysGrid.style.borderRadius = "8px";
        daysGrid.style.overflow = "hidden";

        // Boş hücreler
        const emptyDays = firstDayOfMonth === 0 ? 6 : firstDayOfMonth - 1;
        for (let i = 0; i < emptyDays; i++) {
            const emptyDiv = document.createElement("div");
            emptyDiv.style.height = "120px";
            emptyDiv.style.background = "#fff";
            daysGrid.appendChild(emptyDiv);
        }

        // Gün hücreleri
        for (let day = 1; day <= daysInMonth; day++) {
            const date = `${year}-${String(month + 1).padStart(2, "0")}-${String(day).padStart(2, "0")}`;
            const dayDiv = document.createElement("div");
            dayDiv.className = "calendar-day";
            dayDiv.style.height = "120px";
            dayDiv.style.background = "white";
            dayDiv.style.padding = "8px";
            dayDiv.style.display = "flex";
            dayDiv.style.flexDirection = "column";
            dayDiv.style.cursor = "pointer";
            dayDiv.style.transition = "all 0.3s ease";
            
            const isToday = day === today.getDate() && month === today.getMonth() && year === today.getFullYear();
            const hasNote = userNotes[date]?.title;
            const hasVideo = videoUploadDays.includes(date) && videoTitles[date];
            const isExcuseDay = excuseDays.includes(date);

            if (hasNote) {
                dayDiv.style.borderLeft = "3px solid #28a745";
            }
            if (hasVideo) {
                dayDiv.style.borderRight = "3px solid #007bff";
            }

            dayDiv.innerHTML = `
                <div class="day-number" style="font-weight: 600; font-size: 14px; margin-bottom: 4px; ${
                    isToday ? 'background: #dc3545; color: white; width: 24px; height: 24px; border-radius: 50%; display: flex; align-items: center; justify-content: center;' : ''
                }">${day}</div>
                <div style="flex: 1; display: flex; flex-direction: column; gap: 2px;">
                    ${hasNote ? `
                        <div style="display: flex; align-items: center; gap: 4px; font-size: 11px; padding: 2px 4px; border-radius: 3px; background: rgba(40, 167, 69, 0.1); color: #28a745; white-space: nowrap; overflow: hidden; text-overflow: ellipsis;">
                            <svg width="12" height="12" viewBox="0 0 16 16">
                                <path d="M14 4.5V14a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V2a2 2 0 0 1 2-2h5.5zm-3 0A1.5 1.5 0 0 1 9.5 3V1H4a1 1 0 0 0-1 1v12a1 1 0 0 0 1 1h8a1 1 0 0 0 1-1V4.5z"/>
                            </svg>
                            <span>${userNotes[date]?.title || ''}</span>
                        </div>
                    ` : ''}
                    ${hasVideo ? `
                        <div style="display: flex; align-items: center; gap: 4px; font-size: 11px; padding: 2px 4px; border-radius: 3px; background: rgba(0, 123, 255, 0.1); color: #007bff; white-space: nowrap; overflow: hidden; text-overflow: ellipsis;">
                            <svg width="12" height="12" viewBox="0 0 16 16">
                                <path fill-rule="evenodd" d="M0 5a2 2 0 0 1 2-2h7.5a2 2 0 0 1 1.983 1.738l3.11-1.382A1 1 0 0 1 16 4.269v7.462a1 1 0 0 1-1.406.913l-3.111-1.382A2 2 0 0 1 9.5 13H2a2 2 0 0 1-2-2zm11.5 5.175 3.5 1.556V4.269l-3.5 1.556zM2 4a1 1 0 0 0-1 1v6a1 1 0 0 0 1 1h7.5a1 1 0 0 0 1-1V5a1 1 0 0 0-1-1z"/>
                            </svg>
                            <span>${videoTitles[date] || ''}</span>
                        </div>
                    ` : ''}
                    ${isExcuseDay ? `
                        <div style="display: flex; align-items: center; gap: 4px; font-size: 11px; padding: 2px 4px; border-radius: 3px; background: rgba(255, 193, 7, 0.2); color: #856404; font-weight: 600;">
                            <span>Mazeretli</span>
                        </div>
                    ` : ''}
                </div>
            `;

            daysGrid.appendChild(dayDiv);
        }

        calendarDiv.appendChild(daysGrid);
    }

    // Buton event listener'ları
    prevMonthBtn.addEventListener("click", () => {
        currentMonth--;
        if (currentMonth < 0) {
            currentMonth = 11;
            currentYear--;
        }
        generateCalendar(currentYear, currentMonth);
    });

    nextMonthBtn.addEventListener("click", () => {
        currentMonth++;
        if (currentMonth > 11) {
            currentMonth = 0;
            currentYear++;
        }
        generateCalendar(currentYear, currentMonth);
    });

    // İlk yükleme
    generateCalendar(currentYear, currentMonth);
    return true;
}

// Global fonksiyon olarak tanımla
window.initializeCalendar = initializeCalendar;
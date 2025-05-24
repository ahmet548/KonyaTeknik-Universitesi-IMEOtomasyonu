const userNotes = JSON.parse(document.getElementById("userNotesData").textContent);
const videoUploadDays = JSON.parse(document.getElementById("videoUploadDaysData").textContent);
const videoTitles = JSON.parse(document.getElementById("videoTitlesData").textContent);
const excuseDays = JSON.parse(document.getElementById("excuseDaysData").textContent);

let currentYear = new Date().getFullYear();
let currentMonth = new Date().getMonth();

function generateCalendar(year, month) {
    const calendarDiv = document.getElementById("calendar");
    const calendarHeader = document.getElementById("calendarHeader");
    calendarDiv.innerHTML = "";

    const today = new Date();
    const firstDayOfMonth = new Date(year, month, 1).getDay();
    const daysInMonth = new Date(year, month + 1, 0).getDate();

    const monthNames = [
        "Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran",
        "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık"
    ];

    calendarHeader.textContent = `${monthNames[month]} ${year}`;

    const daysOfWeek = ["Pzt", "Sal", "Çar", "Per", "Cum", "Cmt", "Paz"];
    const daysRow = document.createElement("div");
    daysRow.style.display = "grid";
    daysRow.style.gridTemplateColumns = "repeat(7, 1fr)";

    daysOfWeek.forEach((day, i) => {
        const dayDiv = document.createElement("div");
        dayDiv.style.fontWeight = "bold";
        dayDiv.style.padding = "0 10px";
        dayDiv.style.fontSize = "14px";
        dayDiv.style.borderRight = "1px dashed #b3e5fc";

        if (i === 0) {
            dayDiv.style.borderLeft = "1px dashed #b3e5fc";
        }

        dayDiv.style.borderTop = "1px dashed #b3e5fc";
        dayDiv.textContent = day;
        daysRow.appendChild(dayDiv);
    });
    calendarDiv.appendChild(daysRow);

    const daysGrid = document.createElement("div");
    daysGrid.style.display = "grid";
    daysGrid.style.gridTemplateColumns = "repeat(7, 1fr)";
    daysGrid.style.gap = "0px"; // grid gap sıfır

    for (let i = 0; i < (firstDayOfMonth === 0 ? 6 : firstDayOfMonth - 1); i++) {
        const emptyDiv = document.createElement("div");
        emptyDiv.style.backgroundColor = "white";
        emptyDiv.style.height = "140px";
        emptyDiv.style.borderRight = "1px dashed #b3e5fc";
        emptyDiv.style.borderBottom = "1px dashed #b3e5fc";
        emptyDiv.style.borderTop = "1px dashed #b3e5fc";

        if (i === 0) {
            emptyDiv.style.borderLeft = "1px dashed #b3e5fc";
        }

        daysGrid.appendChild(emptyDiv);
    }

    for (let day = 1; day <= daysInMonth; day++) {
        const date = `${year}-${String(month + 1).padStart(2, "0")}-${String(day).padStart(2, "0")}`;
        const dayDiv = document.createElement("div");
        dayDiv.style.backgroundColor = "white";
        dayDiv.style.height = "140px";
        dayDiv.style.width = "98.5px";
        dayDiv.style.display = "flex";
        dayDiv.style.flexDirection = "column";
        dayDiv.style.padding = "10px";
        dayDiv.style.cursor = "pointer";
        dayDiv.style.fontSize = "14px";
        dayDiv.style.borderRight = "1px dashed #b3e5fc";
        dayDiv.style.borderBottom = "1px dashed #b3e5fc";

        const gridIndex = day + (firstDayOfMonth === 0 ? 6 : firstDayOfMonth - 1) - 1;
        if (gridIndex % 7 === 5 || gridIndex % 7 === 6) {
            dayDiv.style.backgroundColor = "#f8f9fa";
            dayDiv.style.color = "#222";
            dayDiv.style.fontWeight = "bold";
        }

        if (gridIndex < 7) {
            dayDiv.style.borderTop = "1px dashed #b3e5fc";
        }
        if (gridIndex % 7 === 0) {
            dayDiv.style.borderLeft = "1px dashed #b3e5fc";
        }

        dayDiv.onmouseenter = () => {
            dayDiv.style.backgroundColor = "#f0f0f0";
        };
        dayDiv.onmouseleave = () => {
            if (excuseDays.includes(date)) {
                dayDiv.style.backgroundColor = "#fff3cd";
            } else if (gridIndex % 7 === 5 || gridIndex % 7 === 6) {
                dayDiv.style.backgroundColor = "#f8f9fa";
            } else {
                dayDiv.style.backgroundColor = "white";
            }
        };

        const dayNumber = document.createElement("div");
        dayNumber.textContent = day;
        dayNumber.style.fontWeight = "bold";
        dayNumber.style.marginBottom = "4px";

        if (
            day === today.getDate() &&
            month === today.getMonth() &&
            year === today.getFullYear()
        ) {
            dayNumber.style.background = "#b52525";
            dayNumber.style.color = "#fff";
            dayNumber.style.borderRadius = "50%";
            dayNumber.style.width = "32px";
            dayNumber.style.height = "32px";
            dayNumber.style.display = "flex";
            dayNumber.style.alignItems = "center";
            dayNumber.style.justifyContent = "center";
        }

        dayDiv.appendChild(dayNumber);

        if (userNotes[date]?.title) {
            const noteTitle = document.createElement("div");
            noteTitle.innerHTML = `
            <span style="margin-right:4px;">
                <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" fill="currentColor" class="bi bi-file-earmark" viewBox="0 0 16 16">
                    <path d="M14 4.5V14a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V2a2 2 0 0 1 2-2h5.5zm-3 0A1.5 1.5 0 0 1 9.5 3V1H4a1 1 0 0 0-1 1v12a1 1 0 0 0 1 1h8a1 1 0 0 0 1-1V4.5z"/>
                </svg>
            </span>
            ${userNotes[date].title}`;
            noteTitle.style.fontSize = "13px";
            noteTitle.style.marginBottom = "2px";
            noteTitle.style.whiteSpace = "nowrap";
            noteTitle.style.overflow = "hidden";
            noteTitle.style.textOverflow = "ellipsis";
            noteTitle.style.width = "100%";
            noteTitle.style.display = "block";
            dayDiv.appendChild(noteTitle);
        }

        if (videoUploadDays.includes(date) && videoTitles[date]) {
            const videoTitle = document.createElement("div");
            videoTitle.innerHTML = `
            <span style="margin-right:4px;">
                <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" fill="currentColor" class="bi bi-camera-video" viewBox="0 0 16 16">
                    <path fill-rule="evenodd" d="M0 5a2 2 0 0 1 2-2h7.5a2 2 0 0 1 1.983 1.738l3.11-1.382A1 1 0 0 1 16 4.269v7.462a1 1 0 0 1-1.406.913l-3.111-1.382A2 2 0 0 1 9.5 13H2a2 2 0 0 1-2-2zm11.5 5.175 3.5 1.556V4.269l-3.5 1.556zM2 4a1 1 0 0 0-1 1v6a1 1 0 0 0 1 1h7.5a1 1 0 0 0 1-1V5a1 1 0 0 0-1-1z"/>
                </svg>
            </span>
            ${videoTitles[date]}`;
            videoTitle.style.fontSize = "12px";
            videoTitle.style.whiteSpace = "nowrap";
            videoTitle.style.overflow = "hidden";
            videoTitle.style.textOverflow = "ellipsis";
            videoTitle.style.width = "100%";
            videoTitle.style.display = "block";
            dayDiv.appendChild(videoTitle);
        } 

        // Mazaretli günleri uyarı rengine boya ve "Mazaretli" yaz
        if (excuseDays.includes(date)) {
            dayDiv.style.backgroundColor = "#fff3cd"; // Bootstrap alert-warning
            const excuseLabel = document.createElement("div");       
            dayDiv.appendChild(excuseLabel);
        }

        daysGrid.appendChild(dayDiv);
    }

    calendarDiv.appendChild(daysGrid);
}

document.getElementById("prevMonth").addEventListener("click", () => {
    currentMonth--;
    if (currentMonth < 0) {
        currentMonth = 11;
        currentYear--;
    }
    generateCalendar(currentYear, currentMonth);
});

document.getElementById("nextMonth").addEventListener("click", () => {
    currentMonth++;
    if (currentMonth > 11) {
        currentMonth = 0;
        currentYear++;
    }
    generateCalendar(currentYear, currentMonth);
});

generateCalendar(currentYear, currentMonth);


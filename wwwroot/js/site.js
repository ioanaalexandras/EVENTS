function initUploadModal() {
    const uploadModal = document.getElementById('uploadModal');
    if (uploadModal) {
        uploadModal.addEventListener('show.bs.modal', event => {
            const button = event.relatedTarget;
            const taskId = button.getAttribute('data-task-id');
            const taskName = button.getAttribute('data-task-name');

            uploadModal.querySelector('.modal-title').textContent = `Încarcă poză pentru: ${taskName}`;
            uploadModal.querySelector('#uploadTaskId').value = taskId;
        });
    }
}

function submitDeletePhoto(photoId) {
    const form = document.getElementById('deletePhotoForm');
    const input = document.getElementById('deletePhotoId');
    
    if (form && input) {
        input.value = photoId;
        form.submit();
    }
}

document.addEventListener('DOMContentLoaded', () => {
    initUploadModal();
});

let calendar;

document.addEventListener('DOMContentLoaded', function () {
    console.log("Evenimente primite:", calendarEventsFromServer);
    const calendarEl = document.getElementById('calendar');
    if (!calendarEl || typeof calendarEventsFromServer === 'undefined') return; // Nu face nimic dacă nu e pe pagina Calendar
    

    calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth',
        firstDay: 1,
        showNonCurrentDates: false,
        height: 'auto',
        headerToolbar: {
            left: 'today',
            center: 'title',
            right: 'prev,next'
        },
        events: calendarEventsFromServer, // variabilă globală definită în Razor
        eventClick: function (info) {
            const eventId = info.event.id;
            window.location.href = `/Checklist?eventId=${info.event.id}`;
        },

        eventColor: '#ffb6c1',
        eventTextColor: '#000000',
        eventDisplay: 'block'
    });

    calendar.render();

    const btnSearch = document.getElementById("btnSearch");
    if (btnSearch) {
        btnSearch.addEventListener("click", searchEvent);
    }

});

function searchEvent() {
    const query = document.getElementById("searchInput").value.trim().toLowerCase();
    if (!query || !calendar) return;

    const events = calendar.getEvents();

    const match = events.find(ev => ev.title.toLowerCase().includes(query));

    if (match) {
        calendar.gotoDate(match.start);
        document.getElementById("searchInput").value = "";
        match.setProp("backgroundColor", "#ff99cc");  // evidențiere temporară
        match.setProp("borderColor", "#cc3366");

        setTimeout(() => {
            match.setProp("backgroundColor", "");
            match.setProp("borderColor", "");
        }, 2000);
    } else {
        alert("Evenimentul nu a fost găsit.");
    }
}





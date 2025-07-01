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

document.addEventListener('DOMContentLoaded', function () {
    var calendarEl = document.getElementById('calendar');
    if (!calendarEl || typeof calendarEventsFromServer === 'undefined') return; // Nu face nimic dacă nu e pe pagina Calendar

    var calendar = new FullCalendar.Calendar(calendarEl, {
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
        eventColor: '#ffb6c1',
        eventTextColor: '#000000',
        eventDisplay: 'block'
    });

    calendar.render();
});



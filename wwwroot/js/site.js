// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// =========================
// 🔹 Section: Modale Bootstrap
// =========================
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


// =========================
// 🔹 Section: Efecte UI (ex: toasturi, scroll, tooltips)
// =========================
// function initTooltips() { ... }
// function initToasts() { ... }


// =========================
// 🔹 Section: Inițializare globală
// =========================
document.addEventListener('DOMContentLoaded', function () {
    initUploadModal();
    // initTooltips();
    // initToasts();
});


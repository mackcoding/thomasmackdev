function openModal(modalId) {
    document.getElementById(modalId).showModal();
}

function preventDefaultScroll(targetId) {
    document.getElementById(targetId).scrollIntoView({ behavior: 'instant', block: 'nearest' });
}

document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('[data-modal]').forEach(element => {
        element.addEventListener('click', function () {
            openModal(this.dataset.modal);
        });
    });

    document.querySelectorAll('[data-slide]').forEach(element => {
        element.addEventListener('click', function (e) {
            e.preventDefault();
            preventDefaultScroll(this.dataset.slide);
        });
    });
});

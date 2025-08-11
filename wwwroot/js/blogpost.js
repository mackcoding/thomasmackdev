document.addEventListener('DOMContentLoaded', function () {
    const copyButtons = document.querySelectorAll('.copy-btn');
    copyButtons.forEach(button => {
        button.addEventListener('click', function () {
            const target = this.getAttribute('data-target');
            const container = target ? document.querySelector(target) : null;
            let code = this.getAttribute('data-code') || '';
            if (container) {
                code = container.innerText.replace(/\u00A0/g, ' ');
            } else if (code) {
                code = code.replace(/&#10;/g, '\n');
            }
            copyToClipboard(this, code, container);
        });
    });
});

function copyToClipboard(button, codeSelector) {
    const codeElement = document.querySelector(codeSelector);
    if (!codeElement) {
        console.error('Code element not found:', codeSelector);
        return;
    }

    const text = codeElement.textContent || codeElement.innerText;

    navigator.clipboard.writeText(text).then(function () {
        const originalContent = button.innerHTML;
        button.innerHTML = '<span class="icon-[iconoir--check] text-sm"></span>';
        button.classList.add('btn-success');
        setTimeout(function () {
            button.innerHTML = originalContent;
            button.classList.remove('btn-success');
        }, 2000);
    }).catch(function (err) {
        console.error('Failed to copy text: ', err);
    });
}

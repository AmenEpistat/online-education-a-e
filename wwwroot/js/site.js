// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener('DOMContentLoaded', () => {
    const loginBtn = document.getElementById('login-btn');
    const registerBtn = document.getElementById('register-btn');

    loginBtn.addEventListener('click', async () => {
        const username = document.getElementById('login-email').value;
        const password = document.getElementById('login-password').value;

        const response = await fetch('/Account/Login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: new URLSearchParams({ username, password })
        });

        if (response.ok) {
            window.location.href = '/Dashboard';
        } else {
            alert(await response.text());
        }
    });

    registerBtn.addEventListener('click', async () => {
        const username = document.getElementById('register-name').value;
        const email = document.getElementById('register-email').value;
        const password = document.getElementById('register-password').value;

        const response = await fetch('/Account/Register', {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: new URLSearchParams({ username, email, password })
        });

        if (response.ok) {
            window.location.href = '/Dashboard';
        } else {
            alert(await response.text());
        }
    });
});

// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

window.setJwt = (token) => {
    localStorage.setItem("JwtToken", token);
}

window.getJwt = (token) => {
    var token = localStorage.getItem("JwtToken");
    return token;
}
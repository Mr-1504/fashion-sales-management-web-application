$(document).ready(function () {
    $("#change").on("click", function (e) {
        e.preventDefault();
        $.ajax({
            url: '/Khachhang/UpdatePassword',
            type: 'GET',
            success: function (data) {
                $("#change-password").html(data);
            },
            error: function () {
                alert("Có lỗi xảy ra khi tải nội dung!");
            }
        });
    });

    $("#suathongtin").on("click", function (e) {
        e.preventDefault();
        $.ajax({
            url: '/KhachHang/Suathongtinajax',
            type: 'GET',
            success: function (data) {
                $("#change-password").html(data);
            },
            error: function () {
                alert("Có lỗi xảy ra khi tải nội dung!");
            }
        });
    });

    $("#donhang").on("click", function (e) {
        e.preventDefault();
        $.ajax({
            url: '/KhachHang/DonHang',
            type: 'GET',
            success: function (data) {
                $("#change-password").html(data);
            },
            error: function () {
                alert("Có lỗi xảy ra khi tải nội dung!");
            } 
        });
    });
});

document.addEventListener("DOMContentLoaded", function () {
    // Get all navigation links
    const navLinks = document.querySelectorAll(".list-group-item");

    // Get the current page path
    const currentPath = window.location.pathname;

    navLinks.forEach(link => {
        // Check if the link's href matches the current path
        const linkPath = link.getAttribute("href");

        if (linkPath && currentPath.includes(linkPath)) {
            // Remove 'active' class from other links
            navLinks.forEach(l => l.classList.remove("active"));

            // Add 'active' class to the current link
            link.classList.add("active");
        }

        // Add a click event to navigate and update the active class
        link.addEventListener("click", function () {
            navLinks.forEach(l => l.classList.remove("active"));
            this.classList.add("active");
        });
    });
});
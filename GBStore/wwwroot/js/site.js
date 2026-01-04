// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.




document.addEventListener("DOMContentLoaded", function() {
    const stars = document.querySelectorAll("#starRating .star");
    const ratingInput = document.getElementById("ratingValue");
    let selectedRating = 0;

    stars.forEach(star => {
        star.addEventListener("mouseover", function() {
            const val = parseInt(this.getAttribute("data-value"));
            highlightStars(val);
        });

        star.addEventListener("mouseout", function() {
            highlightStars(selectedRating);
        });

        star.addEventListener("click", function() {
            selectedRating = parseInt(this.getAttribute("data-value"));
            ratingInput.value = selectedRating;
            highlightStars(selectedRating);
        });
    });

    function highlightStars(rating) {
        stars.forEach(star => {
            const val = parseInt(star.getAttribute("data-value"));
            if (val <= rating) {
                star.classList.add("selected");
            } else {
                star.classList.remove("selected");
            }
        });
    }
});

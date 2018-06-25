/// <reference path="/Resource/Javascript/Jquery/jquery-2.1.4.js" />
/// <reference path="/Resource/Javascript/ProviderBase/ProviderBase.Ajax.js" />
/// <reference path="/Resource/Javascript/ProviderBase/ProviderBase.js" />
$(function () {
    OCromwellHotel.LoadCarousel();
    OCromwellHotel.LoadSlides();
});

(function (OCromwellHotel, $, undefined) {
    var carouselIndex = 0;
    var nextPageTimeout = null;
    var slideIndex = new Array();
    
    OCromwellHotel.LoadCarousel = function () {
        // Setup images and page buttons
        var slides = document.getElementsByClassName("js-body-carousel-item-image");

        if (slides != null && slides.length > 0) {
            var slidePage = document.getElementById("js-body-carousel-item-pagelist");

            if (slidePage != null) {
                for (var i = 0; i < slides.length; i++) {
                    slidePage.innerHTML += `<span class='body-carousel-item-page js-body-carousel-item-page' onclick='OCromwellHotel.CurrentCarousel(${i})'></span>`;
                }
            }

            OCromwellHotel.ShowCarousel();
        }
    }

    OCromwellHotel.PlusCarousel = function (page) {
        // Next/previous controls
        OCromwellHotel.ShowCarousel(carouselIndex += page);
    }

    OCromwellHotel.CurrentCarousel = function (page) {
        // Thumbnail image controls
        OCromwellHotel.ShowCarousel(carouselIndex = page);
    }

    OCromwellHotel.ShowCarousel = function () {
        // Show image and set timer to fade to next one
        if (nextPageTimeout != null) {
            clearTimeout(nextPageTimeout);
        }

        var slides = document.getElementsByClassName("js-body-carousel-item-image");

        var dots = document.getElementsByClassName("js-body-carousel-item-page");

        for (var i = 0; i < slides.length; i++) {
            slides[i].style.display = "none";
        }

        carouselIndex++;

        if (carouselIndex > slides.length) {
            carouselIndex = 1
        }

        for (var i = 0; i < dots.length; i++) {
            dots[i].className = dots[i].className.replace(" active", "");
        }

        slides[carouselIndex - 1].style.display = "block";
        dots[carouselIndex - 1].className += " active";

        nextPageTimeout = setTimeout(OCromwellHotel.ShowCarousel, 20000); // Change image every 20 seconds
    };

    OCromwellHotel.LoadSlides = function () {
        var slides = document.getElementsByClassName("js-body-slideshow-item");

        if (slides != null && slides.length > 0) {
            for (var i = 0; i < slides.length; i++){
                var slideImages = $(slides[i]).find(".js-body-slideshow-item-image");
                var slideNavigation = $(slides[i]).find(".js-body-slideshow-item-pagelist");

                if (slideImages != null && slideImages.length > 1) {
                    if (slideNavigation != null && slideNavigation.length > 0) {
                        slideNavigation[0].innerHTML += `<a class="prev" onclick='OCromwellHotel.PlusSlide(this, -1, ${i});'>&#10094;</a>`;
                        slideNavigation[0].innerHTML += `<a class="next" onclick='OCromwellHotel.PlusSlide(this, 1, ${i});'>&#10095;</a>`;
                    }

                    for (var j = 0; j < slideImages.length; j++) {
                        var slidePage = $(slideImages[j]).find("span");

                        if (slidePage != null && slidePage.length > 0) {
                            slidePage[0].innerHTML = `${j + 1} / ${slideImages.length}`;
                        }
                    }

                    slideImages[0].style.display = "block";

                    slideIndex[i] = 0;

                    OCromwellHotel.ShowSlide(slides[i], i);
                }
            }
        }
    };

    OCromwellHotel.PlusSlide = function (element, page, index) {
        if (index == undefined) {
            index = 0;
        }

        var container = $(element).parents(".js-body-slideshow-item");

        slideIndex[index] += page;

        if (slideIndex[index] < 0) {
            var slideImages = container.find(".js-body-slideshow-item-image");

            if (slideImages != null && slideImages.length > 0) {
                slideIndex[index] = slideImages.length - 1;
            }
        }

        // Next/previous controls
        OCromwellHotel.ShowSlide(container, index);
    };

    OCromwellHotel.CurrentSlide = function (element, page, index) {
        if (index == undefined) {
            index = 0;
        }

        var container = $(element).parents(".js-body-slideshow-item");

        slideIndex[index] = page;

        // Thumbnail image controls
        OCromwellHotel.ShowSlide(container, index);
    };

    OCromwellHotel.ShowSlide = function (container, index) {
        if (index == undefined) {
            index = 0;
        }

        var slideImages = $(container).find(".js-body-slideshow-item-image");

        if (slideImages != null && slideImages.length > 0) {
            var pageShown = false;

            for (var i = 0; i < slideImages.length; i++) {
                slideImages[i].style.display = "none";

                if (i == slideIndex[index]) {
                    slideImages[i].style.display = "block";
                    pageShown = true;
                }
            }

            if (pageShown == false) {
                slideIndex[index] = 0;
                slideImages[0].style.display = "block";
            }
        }
    };

    OCromwellHotel.MenuMouseEnter = function (parent, elementName) {
        var parentWidth = $(parent).outerWidth();
        var parentOffset = $(parent).offset();

        $(`#MenuHover-${elementName}`).css({
            width: parentWidth + "px",
            top: (parentOffset.top - 5) + "px",
            left: parentOffset.left + "px"
        }).show();
    }

    OCromwellHotel.MenuHoverMouseOut = function (elementName) {
        $(`#MenuHover-${elementName}`).hide();
    }

    OCromwellHotel.MenuHoverMouseEnter = function (elementName) {
        $(`#MenuHover-${elementName}`).show();
    }
}(window.OCromwellHotel = window.OCromwellHotel || {}, jQuery));

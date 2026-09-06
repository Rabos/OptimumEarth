(function () {
  "use strict";

  var reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;

  // Mobile navigation toggle
  var toggle = document.querySelector("[data-nav-toggle]");
  var nav = document.querySelector("[data-primary-nav]");

  if (toggle && nav) {
    toggle.addEventListener("click", function () {
      var isOpen = nav.getAttribute("data-open") === "true";
      nav.setAttribute("data-open", String(!isOpen));
      toggle.setAttribute("aria-expanded", String(!isOpen));
      document.body.style.overflow = !isOpen ? "hidden" : "";
    });

    nav.querySelectorAll("a").forEach(function (link) {
      link.addEventListener("click", function () {
        nav.setAttribute("data-open", "false");
        toggle.setAttribute("aria-expanded", "false");
        document.body.style.overflow = "";
      });
    });
  }

  // Home hero carousel: auto-advance every 6s, pause on hover/focus,
  // real buttons for indicators, and no autoplay under reduced motion.
  var carousel = document.querySelector("[data-hero-carousel]");
  if (carousel) {
    var slides = carousel.querySelectorAll("[data-hero-slide]");
    var copies = carousel.querySelectorAll("[data-hero-copy]");
    var dots = carousel.querySelectorAll("[data-hero-dot]");
    var current = 0;
    var timer = null;

    var show = function (index) {
      current = index;
      slides.forEach(function (slide, i) { slide.classList.toggle("is-active", i === index); });
      copies.forEach(function (copy, i) { copy.hidden = i !== index; });
      dots.forEach(function (dot, i) {
        dot.classList.toggle("is-active", i === index);
        dot.setAttribute("aria-pressed", i === index ? "true" : "false");
      });
    };

    var next = function () { show((current + 1) % slides.length); };

    var start = function () {
      if (reduceMotion || timer || slides.length < 2) return;
      timer = window.setInterval(next, 6000);
    };

    var stop = function () {
      if (timer) { window.clearInterval(timer); timer = null; }
    };

    dots.forEach(function (dot, i) {
      dot.addEventListener("click", function () {
        show(i);
        stop();
        start();
      });
    });

    carousel.addEventListener("mouseenter", stop);
    carousel.addEventListener("mouseleave", start);
    carousel.addEventListener("focusin", stop);
    carousel.addEventListener("focusout", function (e) {
      if (!carousel.contains(e.relatedTarget)) start();
    });

    start();
  }
})();

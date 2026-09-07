(function () {
  "use strict";

  var reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;

  // Mobile navigation: full-page overlay, opened by the header hamburger
  // and closed via its own close button, a link tap, or Escape.
  var toggle = document.querySelector("[data-nav-toggle]");
  var navClose = document.querySelector("[data-nav-close]");
  var nav = document.querySelector("[data-primary-nav]");

  if (toggle && nav) {
    var setNavOpen = function (open) {
      nav.setAttribute("data-open", String(open));
      toggle.setAttribute("aria-expanded", String(open));
      document.body.setAttribute("data-nav-open", String(open));
      document.body.style.overflow = open ? "hidden" : "";
    };

    toggle.addEventListener("click", function () { setNavOpen(true); });

    if (navClose) {
      navClose.addEventListener("click", function () { setNavOpen(false); });
    }

    nav.querySelectorAll("a").forEach(function (link) {
      link.addEventListener("click", function () { setNavOpen(false); });
    });

    document.addEventListener("keydown", function (e) {
      if (e.key === "Escape" && nav.getAttribute("data-open") === "true") setNavOpen(false);
    });
  }

  // Home hero carousel: auto-advance every 6s, pause on hover/focus,
  // real buttons for indicators, and no autoplay under reduced motion.
  var carousel = document.querySelector("[data-hero-carousel]");
  if (carousel) {
    var slides = carousel.querySelectorAll("[data-hero-slide]");
    var copies = carousel.querySelectorAll("[data-hero-copy]");
    var prevBtn = carousel.querySelector("[data-hero-prev]");
    var nextBtn = carousel.querySelector("[data-hero-next]");
    var current = 0;
    var timer = null;

    var show = function (index) {
      current = index;
      slides.forEach(function (slide, i) { slide.classList.toggle("is-active", i === index); });
      copies.forEach(function (copy, i) { copy.hidden = i !== index; });
    };

    var next = function () { show((current + 1) % slides.length); };
    var prev = function () { show((current - 1 + slides.length) % slides.length); };

    var start = function () {
      if (reduceMotion || timer || slides.length < 2) return;
      timer = window.setInterval(next, 6000);
    };

    var stop = function () {
      if (timer) { window.clearInterval(timer); timer = null; }
    };

    if (prevBtn) {
      prevBtn.addEventListener("click", function () {
        prev();
        stop();
        start();
      });
    }

    if (nextBtn) {
      nextBtn.addEventListener("click", function () {
        next();
        stop();
        start();
      });
    }

    carousel.addEventListener("mouseenter", stop);
    carousel.addEventListener("mouseleave", start);
    carousel.addEventListener("focusin", stop);
    carousel.addEventListener("focusout", function (e) {
      if (!carousel.contains(e.relatedTarget)) start();
    });

    start();
  }

  // Back to top: appears once the page has scrolled past one viewport.
  var backToTop = document.querySelector("[data-back-to-top]");
  if (backToTop) {
    var toggleBackToTop = function () {
      backToTop.classList.toggle("is-visible", window.scrollY > window.innerHeight * 0.6);
    };

    backToTop.addEventListener("click", function () {
      window.scrollTo({ top: 0, behavior: reduceMotion ? "auto" : "smooth" });
    });

    window.addEventListener("scroll", toggleBackToTop, { passive: true });
    toggleBackToTop();
  }
})();

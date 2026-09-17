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

  // Foundation page sub-nav: highlights the section currently in view as
  // the visitor scrolls (scroll-spy), on top of the browser's native
  // smooth-scroll-to-anchor on click.
  var subNav = document.querySelector("[data-sub-nav]");
  if (subNav) {
    var subNavLinks = subNav.querySelectorAll("[data-sub-nav-link]");
    var subNavSections = Array.prototype.map.call(subNavLinks, function (link) {
      return document.getElementById(link.getAttribute("data-sub-nav-link"));
    });

    var setActiveSection = function (id) {
      subNavLinks.forEach(function (link) {
        link.classList.toggle("is-active", link.getAttribute("data-sub-nav-link") === id);
      });
    };

    if (window.IntersectionObserver) {
      var visible = new Map();
      var observer = new IntersectionObserver(function (entries) {
        entries.forEach(function (entry) {
          visible.set(entry.target.id, entry.isIntersecting);
        });
        var current = subNavSections.find(function (section) { return section && visible.get(section.id); });
        if (current) setActiveSection(current.id);
      }, { rootMargin: "-45% 0px -50% 0px" });

      subNavSections.forEach(function (section) {
        if (section) observer.observe(section);
      });
    }
  }

  // Foundation "Stories & Impact" tiles: each opens a native <dialog> holding
  // that story's gallery. <dialog> gives focus trapping and Esc for free; this
  // adds the close button, the backdrop click, and moving between images.
  //
  // A tile whose key has no gallery renders no dialog, so it stays inert rather
  // than opening an empty frame.
  var storyTriggers = document.querySelectorAll("[data-story-trigger]");
  storyTriggers.forEach(function (trigger) {
    var dialog = document.getElementById("story-" + trigger.getAttribute("data-story-trigger"));
    if (!dialog) return;

    var slides = Array.prototype.slice.call(dialog.querySelectorAll("[data-lightbox-slide]"));
    var thumbs = Array.prototype.slice.call(dialog.querySelectorAll("[data-lightbox-thumb]"));
    var caption = dialog.querySelector("[data-lightbox-caption]");
    var position = dialog.querySelector("[data-lightbox-position]");
    var current = 0;

    var show = function (next) {
      if (!slides.length) return;
      // wraps both ways, so the arrows never dead-end on the first or last image
      current = (next + slides.length) % slides.length;

      slides.forEach(function (slide, i) {
        slide.classList.toggle("is-current", i === current);
      });
      thumbs.forEach(function (thumb, i) {
        thumb.classList.toggle("is-current", i === current);
        thumb.setAttribute("aria-current", i === current ? "true" : "false");
      });

      var img = slides[current].querySelector("img");
      if (caption && img) caption.textContent = img.getAttribute("alt") || "";
      if (position) position.textContent = String(current + 1);
    };

    trigger.addEventListener("click", function () {
      show(0);
      dialog.showModal();
    });

    var closeBtn = dialog.querySelector("[data-story-close]");
    if (closeBtn) {
      closeBtn.addEventListener("click", function () { dialog.close(); });
    }

    // the panel fills the dialog, so a click that lands on the dialog itself is
    // a click on the backdrop
    dialog.addEventListener("click", function (e) {
      if (e.target === dialog) dialog.close();
    });

    var prev = dialog.querySelector("[data-lightbox-prev]");
    var next = dialog.querySelector("[data-lightbox-next]");
    if (prev) prev.addEventListener("click", function () { show(current - 1); });
    if (next) next.addEventListener("click", function () { show(current + 1); });

    thumbs.forEach(function (thumb, i) {
      thumb.addEventListener("click", function () { show(i); });
    });

    dialog.addEventListener("keydown", function (e) {
      if (e.key === "ArrowLeft") { e.preventDefault(); show(current - 1); }
      if (e.key === "ArrowRight") { e.preventDefault(); show(current + 1); }
    });

    // a single-image gallery has nothing to page through
    if (slides.length < 2) {
      if (prev) prev.hidden = true;
      if (next) next.hidden = true;
    }
  });

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

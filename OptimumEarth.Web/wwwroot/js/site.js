(function () {
  "use strict";

  var reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;

  var loadBackground = function (element) {
    var template = element.getAttribute("data-image-style");
    if (!template) return;
    var needed = Math.max(element.clientWidth, 1) * (window.devicePixelRatio || 1);
    var width = needed <= 480 ? 480 : needed <= 960 ? 960 : 1920;
    element.setAttribute("style", template.replace(/IMAGE_WIDTH/g, String(width)));
    element.removeAttribute("data-image-style");
  };
  var backgroundImages = document.querySelectorAll("[data-image-style]:not([data-image-deferred])");
  if ("IntersectionObserver" in window) {
    var imageObserver = new IntersectionObserver(function (entries) {
      entries.forEach(function (entry) {
        if (entry.isIntersecting) {
          loadBackground(entry.target);
          imageObserver.unobserve(entry.target);
        }
      });
    }, { rootMargin: "300px" });
    backgroundImages.forEach(function (element) { imageObserver.observe(element); });
  } else {
    backgroundImages.forEach(loadBackground);
  }

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
      slides[index].querySelectorAll("[data-image-style]").forEach(loadBackground);
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

  // Who We Are: the SCALES values are tabs. One panel shows at a time, the
  // arrow keys move between letters, and while the section is on screen it
  // advances on its own until the visitor picks a letter.
  var valuesSection = document.querySelector("[data-values]");
  if (valuesSection) {
    var valueTabs = Array.prototype.slice.call(valuesSection.querySelectorAll("[data-value-tab]"));
    var valuePanels = Array.prototype.slice.call(valuesSection.querySelectorAll("[data-value-panel]"));
    var currentValue = 0;
    var valueTimer = null;
    var valuePicked = false;

    var showValue = function (index, animate) {
      currentValue = index;
      valueTabs.forEach(function (tab, i) {
        tab.setAttribute("aria-selected", String(i === index));
        tab.tabIndex = i === index ? 0 : -1;
      });
      valuePanels.forEach(function (panel, i) {
        panel.hidden = i !== index;
        panel.classList.toggle("is-showing", animate && i === index);
      });
    };

    var stopValues = function () {
      valuePicked = true;
      clearInterval(valueTimer);
    };

    showValue(0, false);
    valueTabs.forEach(function (tab, i) {
      tab.addEventListener("click", function () {
        stopValues();
        showValue(i, true);
      });
      tab.addEventListener("keydown", function (e) {
        if (e.key !== "ArrowRight" && e.key !== "ArrowLeft") return;
        e.preventDefault();
        stopValues();
        var next = (currentValue + (e.key === "ArrowRight" ? 1 : -1) + valueTabs.length) % valueTabs.length;
        showValue(next, true);
        valueTabs[next].focus();
      });
    });

    if ("IntersectionObserver" in window && !reduceMotion) {
      new IntersectionObserver(function (entries) {
        entries.forEach(function (entry) {
          clearInterval(valueTimer);
          if (entry.isIntersecting && !valuePicked) {
            valueTimer = setInterval(function () { showValue((currentValue + 1) % valueTabs.length, true); }, 5500);
          }
        });
      }, { threshold: 0.5 }).observe(valuesSection);
    }
  }

  // Redesigned pages: stats count up from zero the first time they come into
  // view (the real figures are in the markup, so they show without JavaScript),
  // and [data-reveal] sections fade in.
  var animateRoot = document.querySelector("[data-animate-root]");
  if (animateRoot && "IntersectionObserver" in window && !reduceMotion) {
    var counters = animateRoot.querySelectorAll("[data-count]");
    var countObserver = new IntersectionObserver(function (entries) {
      entries.forEach(function (entry) {
        if (!entry.isIntersecting) return;
        countObserver.unobserve(entry.target);
        var el = entry.target;
        var end = parseInt(el.getAttribute("data-count"), 10) || 0;
        var suffix = el.getAttribute("data-suffix") || "";
        var start = performance.now();
        var step = function (now) {
          var f = Math.min((now - start) / 1100, 1);
          el.textContent = Math.round(end * (1 - Math.pow(1 - f, 3))) + suffix;
          if (f < 1) requestAnimationFrame(step);
        };
        el.textContent = "0" + suffix;
        requestAnimationFrame(step);
      });
    }, { threshold: 0.4 });
    counters.forEach(function (el) { countObserver.observe(el); });

    // Sections fade in as they scroll into view.
    animateRoot.classList.add("is-animated");
    var revealObserver = new IntersectionObserver(function (entries) {
      entries.forEach(function (entry) {
        if (entry.isIntersecting) {
          entry.target.classList.add("is-in");
          revealObserver.unobserve(entry.target);
        }
      });
    }, { threshold: 0.12 });
    animateRoot.querySelectorAll("[data-reveal]").forEach(function (el) { revealObserver.observe(el); });
  }

  // Services: the hero video only loads on wide screens with motion allowed and
  // no data saver; everywhere else the poster image stays.
  var heroVideo = document.querySelector("[data-hero-video]");
  if (heroVideo) {
    var saveData = navigator.connection && navigator.connection.saveData;
    if (!reduceMotion && !saveData && window.matchMedia("(min-width: 861px)").matches) {
      heroVideo.querySelectorAll("source[data-src]").forEach(function (source) {
        source.src = source.getAttribute("data-src");
      });
      heroVideo.load();
      var playing = heroVideo.play();
      if (playing && playing.catch) playing.catch(function () {});
    }
  }

  // Services: dots under the swipeable tile row on phones.
  var tileRow = document.querySelector("[data-tiles]");
  var tileDots = document.querySelectorAll("[data-tile-dots] i");
  if (tileRow && tileDots.length) {
    tileRow.addEventListener("scroll", function () {
      var middle = tileRow.scrollLeft + tileRow.clientWidth / 2;
      var best = 0, bestDistance = Infinity;
      Array.prototype.forEach.call(tileRow.children, function (tile, i) {
        var distance = Math.abs(tile.offsetLeft + tile.offsetWidth / 2 - middle);
        if (distance < bestDistance) { bestDistance = distance; best = i; }
      });
      tileDots.forEach(function (dot, i) { dot.classList.toggle("is-on", i === best); });
    }, { passive: true });
  }

  // Services: the jump menu marks the section in view and, on phones, keeps it scrolled into sight.
  var subnavLinks = document.querySelectorAll("[data-subnav-link]");
  if (subnavLinks.length) {
    var sections = Array.prototype.map.call(subnavLinks, function (link) {
      return document.getElementById(link.getAttribute("data-subnav-link"));
    });
    var pageEnd = document.getElementById("start");
    var lastActive = null;
    var markActive = function () {
      var line = window.scrollY + 180;
      var current = null;
      sections.forEach(function (section) {
        if (section && section.getBoundingClientRect().top + window.scrollY <= line) current = section.id;
      });
      if (pageEnd && pageEnd.getBoundingClientRect().top + window.scrollY <= line) current = null;
      if (current === lastActive) return;
      lastActive = current;
      subnavLinks.forEach(function (link) {
        var on = link.getAttribute("data-subnav-link") === current;
        link.classList.toggle("is-active", on);
        if (on) link.parentNode.scrollTo({ left: link.offsetLeft - 16, behavior: reduceMotion ? "auto" : "smooth" });
      });
    };

    // The construction band's photo drifts a little as it scrolls past (wide screens only).
    var parallax = document.querySelector("[data-parallax]");
    var wide = window.matchMedia("(min-width: 861px)");
    var drift = function () {
      if (!parallax || reduceMotion || !wide.matches) return;
      var box = parallax.parentNode.getBoundingClientRect();
      if (box.bottom < 0 || box.top > window.innerHeight) return;
      var t = (box.top + box.height / 2 - window.innerHeight / 2) / window.innerHeight;
      parallax.style.transform = "scale(1.15) translateY(" + (t * -40).toFixed(1) + "px)";
    };

    var ticking = false;
    window.addEventListener("scroll", function () {
      if (ticking) return;
      ticking = true;
      requestAnimationFrame(function () { markActive(); drift(); ticking = false; });
    }, { passive: true });
    markActive();
    drift();
  }

  // Services: arrows step the drone photo strip one photo at a time.
  var aerialTrack = document.querySelector("[data-aerial-track]");
  if (aerialTrack) {
    document.querySelectorAll("[data-aerial-step]").forEach(function (button) {
      button.addEventListener("click", function () {
        var shot = aerialTrack.querySelector("figure");
        var step = shot ? shot.offsetWidth + 16 : aerialTrack.clientWidth;
        aerialTrack.scrollBy({ left: step * Number(button.getAttribute("data-aerial-step")), behavior: reduceMotion ? "auto" : "smooth" });
      });
    });
  }

  // Services: the case-study map has two views (tabs), and the land cover view
  // is a before/after slider driven by mouse, horizontal swipes and the keyboard.
  var mapTabs = document.querySelectorAll("[data-map-tab]");
  if (mapTabs.length) {
    var showView = function (view) {
      mapTabs.forEach(function (tab) {
        var on = tab.getAttribute("data-map-tab") === view;
        tab.setAttribute("aria-selected", String(on));
        tab.tabIndex = on ? 0 : -1;
      });
      document.querySelectorAll("[data-map-view]").forEach(function (panel) { panel.hidden = panel.getAttribute("data-map-view") !== view; });
      document.querySelectorAll("[data-map-legend]").forEach(function (legend) { legend.hidden = legend.getAttribute("data-map-legend") !== view; });
    };
    mapTabs.forEach(function (tab, i) {
      tab.addEventListener("click", function () { showView(tab.getAttribute("data-map-tab")); });
      tab.addEventListener("keydown", function (e) {
        if (e.key !== "ArrowRight" && e.key !== "ArrowLeft") return;
        e.preventDefault();
        var next = mapTabs[(i + (e.key === "ArrowRight" ? 1 : -1) + mapTabs.length) % mapTabs.length];
        showView(next.getAttribute("data-map-tab"));
        next.focus();
      });
    });
  }

  var compare = document.querySelector("[data-compare]");
  if (compare) {
    var range = compare.querySelector("[data-compare-range]");
    var dragging = false;
    var setPosition = function (percent) {
      percent = Math.max(0, Math.min(100, percent));
      compare.style.setProperty("--pos", percent + "%");
      range.value = Math.round(percent);
    };
    var fromPointer = function (e) {
      var box = compare.getBoundingClientRect();
      setPosition((e.clientX - box.left) / box.width * 100);
    };
    compare.addEventListener("pointerdown", function (e) {
      if (e.pointerType === "touch") return; // touch is handled below so vertical scrolling still works
      dragging = true;
      compare.classList.add("is-touched");
      fromPointer(e);
      compare.setPointerCapture(e.pointerId);
    });
    compare.addEventListener("pointermove", function (e) { if (dragging) fromPointer(e); });
    ["pointerup", "pointercancel", "pointerleave"].forEach(function (name) {
      compare.addEventListener(name, function () { dragging = false; });
    });
    var startX = 0, startY = 0, axis = null;
    compare.addEventListener("touchstart", function (e) { startX = e.touches[0].clientX; startY = e.touches[0].clientY; axis = null; }, { passive: true });
    compare.addEventListener("touchmove", function (e) {
      var dx = e.touches[0].clientX - startX, dy = e.touches[0].clientY - startY;
      if (axis === null && (Math.abs(dx) > 6 || Math.abs(dy) > 6)) axis = Math.abs(dx) > Math.abs(dy) ? "x" : "y";
      if (axis === "x") { compare.classList.add("is-touched"); fromPointer(e.touches[0]); }
    }, { passive: true });
    range.addEventListener("input", function () { compare.classList.add("is-touched"); setPosition(Number(range.value)); });

    // One gentle sweep the first time the map is seen, to show that it moves.
    if (!reduceMotion && "IntersectionObserver" in window) {
      var sweepObserver = new IntersectionObserver(function (entries) {
        if (!entries[0].isIntersecting) return;
        sweepObserver.disconnect();
        var started = null;
        var sweep = function (now) {
          if (compare.classList.contains("is-touched")) return;
          if (started === null) started = now;
          var t = (now - started) / 2200;
          if (t >= 1) { setPosition(50); return; }
          setPosition(50 + 30 * Math.sin(t * Math.PI * 2));
          requestAnimationFrame(sweep);
        };
        requestAnimationFrame(sweep);
      }, { threshold: 0.5 });
      sweepObserver.observe(compare);
    }
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

  // Foundation SDG loops: each opens a <dialog> holding that goal's approach
  // PDF in an iframe. Same open/close wiring as the story lightbox above,
  // minus the gallery paging since there is only ever one document.
  //
  // The trigger is a role="button" div rather than a real <button> - the
  // ring is an SVG with a <mask>, and wrapping that in an actual <button>
  // trips the Razor HTML parser (RZ1025, "element was not closed") even
  // though the tags themselves balance - so Enter/Space need wiring by hand.
  var sdgTriggers = document.querySelectorAll("[data-sdg-trigger]");
  sdgTriggers.forEach(function (trigger) {
    var dialog = document.getElementById("sdg-" + trigger.getAttribute("data-sdg-trigger"));
    if (!dialog) return;

    trigger.addEventListener("click", function () { dialog.showModal(); });
    trigger.addEventListener("keydown", function (e) {
      if (e.key === "Enter" || e.key === " " || e.key === "Spacebar") {
        e.preventDefault();
        dialog.showModal();
      }
    });

    var closeBtn = dialog.querySelector("[data-story-close]");
    if (closeBtn) {
      closeBtn.addEventListener("click", function () { dialog.close(); });
    }

    dialog.addEventListener("click", function (e) {
      if (e.target === dialog) dialog.close();
    });
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

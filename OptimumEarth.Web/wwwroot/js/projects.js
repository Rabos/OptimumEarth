// Projects page: instant filtering, the detail panel, the country map and the hero slideshow.
// The server renders every card and applies the filters in the URL, so the page works without
// this script; with it, filters apply as they change and the address bar keeps up.
(function () {
  "use strict";

  var root = document.querySelector("[data-projects-page]");
  var dataEl = document.getElementById("projects-data");
  if (!root || !dataEl) return;

  var data = JSON.parse(dataEl.textContent);
  var reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
  root.classList.add("is-live");

  var index = function (list, key) {
    var map = {};
    list.forEach(function (item) { map[item[key]] = item; });
    return map;
  };
  var bySlug = index(data.projects, "slug");
  var countries = index(data.countries, "key");
  var services = index(data.services, "key");
  var sectors = index(data.sectors, "key");

  var form = root.querySelector("[data-filters]");
  var search = form.querySelector("[data-filter-search]");
  var cards = Array.prototype.slice.call(root.querySelectorAll("[data-project]"));
  var grid = root.querySelector("[data-grid]");
  var mapBox = root.querySelector("[data-map]");
  var view = mapBox.hidden ? "grid" : "map";
  var visible = [];

  var el = function (tag, className, text) {
    var node = document.createElement(tag);
    if (className) node.className = className;
    if (text !== undefined) node.textContent = text;
    return node;
  };

  // ---------- Filtering ----------

  var readFilters = function () {
    var picked = function (group) {
      return Array.prototype.map.call(form.querySelectorAll("[data-filter=\"" + group + "\"]:checked"), function (input) { return input.value; });
    };
    return {
      country: picked("country"),
      service: picked("service"),
      sector: picked("sector"),
      ongoing: picked("status").length > 0,
      q: search.value.trim().toLowerCase()
    };
  };

  var matches = function (p, f, ignore) {
    if (ignore !== "country" && f.country.length && f.country.indexOf(p.country) < 0) return false;
    if (ignore !== "service" && f.service.length && !p.services.some(function (s) { return f.service.indexOf(s) >= 0; })) return false;
    if (ignore !== "sector" && f.sector.length && f.sector.indexOf(p.sector) < 0) return false;
    if (f.ongoing && !p.ongoing) return false;
    if (f.q && !f.q.split(/\s+/).every(function (word) { return p.search.indexOf(word) >= 0; })) return false;
    return true;
  };

  var has = function (p, group, key) {
    if (group === "country") return p.country === key;
    if (group === "service") return p.services.indexOf(key) >= 0;
    return p.sector === key;
  };

  var labelOf = function (group, key) {
    if (group === "country") return countries[key] ? countries[key].name : key;
    if (group === "service") return services[key] ? services[key].label : key;
    if (group === "sector") return sectors[key] ? sectors[key].name : key;
    return key;
  };

  var writeUrl = function (f) {
    var params = new URLSearchParams();
    f.country.forEach(function (v) { params.append("country", v); });
    f.service.forEach(function (v) { params.append("service", v); });
    f.sector.forEach(function (v) { params.append("sector", v); });
    if (f.ongoing) params.set("status", "ongoing");
    if (f.q) params.set("q", search.value.trim());
    if (view === "map") params.set("view", "map");
    var query = params.toString();
    history.replaceState(null, "", location.pathname + (query ? "?" + query : "") + location.hash);
  };

  var renderPills = function (f) {
    var box = root.querySelector("[data-active-pills]");
    box.textContent = "";
    var add = function (label, remove) {
      var pill = el("a", "prj-pill");
      pill.href = "#explore";
      pill.setAttribute("data-remove", remove);
      pill.appendChild(document.createTextNode(label + " "));
      var x = el("span", null, "×");
      x.setAttribute("aria-hidden", "true");
      pill.appendChild(x);
      pill.appendChild(el("span", "visually-hidden", " (remove)"));
      box.appendChild(pill);
    };
    ["country", "service", "sector"].forEach(function (group) {
      f[group].forEach(function (key) { add(labelOf(group, key), group + ":" + key); });
    });
    if (f.ongoing) add("Ongoing only", "status:ongoing");
    if (f.q) add("“" + search.value.trim() + "”", "q:");
    if (box.children.length) {
      var clear = el("a", "prj-clear", "Clear all");
      clear.href = "#explore";
      clear.setAttribute("data-clear", "");
      box.appendChild(clear);
    }
  };

  var apply = function () {
    var f = readFilters();
    visible = [];
    cards.forEach(function (card) {
      var p = bySlug[card.getAttribute("data-project")];
      var ok = matches(p, f);
      card.hidden = !ok;
      if (ok) visible.push(p);
    });

    root.querySelectorAll("[data-count-for]").forEach(function (counter) {
      var parts = counter.getAttribute("data-count-for").split(":");
      var n = data.projects.filter(function (p) { return matches(p, f, parts[0]) && has(p, parts[0], parts[1]); }).length;
      counter.textContent = n;
      var row = counter.closest(".prj-check");
      row.classList.toggle("is-empty", n === 0 && !row.querySelector("input").checked);
    });
    ["country", "service", "sector"].forEach(function (group) {
      var badge = root.querySelector("[data-dd-count=\"" + group + "\"]");
      if (badge) badge.textContent = f[group].length ? f[group].length : "";
    });

    renderPills(f);
    root.querySelector("[data-result-count]").innerHTML = "Showing <b>" + visible.length + "</b> of " + data.projects.length + " projects";
    root.querySelector("[data-empty]").hidden = visible.length > 0;
    if (view === "map") renderMap();
    writeUrl(f);
  };

  var clearAll = function () {
    form.querySelectorAll("[data-filter]:checked").forEach(function (input) { input.checked = false; });
    search.value = "";
    apply();
  };

  form.addEventListener("submit", function (e) { e.preventDefault(); apply(); });
  form.addEventListener("change", function (e) { if (e.target.hasAttribute("data-filter")) apply(); });
  var typing;
  search.addEventListener("input", function () {
    clearTimeout(typing);
    typing = setTimeout(apply, 160);
  });

  root.addEventListener("click", function (e) {
    var remove = e.target.closest("[data-remove]");
    if (remove) {
      e.preventDefault();
      var parts = remove.getAttribute("data-remove").split(":");
      if (parts[0] === "q") search.value = "";
      else {
        var input = form.querySelector("[data-filter=\"" + parts[0] + "\"][value=\"" + parts[1] + "\"]");
        if (input) input.checked = false;
      }
      apply();
      return;
    }
    if (e.target.closest("[data-clear]")) {
      e.preventDefault();
      clearAll();
    }
  });

  // ---------- Dropdowns ----------

  var dropdowns = Array.prototype.slice.call(root.querySelectorAll("[data-dd]"));
  var closeDropdowns = function (except) {
    dropdowns.forEach(function (dd) { if (dd !== except) dd.open = false; });
  };
  dropdowns.forEach(function (dd) {
    dd.addEventListener("toggle", function () { if (dd.open) closeDropdowns(dd); });
    dd.querySelector("[data-dd-close]").addEventListener("click", function () {
      dd.open = false;
      dd.querySelector("summary").focus();
    });
  });
  document.addEventListener("click", function (e) {
    if (!e.target.closest("[data-dd]")) closeDropdowns(null);
  });
  // A tap on the dimmed backdrop behind a phone sheet lands on the <details> itself.
  dropdowns.forEach(function (dd) {
    dd.addEventListener("click", function (e) { if (e.target === dd) dd.open = false; });
  });
  document.addEventListener("keydown", function (e) {
    if (e.key !== "Escape") return;
    var open = dropdowns.filter(function (dd) { return dd.open; })[0];
    if (open) {
      open.open = false;
      open.querySelector("summary").focus();
    }
  });

  // ---------- Grid / map ----------

  var setView = function (next) {
    view = next;
    grid.hidden = view !== "grid";
    mapBox.hidden = view !== "map";
    root.querySelectorAll("[data-view]").forEach(function (button) {
      button.setAttribute("aria-pressed", String(button.getAttribute("data-view") === view));
    });
    if (view === "map") showMap();
    writeUrl(readFilters());
  };

  root.querySelectorAll("[data-view]").forEach(function (button) {
    button.addEventListener("click", function (e) {
      e.preventDefault();
      setView(button.getAttribute("data-view"));
    });
  });
  root.querySelectorAll("[data-show-map]").forEach(function (link) {
    link.addEventListener("click", function (e) {
      e.preventDefault();
      setView("map");
      document.getElementById("explore").scrollIntoView({ behavior: reduceMotion ? "auto" : "smooth" });
    });
  });

  // ---------- Map ----------
  // Leaflet is served from the site itself and only loaded when the map is first shown.
  // Every project is pinned at its country, so each pin is a numbered bubble.

  var canvas = root.querySelector("[data-map-canvas]");
  var list = root.querySelector("[data-map-list]");
  var map = null, pins = null, leafletState = "idle", waiting = [];

  var loadLeaflet = function (done) {
    if (window.L) { done(); return; }
    waiting.push(done);
    if (leafletState !== "idle") return;
    leafletState = "loading";
    var css = document.createElement("link");
    css.rel = "stylesheet";
    css.href = "/lib/leaflet/leaflet.css";
    document.head.appendChild(css);
    var script = document.createElement("script");
    script.src = "/lib/leaflet/leaflet.js";
    script.onload = function () { leafletState = "ready"; waiting.splice(0).forEach(function (fn) { fn(); }); };
    script.onerror = function () {
      leafletState = "failed";
      canvas.textContent = "";
      canvas.appendChild(el("p", "prj-map__fallback", "The map could not load. The list beside it shows every matching project."));
    };
    document.head.appendChild(script);
  };

  var showMap = function () {
    renderList();
    loadLeaflet(function () {
      if (!map) {
        map = L.map(canvas, { scrollWheelZoom: false, minZoom: 3, maxZoom: 12 });
        var failures = 0, notice = null;
        L.tileLayer("https://tile.openstreetmap.org/{z}/{x}/{y}.png", {
          maxZoom: 19,
          attribution: "&copy; <a href=\"https://www.openstreetmap.org/copyright\">OpenStreetMap</a> contributors"
        }).on("tileerror", function () {
          failures += 1;
          if (failures === 4 && !notice) {
            notice = el("p", "prj-map__notice", "The map background could not load, but the pins and the list are correct.");
            canvas.appendChild(notice);
          }
        }).addTo(map);
        pins = L.layerGroup().addTo(map);
        // Scroll-zoom only once someone clicks into the map, so the page still scrolls past it.
        map.on("click", function () { map.scrollWheelZoom.enable(); });
      }
      renderMap();
      setTimeout(function () { map.invalidateSize(); fitPins(); }, 60);
    });
  };

  var groups = function () {
    var byCountry = {};
    visible.forEach(function (p) {
      (byCountry[p.country] = byCountry[p.country] || []).push(p);
    });
    return data.countries.filter(function (c) { return byCountry[c.key]; }).map(function (c) {
      return { country: c, projects: byCountry[c.key] };
    });
  };

  var renderList = function () {
    list.textContent = "";
    groups().forEach(function (g) {
      var box = el("div", "prj-map__group");
      box.setAttribute("data-map-group", g.country.key);
      var head = el("h3", null, g.country.name + " ");
      head.appendChild(el("span", null, String(g.projects.length)));
      box.appendChild(head);
      g.projects.forEach(function (p) {
        var link = el("a");
        link.href = "#" + p.slug;
        link.setAttribute("data-open", p.slug);
        link.appendChild(el("strong", null, p.title));
        link.appendChild(el("span", null, p.client + (p.location ? " · " + p.location : "")));
        box.appendChild(link);
      });
      list.appendChild(box);
    });
    if (!list.children.length) list.appendChild(el("p", "prj-map__fallback", "No projects match these filters."));
  };

  var highlight = function (key) {
    list.querySelectorAll("[data-map-group]").forEach(function (box) {
      var on = box.getAttribute("data-map-group") === key;
      box.classList.toggle("is-hl", on);
      if (on) list.scrollTo({ top: box.offsetTop - list.offsetTop, behavior: reduceMotion ? "auto" : "smooth" });
    });
  };

  var onlyCountry = function (key) {
    form.querySelectorAll("[data-filter=\"country\"]").forEach(function (input) { input.checked = input.value === key; });
    apply();
  };

  var renderMap = function () {
    renderList();
    if (!map) return;
    pins.clearLayers();
    groups().forEach(function (g) {
      var n = g.projects.length;
      var ongoing = g.projects.filter(function (p) { return p.ongoing; }).length;
      var size = Math.round(34 + Math.min(30, Math.sqrt(n) * 5));
      var html = "<span class=\"prj-pin" + (ongoing ? " prj-pin--ongoing" : "") + "\">" + n + "</span>";
      var marker = L.marker([g.country.lat, g.country.lng], {
        icon: L.divIcon({ className: "", html: html, iconSize: [size, size] }),
        title: g.country.name + ": " + n + (n === 1 ? " project" : " projects"),
        keyboard: true
      });

      var popup = el("div");
      popup.appendChild(el("b", null, g.country.name));
      popup.appendChild(document.createTextNode(n + (n === 1 ? " project" : " projects") + (ongoing ? ", " + ongoing + " ongoing" : "")));
      popup.appendChild(el("br"));
      var only = el("button", null, "Show only " + g.country.name + " →");
      only.type = "button";
      only.addEventListener("click", function () { map.closePopup(); onlyCountry(g.country.key); });
      popup.appendChild(only);

      marker.bindPopup(popup).on("click", function () { highlight(g.country.key); }).addTo(pins);
    });
  };

  var fitPins = function () {
    var points = groups().map(function (g) { return [g.country.lat, g.country.lng]; });
    if (points.length > 1) map.fitBounds(L.latLngBounds(points).pad(0.3), { maxZoom: 6 });
    else if (points.length === 1) map.setView(points[0], 6);
    else map.setView([-2, 29], 4);
  };

  // ---------- Detail panel ----------

  var drawer = root.querySelector("[data-drawer]");
  var scrim = root.querySelector("[data-drawer-scrim]");
  var media = drawer.querySelector("[data-drawer-media]");
  var body = drawer.querySelector("[data-drawer-body]");
  var prev = drawer.querySelector("[data-drawer-prev]");
  var next = drawer.querySelector("[data-drawer-next]");
  var openSlug = null, lastFocus = null;

  var browseList = function (slug) {
    var inView = visible.some(function (p) { return p.slug === slug; });
    return inView ? visible : cards.map(function (card) { return bySlug[card.getAttribute("data-project")]; });
  };

  var openProject = function (slug) {
    var p = bySlug[slug];
    if (!p) return;
    if (!openSlug) lastFocus = document.activeElement;
    openSlug = slug;

    media.style.backgroundImage = "url(\"" + p.image.replace(/"/g, "%22") + "?w=960\")";
    drawer.querySelector("[data-drawer-rep]").hidden = !p.representative;

    body.textContent = "";
    var eyebrow = el("span", "prj-eyebrow", (countries[p.country] ? countries[p.country].name : "") + (sectors[p.sector] ? " · " + sectors[p.sector].name : ""));
    if (p.ongoing) {
      eyebrow.appendChild(document.createTextNode(" · "));
      eyebrow.appendChild(el("span", "prj-ongoing", "Ongoing"));
    }
    body.appendChild(eyebrow);
    var title = el("h2", null, p.title);
    title.id = "drawer-title";
    body.appendChild(title);
    body.appendChild(el("p", null, p.description));

    var meta = el("dl", "prj-meta");
    var row = function (label, value, full) {
      if (!value) return;
      var box = el("div", full ? "is-full" : null);
      box.appendChild(el("dt", null, label));
      var dd = el("dd");
      if (typeof value === "string") dd.textContent = value; else dd.appendChild(value);
      box.appendChild(dd);
      meta.appendChild(box);
    };
    row("Client", p.client, true);
    row("Location", p.location);
    row("Country", countries[p.country] ? countries[p.country].name : "");
    row("Sector", sectors[p.sector] ? sectors[p.sector].name : "");
    row("Status", p.ongoing ? "Ongoing" : "Completed");
    var serviceLinks = document.createDocumentFragment();
    p.services.forEach(function (key, i) {
      if (!services[key]) return;
      if (i) serviceLinks.appendChild(document.createTextNode(", "));
      var a = el("a", null, services[key].label);
      a.href = "/services#" + services[key].anchor;
      serviceLinks.appendChild(a);
    });
    row("Services", serviceLinks, true);
    body.appendChild(meta);

    var actions = el("div", "prj-drawer__actions");
    var discuss = el("a", "prj-btn prj-btn--solid", "Discuss a similar project");
    discuss.href = "/contact?project=similar&about=" + encodeURIComponent(p.title);
    actions.appendChild(discuss);
    var onMap = el("button", "prj-btn prj-btn--line", "Show on the map");
    onMap.type = "button";
    onMap.addEventListener("click", function () {
      closeProject();
      setView("map");
      document.getElementById("explore").scrollIntoView({ behavior: reduceMotion ? "auto" : "smooth" });
      highlight(p.country);
    });
    actions.appendChild(onMap);
    body.appendChild(actions);

    var order = browseList(slug);
    var at = order.indexOf(p);
    prev.disabled = at <= 0;
    next.disabled = at < 0 || at >= order.length - 1;
    prev.onclick = function () { openProject(order[at - 1].slug); };
    next.onclick = function () { openProject(order[at + 1].slug); };

    drawer.hidden = false;
    scrim.hidden = false;
    body.scrollTop = 0;
    document.body.style.overflow = "hidden";
    if (location.hash !== "#" + slug) history.replaceState(null, "", location.pathname + location.search + "#" + slug);
    drawer.querySelector("[data-drawer-close]").focus();
  };

  var closeProject = function () {
    if (!openSlug) return;
    openSlug = null;
    drawer.hidden = true;
    scrim.hidden = true;
    document.body.style.overflow = "";
    history.replaceState(null, "", location.pathname + location.search);
    if (lastFocus) lastFocus.focus();
  };

  root.addEventListener("click", function (e) {
    var opener = e.target.closest("[data-open], [data-project]");
    if (!opener || !root.contains(opener)) return;
    e.preventDefault();
    if (map) map.closePopup();
    openProject(opener.getAttribute("data-open") || opener.getAttribute("data-project"));
  });
  scrim.addEventListener("click", closeProject);
  drawer.querySelector("[data-drawer-close]").addEventListener("click", closeProject);
  document.addEventListener("keydown", function (e) {
    if (!openSlug) return;
    if (e.key === "Escape") closeProject();
    else if (e.key === "ArrowRight" && !next.disabled) next.click();
    else if (e.key === "ArrowLeft" && !prev.disabled) prev.click();
    else if (e.key === "Tab") {
      // Keep keyboard focus inside the open panel.
      var focusable = drawer.querySelectorAll("a[href], button:not([disabled])");
      var first = focusable[0], last = focusable[focusable.length - 1];
      if (e.shiftKey && document.activeElement === first) { e.preventDefault(); last.focus(); }
      else if (!e.shiftKey && document.activeElement === last) { e.preventDefault(); first.focus(); }
    }
  });
  window.addEventListener("hashchange", function () {
    var slug = location.hash.slice(1);
    if (bySlug[slug]) openProject(slug);
  });

  // ---------- Flagship row ----------

  var rail = root.querySelector("[data-rail-track]");
  root.querySelectorAll("[data-rail]").forEach(function (button) {
    button.addEventListener("click", function () {
      rail.scrollBy({ left: Number(button.getAttribute("data-rail")) * rail.clientWidth * 0.8, behavior: reduceMotion ? "auto" : "smooth" });
    });
  });

  // ---------- Hero slideshow ----------

  var slides = root.querySelector("[data-slides]");
  if (slides) {
    var figures = slides.querySelectorAll("figure");
    var dots = slides.querySelectorAll(".prj-slides__dots i");
    var current = 0;
    var go = function (n) {
      figures[current].classList.remove("is-on");
      current = n % figures.length;
      figures[current].classList.add("is-on");
      dots.forEach(function (dot, j) {
        dot.classList.toggle("is-done", j < current);
        dot.classList.remove("is-on");
      });
      void dots[current].offsetWidth; // restart the progress animation
      dots[current].classList.add("is-on");
    };
    go(0);
    if (!reduceMotion && figures.length > 1) setInterval(function () { go(current + 1); }, 6000);
  }

  // ---------- Start ----------

  apply();
  if (view === "map") showMap();
  var startSlug = location.hash.slice(1);
  if (bySlug[startSlug]) openProject(startSlug);
})();

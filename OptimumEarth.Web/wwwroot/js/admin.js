// Dashboard behaviour. Plain JavaScript, no dependencies; every form still posts normally without it.
(function () {
  'use strict';

  // Character counters: <input data-max="80"> with a sibling <span data-count-for="id">.
  document.addEventListener('input', function (e) {
    var el = e.target;
    if (!el.dataset || !el.dataset.max) { return; }
    var counter = document.querySelector('[data-count-for="' + el.id + '"]');
    if (!counter) { return; }
    counter.textContent = el.value.length + '/' + el.dataset.max;
    counter.classList.toggle('over', el.value.length > Number(el.dataset.max));
  });

  // Image pickers: show a thumbnail of the chosen image.
  document.addEventListener('change', function (e) {
    var el = e.target;
    if (el.matches && el.matches('select[data-image-picker]')) {
      var thumb = document.getElementById(el.dataset.imagePicker);
      if (thumb) { thumb.style.backgroundImage = el.value ? "url('" + el.value + "')" : ''; }
    }
  });

  // Copy buttons: <button data-copy="#selector">.
  document.addEventListener('click', function (e) {
    var btn = e.target.closest && e.target.closest('[data-copy]');
    if (!btn) { return; }
    var source = document.querySelector(btn.dataset.copy);
    if (!source) { return; }
    var text = source.value || source.textContent;
    var done = function () { var old = btn.textContent; btn.textContent = 'Copied'; setTimeout(function () { btn.textContent = old; }, 1500); };
    if (navigator.clipboard && navigator.clipboard.writeText) {
      navigator.clipboard.writeText(text).then(done, function () { source.select && source.select(); });
    } else if (source.select) {
      source.select();
    }
  });

  // Access presets on the Users form: <button data-preset='{"blog":"edit","media":"view"}'>.
  document.addEventListener('click', function (e) {
    var btn = e.target.closest && e.target.closest('[data-preset]');
    if (!btn) { return; }
    e.preventDefault();
    var preset = JSON.parse(btn.dataset.preset);
    var role = document.querySelector('[name="Input.Role"]');
    var isViewer = role && role.value === 'Viewer';
    document.querySelectorAll('input[type="radio"][data-area]').forEach(function (radio) {
      var wanted = preset[radio.dataset.area] || preset['*'] || 'none';
      if (isViewer && wanted === 'edit') { wanted = 'view'; }
      radio.checked = radio.value === wanted;
    });
  });

  // A Viewer can never be given Edit: disable those radios when the role is Viewer.
  function syncRole() {
    var role = document.querySelector('[name="Input.Role"]');
    if (!role) { return; }
    var isViewer = role.value === 'Viewer';
    document.querySelectorAll('input[type="radio"][data-area][value="edit"]').forEach(function (radio) {
      radio.disabled = isViewer;
      if (isViewer && radio.checked) {
        var view = document.querySelector('input[type="radio"][data-area="' + radio.dataset.area + '"][value="view"]');
        if (view) { view.checked = true; }
      }
    });
  }
  document.addEventListener('change', function (e) { if (e.target.name === 'Input.Role') { syncRole(); } });
  syncRole();
})();

// Selects and fields that reload the list when changed: <select data-autosubmit>.
(function () {
  'use strict';
  document.addEventListener('change', function (e) {
    var el = e.target;
    if (el.matches && el.matches('[data-autosubmit]') && el.form) { el.form.submit(); }
  });
})();

// Markdown editor: toolbar buttons, a live preview, and reading time.
(function () {
  'use strict';

  function esc(t) { return t.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;'); }
  function inline(t) {
    return esc(t)
      .replace(/\*\*(.+?)\*\*/g, '<b>$1</b>')
      .replace(/\*(.+?)\*/g, '<i>$1</i>')
      .replace(/\[([^\]]+)\]\((https?:\/\/[^)\s]+)\)/g, '<a href="$2" target="_blank" rel="noopener">$1</a>');
  }
  function render(src) {
    var out = [], list = false;
    String(src || '').split('\n').forEach(function (l) {
      if (/^[-*]\s+/.test(l)) {
        if (!list) { out.push('<ul>'); list = true; }
        out.push('<li>' + inline(l.replace(/^[-*]\s+/, '')) + '</li>');
        return;
      }
      if (list) { out.push('</ul>'); list = false; }
      var m = l.match(/^(#{1,3})\s+(.*)/);
      if (m) { out.push('<h' + (m[1].length + 1) + '>' + inline(m[2]) + '</h' + (m[1].length + 1) + '>'); }
      else if (l.trim()) { out.push('<p>' + inline(l) + '</p>'); }
    });
    if (list) { out.push('</ul>'); }
    return out.join('') || '<p class="hint">Nothing to preview yet.</p>';
  }
  function readTime(t) {
    var words = String(t || '').trim().split(/\s+/).filter(Boolean).length;
    return Math.max(1, Math.round(words / 200)) + ' min read';
  }
  function refresh(area) {
    var preview = document.getElementById(area.id + '-preview');
    if (preview) { preview.innerHTML = render(area.value); }
    var rt = document.querySelector('[data-reading-for="' + area.id + '"]');
    if (rt) { rt.textContent = readTime(area.value); }
  }

  document.querySelectorAll('textarea[data-markdown]').forEach(refresh);
  document.addEventListener('input', function (e) { if (e.target.matches && e.target.matches('textarea[data-markdown]')) { refresh(e.target); } });
  document.addEventListener('click', function (e) {
    var btn = e.target.closest && e.target.closest('[data-md]');
    if (!btn) { return; }
    var ta = document.getElementById(btn.dataset.target);
    if (!ta || ta.disabled) { return; }
    var a = ta.selectionStart, b = ta.selectionEnd, v = ta.value, sel = v.slice(a, b) || 'text';
    var wrap = { b: ['**', '**'], i: ['*', '*'], h: ['\n## ', ''], a: ['[', '](https://)'], ul: ['\n- ', ''] }[btn.dataset.md];
    ta.value = v.slice(0, a) + wrap[0] + sel + wrap[1] + v.slice(b);
    ta.focus();
    refresh(ta);
  });
})();

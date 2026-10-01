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

// Pickers (country pages): choose items, reorder them, override wording. Rows post as parallel arrays in on-screen order.
(function () {
  'use strict';

  function rows(p) { return Array.prototype.slice.call(p.querySelectorAll('[data-picker-row]')); }

  function refresh(p) {
    var all = rows(p);
    var max = p.dataset.max ? Number(p.dataset.max) : Infinity;
    var count = p.querySelector('[data-picker-count]');
    if (count) { count.textContent = max === Infinity ? all.length + ' chosen, no limit' : all.length + ' of ' + max + ' allowed'; }
    var empty = p.querySelector('[data-picker-empty]');
    if (empty) { empty.hidden = all.length > 0; }
    all.forEach(function (row, i) {
      var up = row.querySelector('[data-pick="up"]'), down = row.querySelector('[data-pick="down"]');
      if (up) { up.disabled = i === 0; }
      if (down) { down.disabled = i === all.length - 1; }
    });
    var full = all.length >= max;
    var select = p.querySelector('[data-picker-select]');
    var add = p.querySelector('[data-pick="add"]');
    var custom = p.querySelector('[data-pick="custom"]');
    if (add) { add.disabled = full || !select || !select.querySelector('option:not(:disabled)'); }
    if (custom) { custom.disabled = full; }
    if (select && select.selectedOptions[0] && select.selectedOptions[0].disabled) {
      var next = select.querySelector('option:not(:disabled)');
      if (next) { select.value = next.value; }
    }
  }

  function addRow(p, data) {
    var tpl = p.querySelector('[data-picker-template]');
    var row = tpl.content.firstElementChild.cloneNode(true);
    row.querySelector('[data-label]').textContent = data.label;
    row.querySelector('[data-field="id"]').value = data.id || '';
    var title = row.querySelector('[data-field="title"]'), text = row.querySelector('[data-field="text"]');
    title.placeholder = data.pt || ''; text.placeholder = (data.px || '').length > 70 ? data.px.slice(0, 70) + '…' : (data.px || '');
    var badges = row.querySelector('[data-badges]');
    badges.innerHTML = '';
    if (data.custom) { badges.innerHTML = '<span class="tag t-blue">Custom</span>'; }
    if (data.status === 'draft') { badges.innerHTML += ' <span class="tag t-amber">Draft: hidden on the site</span>'; }
    if (data.custom) {
      row.querySelector('[data-title-label]').textContent = 'Card title';
      row.querySelector('[data-text-label]').textContent = 'Card text';
      var urlField = row.querySelector('[data-url-field]');
      if (urlField) { urlField.hidden = false; }
    }
    p.querySelector('[data-picker-list]').appendChild(row);
    refresh(p);
    if (data.custom) { title.focus(); }
  }

  document.addEventListener('click', function (e) {
    var btn = e.target.closest && e.target.closest('[data-pick]');
    if (!btn) { return; }
    var p = btn.closest('.picker');
    if (!p) { return; }
    e.preventDefault();
    var act = btn.dataset.pick, row = btn.closest('[data-picker-row]');
    if (act === 'add') {
      var sel = p.querySelector('[data-picker-select]'), opt = sel && sel.selectedOptions[0];
      if (!opt || opt.disabled) { return; }
      opt.disabled = true;
      addRow(p, { id: opt.value, label: opt.textContent.replace(/ \(draft\)$/, ''), pt: opt.dataset.title, px: opt.dataset.text, status: opt.dataset.status });
    } else if (act === 'custom') {
      addRow(p, { custom: true, label: 'Custom card' });
    } else if (act === 'remove' && row) {
      var id = row.querySelector('[data-field="id"]').value;
      var sel2 = p.querySelector('[data-picker-select]');
      if (id && sel2) { var o = sel2.querySelector('option[value="' + id + '"]'); if (o) { o.disabled = false; } }
      row.remove();
      refresh(p);
    } else if (act === 'up' && row && row.previousElementSibling) {
      row.parentNode.insertBefore(row, row.previousElementSibling); refresh(p);
    } else if (act === 'down' && row && row.nextElementSibling) {
      row.parentNode.insertBefore(row.nextElementSibling, row); refresh(p);
    }
  });

  document.addEventListener('input', function (e) {
    // Keep a custom card's heading in step with its title.
    var el = e.target;
    if (el.dataset && el.dataset.field === 'title') {
      var row = el.closest('[data-picker-row]');
      if (row && !row.querySelector('[data-field="id"]').value) {
        row.querySelector('[data-label]').textContent = el.value || 'Custom card';
      }
    }
  });

  document.querySelectorAll('.picker').forEach(refresh);
})();

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

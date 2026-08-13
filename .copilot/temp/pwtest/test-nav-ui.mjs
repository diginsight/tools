import { chromium } from 'playwright-core';
import { mkdirSync } from 'node:fs';

const sleep = (ms) => new Promise(r => setTimeout(r, ms));
const BASE = 'https://localhost:7280';
const SHOTS = 'c:/dev/darioairoldi/Learn.01/src/docs/90. Issues/202608/20260803.02-learn-fix/_validation/images';
mkdirSync(SHOTS, { recursive: true });

// Pre-existing and unrelated to the counter change: the home page probes for an optional index.md.
const KNOWN_404 = '/_content-raw/index.md';

const results = [];
const record = (name, ok, detail = '') => {
    results.push({ name, ok, detail });
    console.log(`${ok ? 'PASS' : 'FAIL'}  ${name}${detail ? '  — ' + detail : ''}`);
};

const footer = (page) => page.evaluate(() =>
    (document.querySelector('.bottombar-left')?.innerText || '').replace(/\s+/g, ' ').trim());

// Locale-tolerant: the footer formats with the browser's culture, so the group separator may be
// "," or "." or a narrow no-break space. Strip everything that is not a digit.
const totalOf = (text) => {
    const m = /Total:\s*(≥\s*)?([\d.,\u00a0\u202f ]*\d)\s*articles/.exec(text);
    return m ? { n: parseInt(m[2].replace(/\D/g, ''), 10), bounded: !!m[1] } : { n: null, bounded: false };
};

// Waits for the footer to show an exact (Complete-coverage) total, recording everything it displayed.
const waitExact = async (page, timeoutMs = 45000) => {
    const seen = [];
    const deadline = Date.now() + timeoutMs;
    while (Date.now() < deadline) {
        const t = totalOf(await footer(page));
        seen.push(t.n === null ? '—' : (t.bounded ? '≥' : '') + t.n);
        if (t.n !== null && !t.bounded) return { ...t, seen };
        await sleep(400);
    }
    return { n: null, bounded: false, seen };
};

const api = async (page, method, url) =>
    page.evaluate(async ([m, u]) => (await fetch(u, { method: m })).status, [method, url]);

const run = async () => {
    const browser = await chromium.launch({ channel: 'msedge', headless: false, slowMo: 200 });
    const ctx = await browser.newContext({ ignoreHTTPSErrors: true, viewport: { width: 1500, height: 980 } });
    const page = await ctx.newPage();

    const bad = [];
    page.on('response', r => { if (r.status() >= 400 && !r.url().includes(KNOWN_404)) bad.push(r.status() + ' ' + r.url()); });
    page.on('pageerror', e => bad.push('pageerror: ' + e.message));

    // ---- 1. Simple load -------------------------------------------------------------------
    console.log('\n=== 1. simple load ===');
    await page.goto(BASE + '/', { waitUntil: 'domcontentloaded' });
    const load = await waitExact(page);
    const loadSeq = [...new Set(load.seen)].join(' → ');
    record('footer reaches an exact total', load.n !== null, String(load.n));
    record('never displayed 0', !load.seen.some(v => v === '0' || v === '≥0'), 'sequence: ' + loadSeq);
    record('pre-final values are labelled lower bounds',
        load.seen.slice(0, -1).every(v => v === '—' || v.startsWith('≥')), 'sequence: ' + loadSeq);
    await page.screenshot({ path: `${SHOTS}/01-simple-load-total.png` });
    const baseline = load.n;

    // ---- 2. Expand all --------------------------------------------------------------------
    console.log('\n=== 2. expand all — sample the counter throughout ===');
    const before = await page.evaluate(() => document.querySelectorAll('.dynnav details[open]').length);
    await page.locator('.sidebar-tool[title="Expand all"]').first().click();

    const samples = [];
    for (let i = 0; i < 30; i++) {
        const t = totalOf(await footer(page));
        samples.push(t.n === null ? '—' : (t.bounded ? '≥' : '') + t.n);
        await sleep(350);
    }
    const opened = await page.evaluate(() => document.querySelectorAll('.dynnav details[open]').length);
    record('tree actually expanded', opened > before + 20, `${before} → ${opened} open sections`);
    record('counter constant through expand-all',
        new Set(samples).size === 1 && samples[0] === String(baseline),
        'samples: ' + [...new Set(samples)].join(', '));
    record('counter never showed 0 or a lower bound',
        !samples.some(v => v === '—' || v.startsWith('≥') || v === '0'));
    await page.screenshot({ path: `${SHOTS}/02-expand-all-total-stable.png` });

    // ---- 3. Live add ----------------------------------------------------------------------
    console.log('\n=== 3. add an article while the page is open ===');
    record('add endpoint accepted',
        (await api(page, 'POST', '/_test/article?folder=05.00-issues&name=zz-browser-probe')) === 200);
    let after = null;
    for (let i = 0; i < 25 && after !== baseline + 1; i++) { await sleep(400); after = totalOf(await footer(page)).n; }
    record('total incremented live (no reload)', after === baseline + 1, `${baseline} → ${after}`);
    record('total still exact after add', !totalOf(await footer(page)).bounded);
    await page.screenshot({ path: `${SHOTS}/03-live-add-total-plus-one.png` });

    const issues = page.locator('.dynnav summary .nav-label').filter({ hasText: /^Issues & Solutions$/ }).first();
    await issues.hover();
    await sleep(1200);
    const line = await footer(page);
    record('hovered section shows its updated count', /Issues & Solutions:\s*4\s*articles/.test(line), line); await page.screenshot({ path: `${SHOTS}/04-section-line-updated.png` });

    // ---- 4. Live remove -------------------------------------------------------------------
    console.log('\n=== 4. remove the article ===');
    record('delete endpoint accepted',
        (await api(page, 'DELETE', '/_test/article?folder=05.00-issues&name=zz-browser-probe')) === 200);
    let back = null;
    for (let i = 0; i < 25 && back !== baseline; i++) { await sleep(400); back = totalOf(await footer(page)).n; }
    record('total returned to baseline', back === baseline, `${after} → ${back}`);
    record('total still exact after remove', !totalOf(await footer(page)).bounded);
    await page.screenshot({ path: `${SHOTS}/05-live-remove-back-to-baseline.png` });

    // ---- 5. Reload ------------------------------------------------------------------------
    console.log('\n=== 5. reload — counter must not restart from zero ===');
    // Collapse first: reloading with ~60 expanded sections re-fetches every level and can exceed
    // Playwright's navigation budget, which is a harness limit, not app behavior.
    await page.locator('.sidebar-tool[title="Collapse all"]').first().click();
    await sleep(500);
    await page.goto(BASE + '/', { waitUntil: 'commit', timeout: 60000 });
    const again = await waitExact(page);
    record('never displayed 0 during reload', !again.seen.some(v => v === '0' || v === '≥0'),
        'sequence: ' + [...new Set(again.seen)].join(' → '));
    record('settles back on baseline', again.n === baseline, `final ${again.n}`);
    await page.screenshot({ path: `${SHOTS}/06-reload-no-zero.png` });

    record('no unexpected failed requests or JS errors', bad.length === 0, [...new Set(bad)].slice(0, 3).join(' | '));

    console.log('\n================================');
    const failed = results.filter(r => !r.ok);
    console.log(`PASS: ${results.length - failed.length}   FAIL: ${failed.length}`);
    await browser.close();
    process.exit(failed.length ? 1 : 0);
};

run().catch(e => { console.error(e); process.exit(1); });

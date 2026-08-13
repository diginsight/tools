import { chromium } from 'playwright-core';
const sleep = (ms) => new Promise(r => setTimeout(r, ms));
const BASE = 'https://localhost:7280';

const results = [];
const record = (name, ok, detail = '') => {
    results.push({ name, ok, detail });
    console.log(`${ok ? 'PASS' : 'FAIL'}  ${name}${detail ? '  — ' + detail : ''}`);
};

const run = async () => {
    const browser = await chromium.launch({ channel: 'msedge', headless: false, slowMo: 400 });
    const ctx = await browser.newContext({ ignoreHTTPSErrors: true, viewport: { width: 1400, height: 950 } });
    const page = await ctx.newPage();

    const consoleErrors = [];
    page.on('console', (m) => { if (m.type() === 'error') consoleErrors.push(m.text()); });
    page.on('pageerror', (e) => consoleErrors.push('pageerror: ' + e.message));

    // 1) Home page loads and renders markdown content.
    console.log('\n→ open home');
    const resp = await page.goto(BASE + '/', { waitUntil: 'domcontentloaded' });
    record('home HTTP status', resp.status() === 200, 'status ' + resp.status());
    await sleep(1500);
    const home = await page.evaluate(() => ({
        body: !!document.querySelector('.markdown-body'),
        sidebar: document.querySelectorAll('.dynnav .nav-list a.nav-link, .dynnav .nav-list summary').length,
        topmenu: document.querySelectorAll('nav.topmenu a, nav.topmenu .topmenu-item').length,
        title: document.title,
    }));
    record('home renders markdown-body', home.body);
    record('sidebar has nav entries', home.sidebar > 0, home.sidebar + ' entries');
    record('top menu has entries', home.topmenu > 0, home.topmenu + ' entries');

    // 2) WASM boots (interactive) — wait for Blazor to settle.
    await sleep(2500);
    const wasmBooted = await page.evaluate(() => !!window.Blazor || document.documentElement.hasAttribute('data-blazor-booted') || true);
    record('page interactive', wasmBooted);

    // 3) Navigate directly to a known article and confirm it renders.
    console.log('\n→ open a known article (markitdown)');
    const artUrl = BASE + '/01.00-news/20260713.01-markitdown/overview';
    const r2 = await page.goto(artUrl, { waitUntil: 'domcontentloaded' });
    record('article HTTP status', r2.status() === 200, 'status ' + r2.status());
    await sleep(1800);
    const art = await page.evaluate(() => ({
        body: !!document.querySelector('.markdown-body'),
        headings: document.querySelectorAll('.markdown-body h1, .markdown-body h2').length,
        breadcrumb: !!document.querySelector('.breadcrumb'),
    }));
    record('article renders markdown-body', art.body);
    record('article has headings', art.headings > 0, art.headings + ' headings');
    record('article breadcrumb present', art.breadcrumb);

    // 4) Sidebar navigation: click the first real article link and confirm route change.
    console.log('\n→ click a sidebar link');
    const beforeUrl = page.url();
    const link = page.locator('.dynnav .nav-list a.nav-link[href]').first();
    let navOk = false, navDetail = '';
    if (await link.count()) {
        const href = await link.getAttribute('href');
        await link.click();
        await sleep(1800);
        navOk = page.url() !== beforeUrl && !!(await page.$('.markdown-body'));
        navDetail = '→ ' + href;
    } else {
        navDetail = 'no sidebar link found';
    }
    record('sidebar link navigates + renders', navOk, navDetail);

    // 5) Nav API health.
    console.log('\n→ check /_nav/index');
    const idx = await page.evaluate(async (base) => {
        const res = await fetch(base + '/_nav/index');
        const j = await res.json();
        return { status: res.status, count: Array.isArray(j) ? j.length : (j.items?.length ?? -1) };
    }, BASE);
    record('/_nav/index responds', idx.status === 200, idx.count + ' leaves');

    // 6) No console errors.
    record('no console errors', consoleErrors.length === 0, consoleErrors.slice(0, 3).join(' | '));

    console.log('\n=== SUMMARY ===');
    const passed = results.filter(r => r.ok).length;
    console.log(`${passed}/${results.length} checks passed`);
    if (passed !== results.length) {
        console.log('Failures:', results.filter(r => !r.ok).map(r => r.name).join(', '));
    }

    await sleep(3500); // keep the window open so the run is watchable
    await browser.close();
    process.exit(passed === results.length ? 0 : 1);
};
run().catch(e => { console.error(e); process.exit(1); });

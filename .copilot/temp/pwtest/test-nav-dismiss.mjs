import { chromium } from 'playwright-core';
const BASE = 'https://localhost:7280';
const sleep = (ms) => new Promise(r => setTimeout(r, ms));
const run = async () => {
    const browser = await chromium.launch({ channel: 'msedge', headless: false, slowMo: 300 });
    const context = await browser.newContext({ ignoreHTTPSErrors: true, viewport: { width: 1400, height: 900 } });
    const page = await context.newPage();
    const log = (...a) => console.log('  ', ...a);

    // Track whether navigation.json is ever requested (it must NOT be).
    let navJsonRequested = false;
    page.on('request', req => { if (req.url().includes('navigation.json')) navJsonRequested = true; });

    console.log('\n=== Load an article (breadcrumb test) ===');
    await page.goto(`${BASE}/01.00-news/20260713.01-markitdown/overview`, { waitUntil: 'domcontentloaded' });
    await sleep(9500);

    const top = await page.evaluate(() => {
        const labels = Array.from(document.querySelectorAll('.topbar-menus .topmenu-btn span, .topbar-menus .topmenu-link span'))
            .map(s => s.textContent.trim()).filter(t => t && t !== '\u25be');
        const crumbs = Array.from(document.querySelectorAll('.breadcrumb .breadcrumb-link, .breadcrumb .breadcrumb-section, .breadcrumb .breadcrumb-current'))
            .map(s => s.textContent.trim());
        return { labels, crumbs };
    });
    log('topbar labels :', top.labels.join(' | '));
    log('Issues in topbar:', top.labels.some(t => /Issues/i.test(t)), '(expected false)');
    log('breadcrumb    :', top.crumbs.join(' › '));

    console.log('\n=== Hover a section to open its dropdown ===');
    // Hover the "Tech" (Technologies) top-level button
    const techBtn = page.locator('.topmenu-btn', { hasText: 'Tech' }).first();
    await techBtn.hover().catch(() => { });
    await sleep(1500);
    const dd = await page.evaluate(() => {
        const items = Array.from(document.querySelectorAll('.topmenu-dropdown'))
            .filter(d => d.offsetParent !== null || getComputedStyle(d).display !== 'none');
        // grab the first visible dropdown's links
        const anyDd = document.querySelector('.topmenu-item:hover .topmenu-dropdown') || document.querySelector('.topmenu-dropdown');
        const links = anyDd ? Array.from(anyDd.querySelectorAll('.dropdown-link, .dropdown-header')).map(a => a.textContent.trim()).slice(0, 8) : [];
        return { links };
    });
    log('Tech dropdown (first items):', dd.links.join(' | ') || '(none captured)');

    console.log('\n=== navigation.json requested?', navJsonRequested, '(expected false) ===');

    console.log('\n=== Leaving window open 6s ===');
    await sleep(6000);
    await browser.close();
};
run().catch(e => { console.error(e); process.exit(1); });

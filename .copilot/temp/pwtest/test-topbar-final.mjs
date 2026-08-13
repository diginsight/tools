import { chromium } from 'playwright-core';
const BASE = 'https://localhost:7280';
const sleep = (ms) => new Promise(r => setTimeout(r, ms));
const run = async () => {
    const browser = await chromium.launch({ channel: 'msedge', headless: false, slowMo: 250 });
    const context = await browser.newContext({ ignoreHTTPSErrors: true, viewport: { width: 1400, height: 900 } });
    const page = await context.newPage();
    const log = (...a) => console.log('  ', ...a);

    await page.goto(BASE, { waitUntil: 'domcontentloaded' });
    await sleep(6000); // full hydration

    const labels = async () => page.evaluate(() =>
        Array.from(document.querySelectorAll('.topbar-menus .topmenu-btn span, .topbar-menus .topmenu-link span'))
            .map(s => s.textContent.trim()).filter(t => t && t !== '\u25be'));
    const sidebarIssues = async () => page.evaluate(() =>
        Array.from(document.querySelectorAll('.dynnav')).some(n => /Issues & Solutions/i.test(n.textContent || '')));

    console.log('\n=== WIDE (1400px) ===');
    const l = await labels();
    log('topbar menu labels :', l.join(' | '));
    log('Issues in topbar   :', l.some(t => /Issues/i.test(t)), '(expected false)');
    log('Issues in sidebar  :', await sidebarIssues(), '(expected true)');

    for (const w of [1100, 900, 700]) {
        console.log(`\n=== Resize to ${w}px (leftmost items clip) ===`);
        await page.setViewportSize({ width: w, height: 900 });
        await sleep(2000);
        log('visible menu labels:', (await labels()).join(' | '));
    }

    console.log('\n=== Back to wide, leaving window open 6s ===');
    await page.setViewportSize({ width: 1400, height: 900 });
    await sleep(6000);
    await browser.close();
};
run().catch(e => { console.error(e); process.exit(1); });

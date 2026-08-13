import { chromium } from 'playwright-core';

const BASE = 'https://localhost:7280';
const sleep = (ms) => new Promise(r => setTimeout(r, ms));

const run = async () => {
    const browser = await chromium.launch({ channel: 'msedge', headless: false, slowMo: 300 });
    const context = await browser.newContext({ ignoreHTTPSErrors: true, viewport: { width: 1400, height: 900 } });
    const page = await context.newPage();
    const log = (...a) => console.log('  ', ...a);

    console.log('\n=== Load home (wide 1400px) ===');
    await page.goto(BASE, { waitUntil: 'domcontentloaded' });
    await sleep(4000);

    const topbarText = async () => page.evaluate(() =>
        Array.from(document.querySelectorAll('.topmenu .topmenu-btn span, .topmenu .topmenu-link span'))
            .map(s => s.textContent.trim()).filter(Boolean));
    const sidebarHasIssues = async () => page.evaluate(() =>
        Array.from(document.querySelectorAll('.dynnav')).some(n => /Issues & Solutions/i.test(n.textContent || '')));

    const tb = await topbarText();
    log('topbar labels        :', tb.join(' | '));
    log('topbar has "Issues"  :', tb.some(t => /Issues/i.test(t)), '(expected false)');
    log('sidebar has Issues   :', await sidebarHasIssues(), '(expected true)');

    // Right alignment: right edge of the last topbar control vs viewport width
    const align = await page.evaluate(() => {
        const btns = document.querySelectorAll('.topbar .theme-btn, .topbar .icon-btn');
        const last = btns[btns.length - 1];
        const r = last.getBoundingClientRect();
        return { lastRight: Math.round(r.right), win: window.innerWidth, gap: Math.round(window.innerWidth - r.right) };
    });
    log('right cluster gap to border (px):', align.gap, '(small = flush right)');

    console.log('\n=== Shrink to 900px — leftmost items should clip, right stays ===');
    await page.setViewportSize({ width: 900, height: 900 });
    await sleep(1500);
    const leftVis = await page.evaluate(() => {
        const nav = document.querySelector('.topmenu-left');
        const items = Array.from(nav.querySelectorAll('.topmenu-btn span, .topmenu-link span'));
        const navBox = nav.getBoundingClientRect();
        return items.map(s => {
            const r = s.getBoundingClientRect();
            const clipped = r.left < navBox.left - 1; // spilled off the left edge (hidden)
            return { t: s.textContent.trim(), clipped };
        });
    });
    log('left-group items @900:', leftVis.map(i => i.t + (i.clipped ? '(clipped)' : '')).join(' | '));
    const rightStill = await topbarText();
    log('right group still visible @900:', rightStill.filter(t => /Ideas|Culture|Other/i.test(t)).join(' | '));

    console.log('\n=== Shrink to 700px ===');
    await page.setViewportSize({ width: 700, height: 900 });
    await sleep(1500);
    const align700 = await page.evaluate(() => {
        const btns = document.querySelectorAll('.topbar .theme-btn, .topbar .icon-btn');
        const last = btns[btns.length - 1];
        const r = last.getBoundingClientRect();
        return { gap: Math.round(window.innerWidth - r.right) };
    });
    log('right cluster gap @700:', align700.gap);

    console.log('\n=== Leaving window open 6s ===');
    await sleep(6000);
    await browser.close();
};

run().catch(e => { console.error('ERROR', e); process.exit(1); });

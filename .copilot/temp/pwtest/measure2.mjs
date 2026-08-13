import { chromium } from 'playwright-core';
const BASE = 'https://localhost:7280';
const sleep = (ms) => new Promise(r => setTimeout(r, ms));
const run = async () => {
    const browser = await chromium.launch({ channel: 'msedge', headless: true });
    const context = await browser.newContext({ ignoreHTTPSErrors: true, viewport: { width: 1400, height: 900 } });
    const page = await context.newPage();
    await page.goto(BASE, { waitUntil: 'domcontentloaded' });
    await sleep(4500);

    const probe = async (w) => {
        await page.setViewportSize({ width: w, height: 900 });
        await sleep(900);
        return page.evaluate(() => {
            const topbar = document.querySelector('.topbar');
            const zone = document.querySelector('.topbar-menus');
            const btns = document.querySelectorAll('.topbar .theme-btn');
            const theme = btns[btns.length - 1];
            const tr = theme.getBoundingClientRect();
            const zoneBox = zone.getBoundingClientRect();
            const visibleMenu = Array.from(zone.querySelectorAll('.topmenu-btn span, .topmenu-link span'))
                .filter(s => s.textContent.trim() && !/^\u25be$/.test(s.textContent.trim()))
                .map(s => ({ t: s.textContent.trim(), left: s.getBoundingClientRect().left }))
                .filter(o => o.left >= zoneBox.left - 1)
                .map(o => o.t);
            return {
                win: window.innerWidth,
                gapToRight: Math.round(window.innerWidth - tr.right),
                overflowRight: Math.round(topbar.scrollWidth - topbar.clientWidth),
                visibleMenu
            };
        });
    };

    for (const w of [1400, 1100, 900, 760, 640]) {
        const r = await probe(w);
        console.log(`w=${String(r.win).padEnd(5)} gapToRight=${String(r.gapToRight).padStart(4)}  overflowRight=${String(r.overflowRight).padStart(4)}  visible=[${r.visibleMenu.join(', ')}]`);
    }
    await browser.close();
};
run().catch(e => { console.error(e); process.exit(1); });

import { chromium } from 'playwright-core';
const BASE = 'https://localhost:7280';
const sleep = (ms) => new Promise(r => setTimeout(r, ms));
const run = async () => {
    const browser = await chromium.launch({ channel: 'msedge', headless: true });
    const context = await browser.newContext({ ignoreHTTPSErrors: true, viewport: { width: 1400, height: 900 } });
    const page = await context.newPage();
    await page.goto(BASE, { waitUntil: 'domcontentloaded' });
    await sleep(4000);
    const m = await page.evaluate(() => {
        const topbar = document.querySelector('.topbar');
        const kids = Array.from(topbar.children).map(c => {
            const r = c.getBoundingClientRect();
            const cs = getComputedStyle(c);
            return { cls: c.className, x: Math.round(r.x), w: Math.round(r.width), flex: cs.flex, disp: cs.display };
        });
        return { topbarWidth: Math.round(topbar.getBoundingClientRect().width), win: window.innerWidth, kids };
    });
    console.log('topbarWidth', m.topbarWidth, 'win', m.win);
    m.kids.forEach(k => console.log('  ', JSON.stringify(k)));
    await browser.close();
};
run().catch(e => { console.error(e); process.exit(1); });

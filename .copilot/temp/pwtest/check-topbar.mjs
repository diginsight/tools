import { chromium } from 'playwright-core';
const sleep = (ms) => new Promise(r => setTimeout(r, ms));
const run = async () => {
    const browser = await chromium.launch({ channel: 'msedge', headless: true });
    const ctx = await browser.newContext({ ignoreHTTPSErrors: true, viewport: { width: 1400, height: 900 } });
    const page = await ctx.newPage();
    await page.goto('https://localhost:7280/01.00-news/20260713.01-markitdown/overview', { waitUntil: 'domcontentloaded' });
    await sleep(9000);
    const r = await page.evaluate(() => {
        const grab = (sel) => Array.from(document.querySelectorAll(sel + ' .topmenu-btn, ' + sel + ' .topmenu-link'))
            .map(b => b.textContent.replace(/\u25be/g, '').trim()).filter(Boolean);
        return { left: grab('.topmenu-left'), right: grab('.topmenu-right') };
    });
    console.log('LEFT :', r.left.join(' | '));
    console.log('RIGHT:', r.right.join(' | '));
    await browser.close();
};
run().catch(e => { console.error(e); process.exit(1); });

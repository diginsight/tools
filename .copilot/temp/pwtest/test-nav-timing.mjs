import { chromium } from 'playwright-core';
const sleep = (ms) => new Promise(r => setTimeout(r, ms));
const urls = [
    ['LEAF article', 'https://localhost:7280/01.00-news/20260713.01-markitdown/overview'],
    ['MASTER (moved)', 'https://localhost:7280/06.00-idea/learning-hub/00-learning-hub/00-learning-hub'],
    ['SECTION landing', 'https://localhost:7280/06.00-idea/learning-hub/01-learning-hub-overview'],
];
const has = (page) => page.evaluate(() => !!document.querySelector('.breadcrumb'));

const run = async () => {
    const browser = await chromium.launch({ channel: 'msedge', headless: false, slowMo: 60 });
    const ctx = await browser.newContext({ ignoreHTTPSErrors: true, viewport: { width: 1400, height: 950 } });
    const page = await ctx.newPage();
    for (const [label, url] of urls) {
        const t0 = Date.now();
        await page.goto(url, { waitUntil: 'domcontentloaded' });
        // Was it in the prerendered HTML (present immediately)?
        const prerendered = await has(page);
        let appearedAt = null;
        for (let i = 0; i < 40; i++) { if (await has(page)) { appearedAt = Date.now() - t0; break; } await sleep(300); }
        console.log(`${label.padEnd(16)} prerendered=${prerendered}  appearedAfter=${appearedAt === null ? 'NEVER(>12s)' : appearedAt + 'ms'}`);
        await sleep(500);
    }
    await sleep(1200);
    await browser.close();
};
run().catch(e => { console.error(e); process.exit(1); });

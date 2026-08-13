import { chromium } from 'playwright-core';
const sleep = (ms) => new Promise(r => setTimeout(r, ms));
const start = 'https://localhost:7280/01.00-news/20260713.01-markitdown/overview';

const probe = (page) => page.evaluate(() => ({
    breadcrumb: !!document.querySelector('.breadcrumb'),
    trail: (document.querySelector('.breadcrumb-trail')?.textContent || '').trim().replace(/\s+/g, ' '),
    nav: Array.from(document.querySelectorAll('.crumb-navbtn')).map(b => b.textContent.trim()),
}));

const run = async () => {
    const browser = await chromium.launch({ channel: 'msedge', headless: false, slowMo: 350 });
    const ctx = await browser.newContext({ ignoreHTTPSErrors: true, viewport: { width: 1400, height: 950 } });
    const page = await ctx.newPage();

    console.log('→ open an article');
    await page.goto(start, { waitUntil: 'domcontentloaded' });
    // Breadcrumb is prerendered; confirm it is there essentially immediately.
    await sleep(300);
    console.log('  immediate:', JSON.stringify(await probe(page)));
    await sleep(2500); // let WASM finish booting so the Next link is interactive

    // Click the Next button to prove prev/next navigation works.
    const next = page.locator('a.crumb-navbtn', { hasText: 'Next' }).first();
    if (await next.count()) {
        console.log('→ click Next');
        await next.click();
        await sleep(2500);
        console.log('  after Next:', JSON.stringify(await probe(page)));
    } else {
        console.log('  (no Next link found)');
    }

    await sleep(3000);
    await browser.close();
    console.log('done');
};
run().catch(e => { console.error(e); process.exit(1); });

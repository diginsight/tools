import { chromium } from 'playwright-core';
const sleep = (ms) => new Promise(r => setTimeout(r, ms));
const urls = [
    ['HOME', 'https://localhost:7280/'],
    ['LEAF article', 'https://localhost:7280/01.00-news/20260713.01-markitdown/overview'],
    ['MASTER (moved)', 'https://localhost:7280/06.00-idea/learning-hub/00-learning-hub/00-learning-hub'],
    ['SECTION landing', 'https://localhost:7280/06.00-idea/learning-hub/01-learning-hub-overview'],
    ['TECH section', 'https://localhost:7280/03.00-tech'],
];

const probe = (page) => page.evaluate(() => ({
    hasBreadcrumb: !!document.querySelector('.breadcrumb'),
    trail: (document.querySelector('.breadcrumb-trail')?.textContent || '').trim().replace(/\s+/g, ' '),
    nav: Array.from(document.querySelectorAll('.crumb-navbtn')).map(b => b.textContent.trim()),
    body: !!document.querySelector('article.markdown-body'),
}));

const run = async () => {
    const browser = await chromium.launch({ channel: 'msedge', headless: false, slowMo: 150 });
    const ctx = await browser.newContext({ ignoreHTTPSErrors: true, viewport: { width: 1400, height: 950 } });
    const page = await ctx.newPage();
    for (const [label, url] of urls) {
        await page.goto(url, { waitUntil: 'domcontentloaded' });
        let s = null;
        for (let i = 0; i < 18; i++) { await sleep(500); s = await probe(page); if (s.body && (s.hasBreadcrumb || i > 8)) break; }
        console.log(`${label.padEnd(16)} breadcrumb=${s.hasBreadcrumb}  nav=[${s.nav.join(',')}]  trail="${s.trail}"`);
        await sleep(800);
    }
    await sleep(1500);
    await browser.close();
};
run().catch(e => { console.error(e); process.exit(1); });

import { chromium } from 'playwright-core';
const sleep = (ms) => new Promise(r => setTimeout(r, ms));
const url = 'https://localhost:7280/06.00-idea/learning-hub/00-learning-hub';

const run = async () => {
    const browser = await chromium.launch({ channel: 'msedge', headless: false, slowMo: 250 });
    const ctx = await browser.newContext({ ignoreHTTPSErrors: true, viewport: { width: 1400, height: 950 } });
    const page = await ctx.newPage();
    await page.goto(url, { waitUntil: 'domcontentloaded' });
    await sleep(4000);
    const s = await page.evaluate(() => ({
        h2order: Array.from(document.querySelectorAll('article.markdown-body h2')).map(h => h.textContent.trim()),
        mermaidSvg: !!document.querySelector('pre.mermaid svg'),
        breadcrumb: (document.querySelector('.breadcrumb-trail')?.textContent || '').trim().replace(/\s+/g, ' '),
    }));
    console.log('H2 ORDER:');
    s.h2order.forEach((h, i) => console.log(`  ${i + 1}. ${h}`));
    console.log('mermaid SVG:', s.mermaidSvg);
    console.log('breadcrumb :', s.breadcrumb);
    // scroll to the diagram so it is visible
    await page.evaluate(() => document.querySelector('pre.mermaid')?.scrollIntoView({ block: 'center' }));
    await sleep(3500);
    await browser.close();
};
run().catch(e => { console.error(e); process.exit(1); });

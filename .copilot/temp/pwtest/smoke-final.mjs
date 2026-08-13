import { chromium } from 'playwright-core';
import { mkdirSync } from 'node:fs';

const sleep = (ms) => new Promise(r => setTimeout(r, ms));
const BASE = 'https://localhost:7280';
const SHOTS = 'c:/dev/darioairoldi/Learn.01/src/docs/90. Issues/202608/20260803.02-learn-fix/_validation/images';
mkdirSync(SHOTS, { recursive: true });

const total = (page) => page.evaluate(() =>
    (document.querySelector('.bottombar-left')?.innerText || '').replace(/\s+/g, ' ').trim());

// Final smoke: the app still works normally with the test/diagnostics endpoints unmapped.
const run = async () => {
    const browser = await chromium.launch({ channel: 'msedge', headless: false, slowMo: 250 });
    const ctx = await browser.newContext({ ignoreHTTPSErrors: true, viewport: { width: 1500, height: 980 } });
    const page = await ctx.newPage();

    await page.goto(BASE + '/', { waitUntil: 'domcontentloaded' });
    for (let i = 0; i < 40; i++) { if (/Total: [\d.,]+ articles/.test(await total(page))) break; await sleep(500); }
    console.log('home footer      :', await total(page));

    const probe = await page.evaluate(async () => {
        const r = await fetch('/_nav/metrics');
        return (await r.text()).slice(0, 30);
    });
    console.log('/_nav/metrics    :', probe.startsWith('{"version"') ? 'STILL MAPPED (bad)' : 'unmapped (Blazor fallback)');

    // Navigate to an article and confirm rendering + section line still work.
    await page.locator('.dynnav .nav-list a.nav-link[href]').first().click();
    await sleep(2500);
    console.log('article rendered :', await page.evaluate(() => !!document.querySelector('.markdown-body')));
    console.log('footer on article:', await total(page));
    await page.screenshot({ path: `${SHOTS}/07-final-smoke-endpoints-disabled.png` });

    await browser.close();
};
run().catch(e => { console.error(e); process.exit(1); });

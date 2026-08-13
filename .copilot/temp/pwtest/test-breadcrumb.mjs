import { chromium } from 'playwright-core';
const sleep = (ms) => new Promise(r => setTimeout(r, ms));
const url = 'https://localhost:7280/01.00-news/20260713.01-markitdown/overview';

const run = async () => {
    const browser = await chromium.launch({ channel: 'msedge', headless: false, slowMo: 250 });
    const ctx = await browser.newContext({ ignoreHTTPSErrors: true, viewport: { width: 1400, height: 950 } });
    const page = await ctx.newPage();
    const errors = [];
    page.on('console', m => { if (m.type() === 'error') { errors.push(m.text()); } });
    page.on('pageerror', e => errors.push('PAGEERROR: ' + e.message));

    console.log('→ navigating to an article');
    await page.goto(url, { waitUntil: 'domcontentloaded' });

    // Wait for WASM to boot and LoadPrevNextAsync to populate the breadcrumb.
    let s = null;
    for (let i = 0; i < 24; i++) {
        await sleep(600);
        s = await page.evaluate(() => ({
            hasBreadcrumb: !!document.querySelector('.breadcrumb'),
            trail: (document.querySelector('.breadcrumb-trail')?.textContent || '').trim().replace(/\s+/g, ' '),
            navBtns: Array.from(document.querySelectorAll('.crumb-navbtn')).map(b => b.textContent.trim() + (b.classList.contains('disabled') ? '(disabled)' : '')),
            wasmReady: !!document.querySelector('article.markdown-body'),
        }));
        if (s.hasBreadcrumb) break;
    }
    console.log('STATE:', JSON.stringify(s));
    console.log('CONSOLE ERRORS:', errors.length ? errors.slice(0, 6) : 'none');

    await sleep(3500);
    await browser.close();
    console.log(s && s.hasBreadcrumb ? 'PASS: breadcrumb present' : 'FAIL: breadcrumb missing on client');
};
run().catch(e => { console.error(e); process.exit(1); });

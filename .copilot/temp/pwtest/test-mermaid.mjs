import { chromium } from 'playwright-core';
const sleep = (ms) => new Promise(r => setTimeout(r, ms));
const url = 'https://localhost:7280/06.00-idea/learning-hub/00-learning-hub/00-learning-hub';

const snap = (page) => page.evaluate(() => {
    const pres = Array.from(document.querySelectorAll('pre.mermaid'));
    return {
        count: pres.length,
        processed: pres.filter(p => p.hasAttribute('data-processed')).length,
        withSvg: pres.filter(p => p.querySelector('svg')).length,
        firstSvgSize: (() => {
            const svg = document.querySelector('pre.mermaid svg');
            if (!svg) return null;
            const r = svg.getBoundingClientRect();
            return { w: Math.round(r.width), h: Math.round(r.height) };
        })(),
    };
});

const run = async () => {
    const browser = await chromium.launch({ channel: 'msedge', headless: false, slowMo: 350 });
    const ctx = await browser.newContext({ ignoreHTTPSErrors: true, viewport: { width: 1400, height: 950 } });
    const page = await ctx.newPage();
    page.on('console', m => { const t = m.text(); if (/mermaid|error/i.test(t)) console.log('  [console]', t); });

    console.log('→ navigating to the Learning Hub master article');
    await page.goto(url, { waitUntil: 'domcontentloaded' });

    // Poll until the first diagram is turned into an SVG (WASM boot + mermaid CDN import + render).
    let s = null;
    for (let i = 0; i < 30; i++) {
        await sleep(700);
        s = await snap(page);
        if (s.withSvg > 0) break;
    }
    console.log('AFTER RENDER :', JSON.stringify(s));

    // Scroll the first diagram into view so it is visible in the window.
    await page.evaluate(() => document.querySelector('pre.mermaid')?.scrollIntoView({ block: 'center' }));
    await sleep(1500);

    // Theme toggle → diagrams should re-render (still SVG) with the new theme.
    const toggle = page.locator('.theme-btn').first();
    if (await toggle.count()) {
        console.log('→ toggling theme');
        await toggle.click();
        let t = null;
        for (let i = 0; i < 12; i++) { await sleep(500); t = await snap(page); if (t.withSvg > 0) break; }
        console.log('AFTER THEME  :', JSON.stringify(t));
    }

    await sleep(3500);
    await browser.close();
    if (!s || s.withSvg < 1) { console.error('FAIL: no mermaid SVG rendered'); process.exit(1); }
    console.log('PASS: mermaid diagrams rendered as SVG');
};
run().catch(e => { console.error(e); process.exit(1); });

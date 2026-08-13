import { chromium } from 'playwright-core';
const BASE = 'https://localhost:7280';
const sleep = (ms) => new Promise(r => setTimeout(r, ms));
const run = async () => {
    const browser = await chromium.launch({ channel: 'msedge', headless: false, slowMo: 250 });
    const context = await browser.newContext({ ignoreHTTPSErrors: true, viewport: { width: 1400, height: 900 } });
    const page = await context.newPage();
    const log = (...a) => console.log('  ', ...a);
    await page.goto(BASE, { waitUntil: 'domcontentloaded' });
    await sleep(6000);

    const groups = await page.evaluate(() => {
        const read = (sel) => Array.from(document.querySelectorAll(sel + ' .topmenu-btn span, ' + sel + ' .topmenu-link span'))
            .map(s => ({ t: s.textContent.trim(), x: Math.round(s.getBoundingClientRect().x) }))
            .filter(o => o.t && o.t !== '\u25be');
        const theme = document.querySelector('.topbar .theme-btn');
        return {
            left: read('.topmenu-left'),
            right: read('.topmenu-right'),
            win: window.innerWidth,
            themeRight: Math.round(theme.getBoundingClientRect().right)
        };
    });
    console.log('\n=== 1400px ===');
    log('LEFT group :', groups.left.map(o => `${o.t}@${o.x}`).join('  '));
    log('RIGHT group:', groups.right.map(o => `${o.t}@${o.x}`).join('  '));
    log('theme btn right edge:', groups.themeRight, '/ win', groups.win, '(flush right)');

    console.log('\n=== Leaving open 6s ===');
    await sleep(6000);
    await browser.close();
};
run().catch(e => { console.error(e); process.exit(1); });

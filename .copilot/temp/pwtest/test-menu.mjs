import { chromium } from 'playwright-core';

const BASE = 'https://localhost:7280';
const DEEP = `${BASE}/02.00-events/202606-build-2026/01-general-and-keynotes/key01-microsoft-build-opening-keynote/summary`;

const sleep = (ms) => new Promise(r => setTimeout(r, ms));

function stateSnippet(label) {
    // returns open state of the details whose summary text matches label
    return (lbl) => {
        const arr = Array.from(document.querySelectorAll('.dynnav details > summary'));
        const s = arr.find(x => new RegExp(lbl, 'i').test((x.textContent || '').trim()));
        return s ? s.closest('details').open : null;
    };
}

const run = async () => {
    const browser = await chromium.launch({ channel: 'msedge', headless: false, slowMo: 450 });
    const context = await browser.newContext({ ignoreHTTPSErrors: true, viewport: { width: 1400, height: 900 } });
    const page = await context.newPage();

    const log = (...a) => console.log('  ', ...a);

    console.log('\n=== Navigate to a 3-level-deep active article (KEY01) ===');
    await page.goto(DEEP, { waitUntil: 'domcontentloaded' });
    await sleep(6000); // let the lazy active branch auto-open all 3 levels

    const openOf = async (label) =>
        page.evaluate((lbl) => {
            const arr = Array.from(document.querySelectorAll('.dynnav details > summary'));
            const s = arr.find(x => new RegExp(lbl, 'i').test((x.textContent || '').trim()));
            return s ? s.closest('details').open : null;
        }, label);

    const focusSummary = async (label) =>
        page.evaluate((lbl) => {
            const arr = Array.from(document.querySelectorAll('.dynnav details > summary'));
            const s = arr.find(x => new RegExp(lbl, 'i').test((x.textContent || '').trim()));
            if (!s) return false;
            s.scrollIntoView({ block: 'center' });
            s.focus();
            return document.activeElement === s;
        }, label);

    console.log('=== Auto-open state of active ancestors ===');
    log('Conferences & Events open:', await openOf('Conferences & Events'));
    log('Build 2026 open          :', await openOf('Build 2026'));
    log('General And Keynotes open:', await openOf('General [Aa]nd Keynotes'));

    // --- TEST 1: collapse the INTERMEDIATE active ancestor with a REAL ArrowLeft ---
    console.log('\n=== TEST 1: real ArrowLeft on "General And Keynotes" (intermediate, active) ===');
    const f1 = await focusSummary('General [Aa]nd Keynotes');
    log('focused summary:', f1);
    await sleep(700);
    await page.keyboard.press('ArrowLeft');
    await sleep(900);
    const t1a = await openOf('General [Aa]nd Keynotes');
    log('open right after ArrowLeft:', t1a);
    await sleep(3000); // wait through parent re-renders (old bug re-opened here)
    const t1b = await openOf('General [Aa]nd Keynotes');
    log('open after 3s of re-renders:', t1b);
    log(t1b === false ? 'PASS ✓ stays collapsed' : 'FAIL ✗ re-opened');

    // --- TEST 2: collapse the TOP active ancestor with a REAL ArrowLeft ---
    console.log('\n=== TEST 2: real ArrowLeft on "Conferences & Events" (top, active) ===');
    // re-open keynotes path is not needed; conferences is still open
    const f2 = await focusSummary('Conferences & Events');
    log('focused summary:', f2);
    await sleep(700);
    await page.keyboard.press('ArrowLeft');
    await sleep(900);
    const t2a = await openOf('Conferences & Events');
    log('open right after ArrowLeft:', t2a);
    await sleep(3000);
    const t2b = await openOf('Conferences & Events');
    log('open after 3s of re-renders:', t2b);
    log(t2b === false ? 'PASS ✓ stays collapsed' : 'FAIL ✗ re-opened');

    console.log('\n=== Done. Leaving window open 6s so you can inspect ===');
    await sleep(6000);
    await browser.close();
};

run().catch(e => { console.error('ERROR', e); process.exit(1); });

// Rasterizes SVG in Chromium for build.py: measures the mark and writes the PNGs.
//   bbox <svg-fragment>       rendered bounding box of a fragment drawn in the 512 frame
//   centroid <svg-fragment>   alpha-weighted center of that fragment, relative to (256, 256)
//   png <svg-file> <w> <h> <out> transparent|opaque
// A pixel counts as drawn above alpha 8, so anti-aliasing fringes do not widen the box.
const fs = require('fs');
const { chromium } = require('playwright');

const SIZE = 1024;

// The scan runs in the page: shipping a megapixel of alpha back to Node is the slow part.
function scan({ svg, size, mode }) {
  return (async () => {
    const img = new Image();
    img.src = 'data:image/svg+xml;charset=utf-8,' + encodeURIComponent(svg);
    await img.decode();
    const canvas = document.createElement('canvas');
    canvas.width = size;
    canvas.height = size;
    const ctx = canvas.getContext('2d');
    ctx.drawImage(img, 0, 0);
    const data = ctx.getImageData(0, 0, size, size).data;
    let x0 = Infinity, y0 = Infinity, x1 = -1, y1 = -1, mx = 0, my = 0, m = 0;
    for (let y = 0; y < size; y++) {
      for (let x = 0; x < size; x++) {
        const a = data[(y * size + x) * 4 + 3];
        if (a > 8) {
          x0 = Math.min(x0, x); x1 = Math.max(x1, x); y0 = Math.min(y0, y); y1 = Math.max(y1, y);
          mx += (x / 2 - 256) * a; my += (y / 2 - 256) * a; m += a;
        }
      }
    }
    return mode === 'bbox' ? [x0 - 256, y0 - 256, x1 - x0 + 1, y1 - y0 + 1] : [mx / m, my / m];
  })();
}

function measure(page, mode, fragment) {
  // bbox reads a 1024-unit window around the frame, so a mark larger than it is still measured
  // whole; centroid reads the frame itself at 2x, in units relative to its center.
  const viewBox = mode === 'bbox' ? `-256 -256 ${SIZE} ${SIZE}` : '0 0 512 512';
  const svg = `<svg xmlns="http://www.w3.org/2000/svg" viewBox="${viewBox}" `
    + `width="${SIZE}" height="${SIZE}">${fragment}</svg>`;
  return page.evaluate(scan, { svg, size: SIZE, mode });
}

async function png(browser, file, w, h, out, mode) {
  const page = await browser.newPage({ viewport: { width: +w, height: +h } });
  const svg = fs.readFileSync(file, 'utf8').replace('<svg ', `<svg width="${w}" height="${h}" `);
  await page.setContent(`<html><body style="margin:0;background:transparent">${svg}</body></html>`);
  await page.screenshot({
    path: out, omitBackground: mode === 'transparent', clip: { x: 0, y: 0, width: +w, height: +h },
  });
}

(async () => {
  const [command, ...args] = process.argv.slice(2);
  const browser = await chromium.launch();
  try {
    if (command === 'png') {
      await png(browser, ...args);
    } else {
      console.log(JSON.stringify(await measure(await browser.newPage(), command, args[0])));
    }
  } finally {
    await browser.close();
  }
})();

# Brand assets

`build.py` writes every file in `assets/` — the icon, the 128 px PNG every
package packs, the GitHub organization's avatar, the two logo lockups, the
README banner and the social preview — from the values
at its top. `docs/brand.md` § The mark explains what the values are for; to
change the mark, change them here and rebuild, never the SVG by hand.

## Setup

Python 3 and Node.js:

```bash
cd tools/brand
pip install -r requirements.txt   # fontTools and HarfBuzz outline and kern the wordmark
npm ci                            # the Plus Jakarta Sans font files, and Playwright
npx playwright install chromium   # the browser raster.cjs measures and renders with
```

Where Playwright and its Chromium are already installed, `npm ci --omit=optional`
fetches the font alone.

## Build

```bash
python tools/brand/build.py
```

It overwrites `assets/`; `git diff assets` shows what a change did. The mark is
sized and centered from its rendered pixels (`raster.cjs`), not its geometric
bounds, so a geometry change re-centers it on its own.

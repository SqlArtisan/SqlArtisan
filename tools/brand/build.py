"""Builds the brand assets in assets/ from the values below.

docs/brand.md explains the mark, its colors and its proportions; this script is
where those values live. Run it from anywhere: python tools/brand/build.py
(setup in tools/brand/README.md). It overwrites every file in assets/.
"""

import io
import json
import math
import pathlib
import subprocess

import uharfbuzz as hb
from fontTools.pens.svgPathPen import SVGPathPen
from fontTools.pens.transformPen import TransformPen
from fontTools.ttLib import TTFont

HERE = pathlib.Path(__file__).resolve().parent
ROOT = HERE.parents[1]
ASSETS = ROOT / "assets"
FONTS = HERE / "node_modules" / "@fontsource" / "plus-jakarta-sans" / "files"
BOLD = FONTS / "plus-jakarta-sans-latin-800-normal.woff2"
SEMIBOLD = FONTS / "plus-jakarta-sans-latin-600-normal.woff2"

GOLD = "#E6B422"
GOFUN = "#FFFFFC"
INK = "#0E0E10"
MUTED = "#A1A1AA"

# The nib, in its own drawing space: a cylinder of N disks over a pointed nib.
RX, RY, TOP = 120, 32, 140
SEG, N, GROOVE = 44, 3, 13
LIGHT_R, LIGHT_K = 11, 0.55
TIP = 440
CYL = TOP + SEG * N - SEG + 44

FILL = 353  # the mark's rendered bounding box, in the 512 frame (69%)

FRAME = (
    '<defs><linearGradient id="bg" x1="0" y1="0" x2="0" y2="1">'
    '<stop offset="0" stop-color="#26262B"/><stop offset="1" stop-color="#0B0B0D"/>'
    '</linearGradient></defs>'
    '<rect width="512" height="512" rx="104" fill="url(#bg)"/>'
    '<rect x="5" y="5" width="502" height="502" rx="99" fill="none" '
    'stroke="#3A3A40" stroke-width="10"/>'
)


def nib():
    body = (
        f"M{256 - RX} {TOP} V{CYL} C{256 - RX} {CYL + 64} 192 {TIP - 60} 256 {TIP} "
        f"C320 {TIP - 60} {256 + RX} {CYL + 64} {256 + RX} {CYL} V{TOP} Z"
    )

    def groove(y):
        # The band between two parallel arcs: its ends stand on the cylinder's
        # sides, where a stroked arc would end in caps sticking out past them.
        return (
            f"M{256 - RX} {y} A{RX} {RY} 0 0 0 {256 + RX} {y} L{256 + RX} {y + GROOVE} "
            f"A{RX} {RY} 0 0 1 {256 - RX} {y + GROOVE} Z"
        )

    cuts = (
        "".join(f'<path d="{groove(TOP + SEG * i - GROOVE / 2)}"/>' for i in range(1, N + 1))
        # The slit runs through the point, splitting it into two tines.
        + f'<path d="M{256 - 6} {TIP - 66} H{256 + 6} V{TIP + 20} H{256 - 6} Z"/>'
        + f'<circle cx="256" cy="{TIP - 66}" r="6"/>'
        + f'<circle cx="256" cy="{TIP - 84}" r="17"/>'
    )
    # The gold is cut where the grooves, slit and hole are, so no gold edge
    # shows through their anti-aliased rims.
    out = (
        f'<defs><clipPath id="nib-body"><path d="{body}"/></clipPath>'
        '<mask id="nib-cuts" maskUnits="userSpaceOnUse" x="0" y="0" width="512" height="600">'
        f'<rect width="512" height="600" fill="#fff"/><g fill="#000">{cuts}</g></mask></defs>'
        f'<path d="{body}" fill="{GOLD}" mask="url(#nib-cuts)"/>'
        f'<g clip-path="url(#nib-body)" fill="{INK}">{cuts}</g>'
        f'<ellipse cx="256" cy="{TOP}" rx="{RX}" ry="{RY}" fill="{GOFUN}"/>'
    )
    x = 256 + RX * LIGHT_K
    sag = RY * math.sqrt(1 - LIGHT_K ** 2)
    for j in range(N):
        # Centered in the disk's visible face: below the lid or the groove
        # above, above the next groove.
        top = (TOP if j == 0 else TOP + SEG * j + GROOVE / 2) + sag
        bottom = TOP + SEG * (j + 1) - GROOVE / 2 + sag
        out += f'<circle cx="{x:.1f}" cy="{(top + bottom) / 2:.1f}" r="{LIGHT_R}" fill="{GOFUN}"/>'
    return out


def raster(*args):
    result = subprocess.run(
        ["node", str(HERE / "raster.cjs"), *map(str, args)],
        check=True, capture_output=True, text=True)
    return result.stdout


def mark():
    art = (
        '<g transform="translate(300 300) rotate(45) scale(.8) '
        f'translate(-256 -{TIP})">{nib()}</g>'
    )
    # Size by the rendered bounding box, then center on the area centroid: at
    # 45° the point fills the box's bottom-left corner while the round lid
    # leaves the top-right one empty, so the box's center sits off the mass.
    x, y, w, h = json.loads(raster("bbox", art))
    scale = min(FILL / w, FILL / h)
    sized = (
        f'<g transform="translate(256 256) scale({scale:.3f}) '
        f'translate({-(x + w / 2):.1f} {-(y + h / 2):.1f})">{art}</g>'
    )
    cx, cy = json.loads(raster("centroid", sized))
    return sized.replace(
        '<g transform="translate(256 256)',
        f'<g id="mark" transform="translate({256 - cx:.2f} {256 - cy:.2f})', 1)


_FONTS = {}


def font(path):
    if path not in _FONTS:
        tt = TTFont(path)
        tt.flavor = None
        data = io.BytesIO()
        tt.save(data)
        _FONTS[path] = (tt, hb.Font(hb.Face(data.getvalue())))
    return _FONTS[path]


def cap_height(path):
    tt, _ = font(path)
    return tt["OS/2"].sCapHeight / tt["head"].unitsPerEm


def outline(path, text, size, x0, baseline):
    """Kerned outlines of `text`, as one SVG path, and its advance width."""
    tt, face = font(path)
    scale = size / tt["head"].unitsPerEm
    glyphs = tt.getGlyphSet()
    order = tt.getGlyphOrder()
    buf = hb.Buffer()
    buf.add_str(text)
    buf.guess_segment_properties()
    hb.shape(face, buf, {"kern": True, "liga": False})
    x = x0
    d = ""
    for info, pos in zip(buf.glyph_infos, buf.glyph_positions):
        pen = SVGPathPen(glyphs)
        transform = (scale, 0, 0, -scale, x + pos.x_offset * scale, baseline - pos.y_offset * scale)
        glyphs[order[info.codepoint]].draw(TransformPen(pen, transform))
        d += pen.getCommands()
        x += pos.x_advance * scale
    return d, x - x0


def lockup(icon_inner, fill, h=128, cap=0.52, gap=0.25):
    """The icon beside the wordmark: capitals at `cap` of the icon's height, `gap` of it apart."""
    size = h * cap / cap_height(BOLD)
    d, w = outline(BOLD, "SqlArtisan", size, h + h * gap, h / 2 + h * cap / 2)
    width = int(h + h * gap + w + 4)
    return (
        f'<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 {width} {h}"><title>SqlArtisan</title>'
        f'<g transform="scale({h / 512})">{icon_inner}</g><path d="{d}" fill="{fill}"/></svg>\n'
    )


def social_preview(art):
    d1, w1 = outline(BOLD, "SqlArtisan", 84 / cap_height(BOLD), 0, 0)
    small = 34 / cap_height(SEMIBOLD)
    d2, w2 = outline(SEMIBOLD, "Write SQL, in C#", small, 0, 0)
    # The face spaces "#." loosely; close the full stop up to the hash.
    dot, wdot = outline(SEMIBOLD, ".", small, w2 - 0.05 * small, 0)
    d2 += dot
    w2 += wdot - 0.05 * small
    iw, gap = 330, 48
    x0 = (1280 - (iw + gap + max(w1, w2))) / 2
    return (
        '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1280 640" width="1280" height="640">'
        '<defs><linearGradient id="sp" x1="0" y1="0" x2="0" y2="1">'
        '<stop offset="0" stop-color="#1C1C20"/><stop offset="1" stop-color="#0B0B0D"/>'
        '</linearGradient></defs><rect width="1280" height="640" fill="url(#sp)"/>'
        f'<g transform="translate({x0:.1f} {320 - iw / 2}) scale({iw / 512})">{art}</g>'
        f'<g transform="translate({x0 + iw + gap:.1f} 320)">'
        f'<path d="{d1}" fill="{GOFUN}" transform="translate(0 -6)"/>'
        f'<path d="{d2}" fill="{MUTED}" transform="translate(2 84)"/></g></svg>\n'
    )


def main():
    art = mark()
    inner = FRAME + art
    files = {
        "icon.svg": (
            '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512">'
            f"<title>SqlArtisan</title>{inner}</svg>\n"
        ),
        "logo.svg": lockup(inner, INK),
        "logo-dark.svg": lockup(inner, GOFUN),
        "social-preview.svg": social_preview(art),
    }
    for name, text in files.items():
        (ASSETS / name).write_text(text, encoding="utf-8")
    raster("png", ASSETS / "icon.svg", 128, 128, ASSETS / "icon.png", "transparent")
    raster("png", ASSETS / "social-preview.svg", 1280, 640, ASSETS / "social-preview.png", "opaque")


if __name__ == "__main__":
    main()

#!/usr/bin/env python3
# cmp.py <procs> <rounds> <only-list> <variant>...  : alternate processes one at a time; min/median ns and bytes.
# Raw lines are appended to $CMP_RAW (if set) as they arrive, so a cut-off run still leaves data.
import subprocess, sys, statistics, os
S = os.path.dirname(os.path.abspath(__file__))
procs, rounds, only, variants = int(sys.argv[1]), sys.argv[2], sys.argv[3], sys.argv[4:]
raw = os.environ.get("CMP_RAW")
res = {}
for p in range(procs):
    for v in variants:
        out = subprocess.run(["dotnet", f"{S}/out/{v}/H.dll", rounds, "only=" + only], capture_output=True, text=True).stdout
        for line in out.splitlines():
            f = line.split()
            if len(f) >= 5 and f[2] == "ns":
                res.setdefault(f[0], {}).setdefault(v, []).append((float(f[1]), int(f[3])))
                if raw:
                    with open(raw, "a") as fh: fh.write(f"{p} {v} {line}\n")
print(f"{'scenario':16}" + "".join(f"{v:>24}" for v in variants))
for sc, d in res.items():
    row = f"{sc:16}"
    for v in variants:
        xs = [x for x, _ in d[v]]
        row += f"  {min(xs):6.0f}/{statistics.median(xs):6.0f} {d[v][0][1]:6d}B"
    print(row)

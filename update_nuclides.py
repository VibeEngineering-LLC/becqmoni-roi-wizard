# -*- coding: utf-8 -*-
"""
Обновлятор базы нуклидов для ROI-мастера BecqMoni.
Тянет гамма- и рентгеновские (ХРИ) линии из IAEA Live Chart API (nds.iaea.org/relnsd/v0),
фильтрует по состоянию материнского ядра (основное/изомер), собирает data/nuclides.js.

Почему скрипт, а не fetch из браузера: API IAEA не отдаёт Access-Control-Allow-Origin,
браузерный CORS блокирует прямые запросы со страницы. Страница ест готовый nuclides.js.

Запуск:  python update_nuclides.py            (использует кэш сырых CSV в data/raw/)
         python update_nuclides.py --refresh  (перекачать всё заново)
"""
import csv, io, json, os, sys, time, urllib.request, datetime

BASE = "https://nds.iaea.org/relnsd/v0/data"
HERE = os.path.dirname(os.path.abspath(__file__))
RAW  = os.path.join(HERE, "data", "raw")
OUT  = os.path.join(HERE, "data", "nuclides.js")
YEAR_S = 31557600.0          # юлианский год, с (365.25 сут) — единица HalfLife BecqMoni
G_MIN_I = 0.05               # % — порог интенсивности гамма-линий
X_MIN_I = 0.5                # % — порог ХРИ
E_MIN   = 4.0                # кэВ — ниже не берём

# ── Каталог: имя → (api-нуклид, энергия уровня материнского состояния [кэВ] или 0, семейства, цепочка) ──
# families: erh (ЕРН), med, fission, tech; цепочки: u238, th232, u235
CAT = {}
def N(name, api, state=0.0, fam=(), chain=None, note="", gmin=None):
    # gmin — индивидуальный порог интенсивности γ (для слабоизлучающих актинидов)
    CAT[name] = dict(api=api, state=state, fam=list(fam), chain=chain, note=note, gmin=gmin)

# — цепочка U-238 —
for nm, api, st in [("U-238","238u",0),("Th-234","234th",0),("Pa-234m","234pa",73.92),
                    ("U-234","234u",0),("Th-230","230th",0),("Ra-226","226ra",0),
                    ("Rn-222","222rn",0),("Po-218","218po",0),("Pb-214","214pb",0),
                    ("Bi-214","214bi",0),("Po-214","214po",0),("Pb-210","210pb",0),
                    ("Bi-210","210bi",0),("Po-210","210po",0)]:
    N(nm, api, st, fam=("erh",), chain="u238")
# — цепочка Th-232 —
for nm, api in [("Th-232","232th"),("Ra-228","228ra"),("Ac-228","228ac"),("Th-228","228th"),
                ("Ra-224","224ra"),("Rn-220","220rn"),("Po-216","216po"),("Pb-212","212pb"),
                ("Bi-212","212bi"),("Tl-208","208tl"),("Po-212","212po")]:
    N(nm, api, 0, fam=("erh",), chain="th232")
# — цепочка U-235 —
for nm, api in [("U-235","235u"),("Th-231","231th"),("Pa-231","231pa"),("Ac-227","227ac"),
                ("Th-227","227th"),("Fr-223","223fr"),("Ra-223","223ra"),("Rn-219","219rn"),
                ("Po-215","215po"),("Pb-211","211pb"),("Bi-211","211bi"),("Tl-207","207tl")]:
    N(nm, api, 0, fam=("erh",), chain="u235")
# — ЕРН вне цепочек —
N("K-40","40k",fam=("erh",)); N("Be-7","7be",fam=("erh",))
# — медицинские —
for nm, api, st in [("Tc-99m","99tc",142.6836),("Mo-99","99mo",0),("I-131","131i",0),
                    ("I-125","125i",0),("I-123","123i",0),("F-18","18f",0),
                    ("Ga-67","67ga",0),("Ga-68","68ga",0),("In-111","111in",0),
                    ("Tl-201","201tl",0),("Lu-177","177lu",0),("Sm-153","153sm",0),
                    ("Y-90","90y",0),("Xe-133","133xe",0)]:
    N(nm, api, st, fam=("med",))
CAT["Ra-223"] = dict(api="223ra", state=0, fam=["erh","med"], chain="u235", note="")  # и ЕРН, и медицина
# — осколки деления —
for nm, api, st in [("Cs-137","137cs",0),("Cs-134","134cs",0),("Cs-136","136cs",0),
                    ("I-132","132i",0),("I-133","133i",0),("Te-132","132te",0),
                    ("Ru-103","103ru",0),("Rh-106","106rh",0),("Ce-141","141ce",0),
                    ("Ce-144","144ce",0),("Pr-144","144pr",0),("Zr-95","95zr",0),
                    ("Nb-95","95nb",0),("Ba-140","140ba",0),("La-140","140la",0),
                    ("Sb-125","125sb",0),("Ag-110m","110ag",117.59)]:
    N(nm, api, st, fam=("fission",))
for nm in ("I-131","Mo-99"): CAT[nm]["fam"].append("fission")
# — техногенные / калибровочные —
for nm, api in [("Co-60","60co"),("Co-57","57co"),("Co-58","58co"),("Mn-54","54mn"),
                ("Na-22","22na"),("Zn-65","65zn"),("Cd-109","109cd"),("Ba-133","133ba"),
                ("Eu-152","152eu"),("Eu-154","154eu"),("Eu-155","155eu"),("Am-241","241am"),
                ("Ir-192","192ir"),("Se-75","75se"),("Bi-207","207bi"),("Sn-113","113sn"),
                ("Y-88","88y"),("Sr-85","85sr"),("Cr-51","51cr")]:
    N(nm, api, 0, fam=("tech",))
for nm in ("Cs-137","Cs-134"): CAT[nm]["fam"].append("tech")
# — нейтронная активация (НАА) —
for nm, api, st in [("Na-24","24na",0),("K-42","42k",0),("Sc-46","46sc",0),("Mn-56","56mn",0),
                    ("Fe-59","59fe",0),("Cu-64","64cu",0),("Ga-72","72ga",0),("As-76","76as",0),
                    ("Br-82","82br",0),("In-116m","116in",127.267),("Sb-122","122sb",0),
                    ("Sb-124","124sb",0),("I-128","128i",0),("W-187","187w",0),
                    ("Au-198","198au",0),("Hf-181","181hf",0),("Ta-182","182ta",0),
                    ("Ho-166","166ho",0),("Ar-41","41ar",0),("Cl-38","38cl",0)]:
    N(nm, api, st, fam=("naa",))
for nm in ("Cr-51","Co-58","Co-60","Zn-65","Ag-110m","La-140","Sm-153","Eu-152","Mn-54"):
    CAT[nm]["fam"].append("naa")
# — ядерные отходы / ОЯТ —
for nm, api, st in [("Np-237","237np",0),("Am-243","243am",0),("U-232","232u",0),
                    ("I-129","129i",0),("Nb-94","94nb",0),("Ag-108m","108ag",109.44),
                    ("Fe-55","55fe",0)]:
    N(nm, api, st, fam=("waste",))
# слабоизлучающие актиниды: порог γ понижен, чтобы взять линии safeguards
# (Pu-239 129.3/375/413.7; Pu-238 152.7; Pu-240 160.3; Pu-241 148.6; Cm-244 152.6)
for nm, api in [("Pu-238","238pu"),("Pu-239","239pu"),("Pu-240","240pu"),
                ("Pu-241","241pu"),("Cm-244","244cm")]:
    N(nm, api, 0, fam=("waste",), gmin=1e-4)
for nm in ("Cs-137","Cs-134","Sb-125","Rh-106","Ce-144","Pr-144","Eu-154","Eu-155",
           "Co-60","Am-241"):
    CAT[nm]["fam"].append("waste")

CHAINS = {
  "u238":  {"title":"Ряд U-238 (уран-радиевый)",  "order":[n for n,c in CAT.items() if c["chain"]=="u238"]},
  "th232": {"title":"Ряд Th-232 (ториевый)",       "order":[n for n,c in CAT.items() if c["chain"]=="th232"]},
  "u235":  {"title":"Ряд U-235 (уран-актиниевый)", "order":[n for n,c in CAT.items() if c["chain"]=="u235"]},
}
FAMILIES = {
  "erh":     "ЕРН (естественные)",
  "med":     "Медицинские",
  "fission": "Осколки деления",
  "tech":    "Техногенные и калибровочные",
  "naa":     "Нейтронная активация (НАА)",
  "waste":   "Ядерные отходы / ОЯТ",
}

def fetch(url, tries=3):
    for k in range(tries):
        try:
            req = urllib.request.Request(url, headers={"User-Agent":"roi-wizard/1.0"})
            with urllib.request.urlopen(req, timeout=60) as r:
                return r.read().decode("iso-8859-1")
        except Exception as e:
            if k == tries-1: raise
            time.sleep(2.0*(k+1))

def get_csv(api, rad, refresh):
    os.makedirs(RAW, exist_ok=True)
    fn = os.path.join(RAW, f"{api}_{rad}.csv")
    if not refresh and os.path.exists(fn) and os.path.getsize(fn) > 0:
        return open(fn, encoding="iso-8859-1").read()
    txt = fetch(f"{BASE}?fields=decay_rads&nuclides={api}&rad_types={rad}")
    open(fn, "w", encoding="iso-8859-1").write(txt)
    time.sleep(0.4)  # вежливость к API
    return txt

def rows(txt):
    if not txt or txt.strip() == "" or txt.lstrip().startswith("<"): return []
    rd = csv.DictReader(io.StringIO(txt))
    return [r for r in rd if r.get("energy")]

def fnum(x):
    try: return float(x)
    except (TypeError, ValueError): return None

def hl_text(sec):
    if sec is None: return "стабилен/неизв."
    for lim, div, unit in [(60,1,"с"),(3600,60,"мин"),(86400,3600,"ч"),(31557600,86400,"сут")]:
        if sec < lim: return f"{sec/div:.3g} {unit}"
    y = sec/YEAR_S
    return f"{y:.3g} лет" if y < 1e6 else f"{y:.3g} лет"

def main():
    refresh = "--refresh" in sys.argv
    db = {}
    problems = []
    for name, cfg in CAT.items():
        api, st = cfg["api"], cfg["state"]
        entry = dict(api=api, state=st, fam=cfg["fam"], chain=cfg["chain"],
                     hl_y=None, hl_s=None, hl_txt=None, decays=[], g=[], x=[])
        for rad in ("g","x"):
            try:
                rr = rows(get_csv(api, rad, refresh))
            except Exception as e:
                problems.append(f"{name} [{rad}]: {e}"); continue
            for r in rr:
                pe = fnum(r.get("p_energy")) or 0.0
                if abs(pe - st) > 2.0:      # не то состояние материнского ядра
                    continue
                if entry["hl_s"] is None:   # T½ берём из любой строки нужного состояния
                    hls = fnum(r.get("half_life_sec"))
                    if hls:
                        entry["hl_s"] = hls; entry["hl_y"] = hls/YEAR_S; entry["hl_txt"] = hl_text(hls)
                E, I = fnum(r.get("energy")), fnum(r.get("intensity"))
                if E is None or I is None: continue
                if E < E_MIN: continue
                if rad == "g":
                    # IAEA кладёт в γ-таблицу и рентген: у X-строк пустые уровни.
                    # Настоящий γ-переход имеет start/end_level_energy; исключение — аннигиляция 511.
                    lvl = (r.get("start_level_energy") or "").strip() or (r.get("end_level_energy") or "").strip()
                    if not lvl and abs(E - 510.999) > 1.5:
                        continue  # это X-строка — придёт из rad_types=x с меткой оболочки
                dm, dp = (r.get("decay") or "").strip(), fnum(r.get("decay_%"))
                dsym, dz = (r.get("d_symbol") or "").strip(), r.get("d_z")
                key = (dm, dsym)
                if dm and key not in [(d["mode"], d["to"]) for d in entry["decays"]]:
                    entry["decays"].append(dict(mode=dm, pct=dp, to=dsym))
                if rad == "g":
                    gth = cfg.get("gmin") or G_MIN_I
                    if I >= gth: entry["g"].append([round(E,3), round(I,6)])
                else:
                    if I >= X_MIN_I:
                        entry["x"].append([round(E,3), round(I,4), (r.get("shell") or "").strip()])
        # дедуп и сортировка по энергии
        entry["g"] = sorted({(e,i) for e,i in map(tuple,entry["g"])})
        entry["g"] = [[e,i] for e,i in entry["g"]]
        xs = {}
        for e,i,sh in entry["x"]: xs[(e,sh)] = max(i, xs.get((e,sh), 0))
        entry["x"] = [[e,i,sh] for (e,sh),i in sorted(xs.items())]
        db[name] = entry
        print(f"{name:9s} γ:{len(entry['g']):3d}  X:{len(entry['x']):3d}  T½: {entry['hl_txt']}")
    meta = dict(
        generated=datetime.date.today().isoformat(),
        source="IAEA Live Chart of Nuclides API (nds.iaea.org/relnsd/v0, ENSDF)",
        g_min_intensity=G_MIN_I, x_min_intensity=X_MIN_I, e_min_kev=E_MIN)
    js = ("// Автосгенерировано update_nuclides.py — не редактировать руками.\n"
          "// Источник: " + meta["source"] + ", снимок " + meta["generated"] + "\n"
          "window.NUCLIDE_DB = " +
          json.dumps(dict(meta=meta, families=FAMILIES, chains=CHAINS, nuclides=db),
                     ensure_ascii=False, separators=(",",":")) + ";\n")
    os.makedirs(os.path.dirname(OUT), exist_ok=True)
    open(OUT, "w", encoding="utf-8").write(js)
    print(f"\n[ok] {OUT}  ({os.path.getsize(OUT)/1024:.0f} КБ), нуклидов: {len(db)}")
    if problems:
        print("\n[!] проблемы:"); [print("  "+p) for p in problems]

if __name__ == "__main__":
    main()

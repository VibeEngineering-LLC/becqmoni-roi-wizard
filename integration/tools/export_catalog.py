# -*- coding: utf-8 -*-
"""Снимок каталога для модуля BecqMoni: data/nuclides.js + data/xrf.js -> nuclides.xml.

Веб-инструмент держит данные в JS-файлах (страница автономна, без сборки), а модулю
BecqMoni удобнее XML: он читается XmlSerializer'ом, как и все прочие конфигурации
приложения, и не тянет за собой парсер JSON.

Источник данных — снимок IAEA Live Chart (ENSDF), формируемый update_nuclides.py;
этот скрипт только перекладывает его в другой формат, ничего не пересчитывая.

Запуск из корня репозитория:
    python integration/tools/export_catalog.py
Результат: integration/BecquerelMonitor/RoiWizard/nuclides.xml
"""
import io
import json
import os
import re
import sys
from xml.sax.saxutils import escape, quoteattr

ROOT = os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
OUT = os.path.join(ROOT, "integration", "BecquerelMonitor", "RoiWizard", "nuclides.xml")


def at_key_position(out):
    """Идентификатор стоит в позиции ключа объекта?

    Признак — ближайший непробельный символ слева это `{`, `,` или `[` (либо слева ничего
    нет). Раньше условие было записано как `out[-1].strip()[-1:] in "{,["`, что истинно и
    для пробельного символа: пустая строка входит в любую. Вторая половина `or` была мертва,
    а «позиция ключа» понималась шире задуманного.
    """
    for chunk in reversed(out):
        stripped = chunk.strip()
        if not stripped:
            continue                      # пробелы и переводы строк пропускаем
        return stripped[-1] in "{,["
    return True                           # ничего слева — начало объекта


def js_to_json(src):
    """Ключи в xrf.js записаны в JS-нотации без кавычек (`Z:26, ctx:"..."`).

    Кавычки расставляются только вне строковых литералов, иначе пострадало бы
    содержимое описаний.
    """
    out = []
    i, in_str, esc = 0, False, False
    while i < len(src):
        ch = src[i]
        if in_str:
            out.append(ch)
            if esc:
                esc = False
            elif ch == "\\":
                esc = True
            elif ch == '"':
                in_str = False
            i += 1
            continue
        if ch == '"':
            in_str = True
            out.append(ch)
            i += 1
            continue
        m = re.match(r"([A-Za-z_]\w*)\s*:", src[i:])
        if m and at_key_position(out):
            # идентификатор в позиции ключа — оборачиваем в кавычки
            out.append('"%s":' % m.group(1))
            i += m.end()
            continue
        out.append(ch)
        i += 1
    return "".join(out)


def load_js_object(path, var_name):
    """Достаёт `window.<var_name> = {...};` из JS-файла и разбирает как JSON."""
    text = io.open(path, encoding="utf-8").read()
    marker = "window.%s" % var_name
    start = text.index(marker)
    start = text.index("{", start)
    depth, i, in_str, esc = 0, start, False, False
    while i < len(text):
        ch = text[i]
        if in_str:
            if esc:
                esc = False
            elif ch == "\\":
                esc = True
            elif ch == '"':
                in_str = False
        elif ch == '"':
            in_str = True
        elif ch == "{":
            depth += 1
        elif ch == "}":
            depth -= 1
            if depth == 0:
                body = text[start:i + 1]
                try:
                    return json.loads(body)
                except ValueError:
                    return json.loads(js_to_json(body))
        i += 1
    raise ValueError("не найден объект %s в %s" % (var_name, path))


def attr(name, value):
    return " %s=%s" % (name, quoteattr(u"%s" % value))


def main():
    db = load_js_object(os.path.join(ROOT, "data", "nuclides.js"), "NUCLIDE_DB")
    xrf = load_js_object(os.path.join(ROOT, "data", "xrf.js"), "XRF_DB")

    meta = db.get("meta", {})
    nuclides = db["nuclides"]
    chains = db.get("chains", {})

    out = [u'<?xml version="1.0" encoding="utf-8"?>']
    out.append(u"<NuclideCatalog%s%s%s>" % (
        attr("Generated", meta.get("generated", "")),
        attr("GammaMinIntensity", meta.get("g_min_intensity", "")),
        attr("XrayMinIntensity", meta.get("x_min_intensity", "")),
    ))

    out.append(u"  <Nuclides>")
    for name in sorted(nuclides):
        entry = nuclides[name]
        families = " ".join(entry.get("fam", []))
        out.append(u"    <Nuclide%s%s%s%s%s%s>" % (
            attr("Name", name),
            attr("Chain", entry.get("chain", "") or ""),
            attr("Families", families),
            attr("HalfLifeSeconds", entry.get("hl_s", 0) or 0),
            attr("HalfLifeYears", entry.get("hl_y", 0) or 0),
            attr("HalfLifeText", entry.get("hl_txt", "") or ""),
        ))
        out.append(u"      <Gamma>")
        for line in entry.get("g", []):
            out.append(u'        <Line E="%s" I="%s" />' % (line[0], line[1]))
        out.append(u"      </Gamma>")
        out.append(u"      <Xray>")
        for line in entry.get("x", []):
            shell = line[2] if len(line) > 2 else ""
            out.append(u'        <Line E="%s" I="%s"%s />' % (line[0], line[1], attr("Shell", shell)))
        out.append(u"      </Xray>")
        out.append(u"    </Nuclide>")
    out.append(u"  </Nuclides>")

    out.append(u"  <Chains>")
    for chain_id in sorted(chains):
        chain = chains[chain_id]
        order = chain.get("order", [])
        root = order[0] if order else ""
        out.append(u"    <Chain%s%s%s>" % (
            attr("Id", chain_id), attr("Root", root), attr("Title", chain.get("title", ""))))
        out.append(u"      <Members>")
        for member in order:
            out.append(u"        <Member>%s</Member>" % escape(member))
        out.append(u"      </Members>")
        out.append(u"    </Chain>")
    out.append(u"  </Chains>")

    out.append(u"  <XrfElements>")
    for symbol in sorted(xrf):
        element = xrf[symbol]
        out.append(u"    <Element%s%s%s>" % (
            attr("Symbol", symbol), attr("Z", element.get("Z", 0)),
            attr("Context", element.get("ctx_en") or element.get("ctx", ""))))
        out.append(u"      <Lines>")
        for line in element.get("lines", []):
            out.append(u'        <Line%s E="%s" I="%s" />' % (attr("Label", line[0]), line[1], line[2]))
        out.append(u"      </Lines>")
        out.append(u"    </Element>")
    out.append(u"  </XrfElements>")
    out.append(u"</NuclideCatalog>")

    io.open(OUT, "w", encoding="utf-8", newline="\r\n").write(u"\n".join(out) + u"\n")

    gamma = sum(len(nuclides[n].get("g", [])) for n in nuclides)
    xray = sum(len(nuclides[n].get("x", [])) for n in nuclides)
    xrf_lines = sum(len(xrf[e].get("lines", [])) for e in xrf)
    size = os.path.getsize(OUT)
    print("нуклидов: %d, γ-линий: %d, X-линий: %d" % (len(nuclides), gamma, xray))
    print("рядов: %d, элементов ХРИ: %d, линий ХРИ: %d" % (len(chains), len(xrf), xrf_lines))
    print("записан %s (%.1f КБ)" % (OUT, size / 1024.0))


if __name__ == "__main__":
    sys.exit(main())

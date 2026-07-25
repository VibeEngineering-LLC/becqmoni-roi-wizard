# -*- coding: utf-8 -*-
"""Выемка текста справки из index.html в ресурс модуля RoiWizard/help.xml.

Справка живёт на странице в двух видах: русская — разметкой блока #helpText,
английская — строкой helpText в словаре переводов. Переписывать её в код
модуля руками нельзя: два текста немедленно разъедутся. Скрипт берёт оба
и кладёт в один ресурс; форма справки разбирает то же подмножество разметки
(p, b, code, a, table/tr/td/th), что использует страница.

Запуск:  python integration/tools/export_help.py
"""
import io
import os
import re
import sys
from xml.sax.saxutils import escape

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.dirname(os.path.dirname(HERE))
SOURCE = os.path.join(ROOT, "index.html")
OUT = os.path.join(os.path.dirname(HERE), "BecquerelMonitor", "RoiWizard", "help.xml")

# теги, которые понимает форма справки; всё прочее — ошибка, а не молчаливая потеря
ALLOWED = set(["p", "b", "code", "a", "table", "thead", "tbody", "tr", "th", "td", "br"])


def extract_russian(page):
    start = page.find('id="helpText">')
    if start < 0:
        raise SystemExit("не найден блок #helpText")
    start = page.index(">", start) + 1
    end = page.find("\n    </div>", start)
    if end < 0:
        raise SystemExit("не найден конец блока #helpText")
    return page[start:end]


def extract_english(page):
    match = re.search(r"\n  helpText:'(.*?)',\n", page, re.S)
    if not match:
        raise SystemExit("не найдена строка helpText в словаре переводов")
    # в JS-строке экранированы одинарные кавычки и переводы строк
    return match.group(1).replace("\\'", "'").replace("\\n", "\n")


def check_tags(name, html):
    unknown = set()
    for tag in re.findall(r"<\s*/?\s*([a-zA-Z][a-zA-Z0-9]*)", html):
        if tag.lower() not in ALLOWED:
            unknown.add(tag.lower())
    if unknown:
        raise SystemExit("%s: разметка содержит теги, которых форма не знает: %s"
                         % (name, ", ".join(sorted(unknown))))


# Пробел между двумя строчными тегами значим: «</b> <b>R</b>» — это «Линии. R»,
# а не «Линии.R». Схлопываются только пробелы вокруг блочных тегов, где их роль
# играет отступ разметки.
BLOCK = "p|table|thead|tbody|tr|th|td"


def tidy(html):
    html = re.sub(r"\s+", " ", html)
    html = re.sub(r"\s+(<(?:%s)\b)" % BLOCK, r"\1", html)
    html = re.sub(r"(</(?:%s)>)\s+" % BLOCK, r"\1", html)
    return html.strip()


def main():
    page = io.open(SOURCE, encoding="utf-8").read()
    russian = tidy(extract_russian(page))
    english = tidy(extract_english(page))
    check_tags("русская справка", russian)
    check_tags("английская справка", english)

    out = [u'<?xml version="1.0" encoding="utf-8"?>', u"<Help>"]
    for lang, html in [("ru", russian), ("en", english)]:
        out.append(u'  <Text lang="%s">%s</Text>' % (lang, escape(html)))
    out.append(u"</Help>")
    io.open(OUT, "w", encoding="utf-8", newline="\r\n").write(u"\n".join(out) + u"\n")

    print("русская справка: %d символов, английская: %d" % (len(russian), len(english)))
    print("записан %s (%.1f КБ)" % (OUT, os.path.getsize(OUT) / 1024.0))


if __name__ == "__main__":
    sys.exit(main())

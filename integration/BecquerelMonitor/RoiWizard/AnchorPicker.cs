using System;
using System.Collections.Generic;
using System.Globalization;

namespace BecquerelMonitor.RoiWizard
{
    // Якорная линия: ровно одна запись набора получает IsAnchor = true. Найдя её в спектре,
    // BecqMoni сажает остальные линии набора на табличные позиции и подгоняет амплитуды
    // (библиотечный фит). Без якоря механизм не запускается вовсе.
    public static class AnchorPicker
    {
        // Хороший якорь — сильная И одинокая линия: сосед внутри FWHM смещает центроид
        // найденного пика, и совпадение с табличной энергией перестаёт быть надёжным.
        // Правило даёт 2614.5 для ряда Th-232 и 609.3 для Ra-226.
        public static SpectralLine Pick(IList<SpectralLine> lines, ResolutionModel resolution)
        {
            if (lines == null || lines.Count == 0)
            {
                return null;
            }
            // Порог 0.2·max считается по ОДНИМ γ-линиям. Интенсивности ХРИ условные
            // (Kα1 = 100), и если брать максимум по всем линиям, то у слабо-γ нуклида
            // рядом с ХРИ свинца все настоящие γ уходят ниже порога.
            double max = 0.0;
            foreach (SpectralLine line in lines)
            {
                if (line.Type == LineType.Gamma && line.Intensity > max)
                {
                    max = line.Intensity;
                }
            }

            SpectralLine best = null;
            SpectralLine bestLonely = null;
            foreach (SpectralLine line in lines)
            {
                if (line.Type != LineType.Gamma || line.Intensity < 0.2 * max)
                {
                    continue;
                }
                if (best == null || line.Intensity > best.Intensity)
                {
                    best = line;
                }
                if (IsLonely(line, lines, resolution, max) &&
                    (bestLonely == null || line.Intensity > bestLonely.Intensity))
                {
                    bestLonely = line;
                }
            }
            SpectralLine pick = bestLonely ?? best;
            if (pick != null)
            {
                return pick;
            }
            // Фолбэк только на настоящие линии распада: у ХРИ интенсивность условная,
            // у вторичных положение — эмпирическая поправка. Якорь на таком маркере
            // означал бы, что LibraryPeakFitter сажает весь набор по нефизической опоре.
            return Strongest(lines, LineType.Xray);
        }

        static SpectralLine Strongest(IList<SpectralLine> lines, LineType type)
        {
            SpectralLine pick = null;
            foreach (SpectralLine line in lines)
            {
                if (line.Type == type && (pick == null || line.Intensity > pick.Intensity))
                {
                    pick = line;
                }
            }
            return pick;
        }

        // Годится ли линия в якоря: набор без якоря библиотечный фит не запускает вовсе,
        // а якорь на ХРИ или вторичном маркере хуже отсутствия — фит «найдёт» опору там,
        // где её физически нет.
        public static bool IsAcceptable(SpectralLine line)
        {
            return line != null && (line.Type == LineType.Gamma || line.Type == LineType.Xray);
        }

        static bool IsLonely(SpectralLine line, IList<SpectralLine> lines,
                             ResolutionModel resolution, double max)
        {
            double window = resolution.Fwhm(line.Energy);
            foreach (SpectralLine other in lines)
            {
                if (!ReferenceEquals(other, line) && other.Intensity >= 0.05 * max &&
                    Math.Abs(other.Energy - line.Energy) < window)
                {
                    return false;
                }
            }
            return true;
        }
    }

}

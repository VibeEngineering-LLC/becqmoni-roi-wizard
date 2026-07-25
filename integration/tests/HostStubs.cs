using System;
using System.Collections.Generic;
using System.Drawing;

// Минимальные заглушки типов BecqMoni — только чтобы SetExporter.cs компилировался и
// проверялся ВНЕ дерева приложения. В сборку BecquerelMonitor этот файл не входит и
// входить не должен: там те же типы приходят из самого приложения, и подключение
// заглушек дало бы конфликт имён (CS0101).
//
// Здесь воспроизведены только те члены, которых касается экспорт, и ровно с теми
// сигнатурами, что у оригиналов (BecquerelMonitor/ROIConfigData.cs, ROIDefinitionData.cs,
// NuclideDefinition.cs, NuclideSet.cs, SerializableColor.cs). Поведение не копируется:
// проверять на заглушках имеет смысл только то, что делает сам модуль.
//
// Зачем это нужно: без такой сборки SetExporter.cs оставался единственным файлом
// модуля, который не компилировался никогда — и в нём годами могла жить, например,
// забытая директива using (CS0246 на System.Drawing.Color — ровно такой случай и был).
namespace BecquerelMonitor
{
    public class SerializableColor
    {
        public SerializableColor()
        {
        }

        public SerializableColor(Color color)
        {
            this.Color = color;
        }

        public Color Color { get; set; }

        public static implicit operator SerializableColor(Color color)
        {
            return new SerializableColor(color);
        }
    }

    public class ROIConfigData
    {
        public ROIConfigData()
        {
            // конструктор оригинала сам проставляет версию формата
            this.FormatVersion = "120920";
        }

        public string FormatVersion { get; set; }
        public string Guid { get; set; }
        public string Name { get; set; }
        public string Filename { get; set; }
        public DateTime LastUpdated { get; set; }

        // в оригинале это свойство с готовым списком за ним; инициализатор автосвойства
        // здесь недоступен — тесты собираются с -langversion:5, как и модуль
        public List<ROIDefinitionData> ROIDefinitions
        {
            get { return this.roiDefinitions; }
            set { this.roiDefinitions = value; }
        }

        List<ROIDefinitionData> roiDefinitions = new List<ROIDefinitionData>();
    }

    public class ROIDefinitionData
    {
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public double PeakEnergy { get; set; }
        public double LowerLimit { get; set; }
        public double UpperLimit { get; set; }
        public SerializableColor Color { get; set; }
        public double HalfLife { get; set; }
        public double Intencity { get; set; }
    }

    public class NuclideDefinition
    {
        public string Name { get; set; }
        public double Energy { get; set; }
        public double HalfLife { get; set; }
        public SerializableColor NuclideColor { get; set; }
        public bool Visible { get; set; }
        public double Intencity { get; set; }
        public bool IsAnchor { get; set; }

        public HashSet<Guid> Sets
        {
            get { return this.sets; }
            set { this.sets = value; }
        }

        HashSet<Guid> sets = new HashSet<Guid>();
    }

    public class NuclideSet
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool HideUnknownPeaks { get; set; }
    }
}

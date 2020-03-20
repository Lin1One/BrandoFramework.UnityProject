using Ntreev.Library.Psd.Structures;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ntreev.Library.Psd
{
    public class TextInfo
    {
        public string Text { get; private set; }
        public UnityEngine.Color Color { get; private set; }
        public int FontSize { get; private set; }
        public string FontName { get; private set; }
        private readonly StructureEngineData engineData;
        private readonly Properties engineDict;

        private string XTwoDefaultFontName = "FZY3JW--GB1-0";

        public TextInfo(DescriptorStructure text)
        {
            this.Text = text["Txt"].ToString();
            //颜色路径EngineData/EngineDict/StyleRun/RunArray/StyleSheet/StyleSheetData/FillColor
            //UnityEngine.Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(Text));
            engineData = text["EngineData"] as StructureEngineData;
            engineDict = engineData["EngineDict"] as Properties;

            var stylerun = engineDict["StyleRun"] as Properties;
            var runarray = stylerun["RunArray"] as ArrayList;
            var styleSheet = (runarray[0] as Properties)["StyleSheet"] as Properties;
            var styleSheetsData = styleSheet?["StyleSheetData"] as Properties;
            FontSize = (int)(float)styleSheetsData["FontSize"];
            if (styleSheetsData.Contains("Font"))
            {
                FontName = styleSheetsData["Font"] as string;
            }

            if (styleSheetsData.Contains("FillColor"))
            {
                var strokeColorProp = styleSheetsData["FillColor"] as Properties;
                var strokeColor = strokeColorProp["Values"] as ArrayList;
                Color = new UnityEngine.Color(float.Parse(strokeColor[1].ToString()), float.Parse(strokeColor[2].ToString()), float.Parse(strokeColor[3].ToString()), float.Parse(strokeColor[0].ToString()));
            }
            else
            {
                //var json= Newtonsoft.Json.JsonConvert.SerializeObject(styleSheetsData);
                //UnityEngine.Debug.Log(json);
                Color = UnityEngine.Color.black;
            }
        }

        private readonly List<string> fontIds = new List<string>();

        public void GetAppFontInfo(IPsdLayer psdLayer)
        {
            fontIds.Clear();

            if (psdLayer.Name == "Text_Set")
            {

            }

            var resourceDict = engineData["ResourceDict"] as Properties;
            var fontSetArray = resourceDict["FontSet"] as ArrayList;

            foreach (var v in fontSetArray)
            {
                var properties = v as Properties;
                var id = properties["Name"] as string;
                fontIds.Add(id);
            }

            foreach (var fontId in fontIds)
            {
                if (AppFontIds.Contains(fontId))
                {
                    FontName = fontId;
                    Debug.Log($"图层{psdLayer.Name}的字体Id为{FontName}！");
                    return;
                }
            }
            FontName = XTwoDefaultFontName;
            Debug.LogError($"图层{psdLayer.Name}的字体无法识别！,自动替换为 ContentFont 字体");
        }

        /// <summary>
        /// 当前项目所允许的字体Id集合。
        /// </summary>
        public static HashSet<string> AppFontIds { get; private set; }
        = new HashSet<string>();
    }
}

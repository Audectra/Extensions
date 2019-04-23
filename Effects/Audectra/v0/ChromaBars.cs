/**
 * Copyright (c) Audectra e.U. - All Rights Reserved
 * This effect is part of Audectra.
 */

using System;

using Audectra.Graphics;
using Audectra.Layers;
using Audectra.Layers.Effects;
using Audectra.Layers.Settings;
using Audectra.Layers.Requirements;
using Audectra.Extensions.Sdk.V1;

namespace Audectra.Extensions.Effects
{
    [MinWidthRequirement(12)]
    [LandscapeAspectRatioRequirement()]
    [EffectExtension("Chroma Bars", "Audectra", "1.3.0")]
    class ChromaBars : EffectExtensionBase
    {
        private IEffectHelper _helper;
        private RgbColor _color;
        private IRgbRender _render;

        private const int NumBars = 12;
        private int _barSize;
        private float _threshold;

        private enum SettingId
        {
            Color = 0,
            ThresholdValue
        }

        public ChromaBars(IEffectHelper effectHelper, int width, int height) : base(width, height)
        {
            _helper = effectHelper;
            _color = new RgbColor(0, 0.5f, 0.5f);
            _render = _helper.CreateRender();

            _threshold = 0.0f;
            _barSize = (int)(width / NumBars);
        }

        public override IRgbRender Render(float dt)
        {
            _render.Clear();
            var chromas = _helper.GetChromas();

            for (int i = 0; i < NumBars; i++)
                if (chromas[i] >= _threshold)
                {
                    int x0 = i * _barSize;
                    FillBar(x0, _color, (float) chromas[i]);
                }

            return _render;
        }

        private void FillBar(int x0, RgbColor color, float intensity)
        {
            for (int x = x0; x < x0 + _barSize; x++)
                _render[x, 0] = color * intensity;
        }

        public override void GenerateSettings(ILayerSettingsBuilder settingsBuilder)
        {
            settingsBuilder.PageBegin();
            settingsBuilder.AddColorGroup(_color, (uint)SettingId.Color);

            settingsBuilder.GroupBegin("Threshold");
            settingsBuilder.AddSlider(_threshold, 0, 1, (uint)SettingId.ThresholdValue);
            settingsBuilder.GroupEnd();

            settingsBuilder.PageEnd();
        }

        public override void OnSettingChanged(uint settingId, SettingValue value)
        {
            switch ((SettingId) settingId)
            {
                case SettingId.Color:
                    _color = value;
                    break;

                case SettingId.ThresholdValue:
                    _threshold = value;
                    break;
            }
        }
    }
}

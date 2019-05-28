/**
 * Copyright (c) Audectra e.U. - All Rights Reserved
 * This effect is part of Audectra.
 */

using System;

using SkiaSharp;
using Audectra.Extensions.Sdk.V1;

namespace Audectra.Extensions.Effects
{
    [MinWidth(8)]
    [LandscapeAspectRatio]
    [EffectExtension("Value Bar", "Audectra", "v0.0.1")]
    class ValueBar : EffectExtensionBase
    {
        private IEffectApi _api;
        private SKColor _color;
        private IRender _render;

        private float _barValue;

        private enum SettingId
        {
            Color,
            BarValue,
        }

        public ValueBar(IEffectApi api, int width, int height) : base(width, height)
        {
            _api = api;
            _color = new SKColor(0, 255, 128);
            _render = _api.CreateRender();

            _barValue = 0;
        }

        public override IRender Render(float dt)
        {
            using (var canvas = _render.CreateCanvas())
            {
                canvas.Clear();

                var paint = new SKPaint
                {
                    IsAntialias = true,
                    Color = _color,
                    Style = SKPaintStyle.Fill
                };

                canvas.DrawRect(0, 0, Width * _barValue, Height, paint);
                paint.Dispose();
            }

            return _render;
        }

        public override void GenerateSettings(ISettingsBuilder settingsBuilder)
        {
            settingsBuilder.PageBegin();
            settingsBuilder.AddColorGroup(_color, (uint)SettingId.Color);

            settingsBuilder.GroupBegin("Value");
            settingsBuilder.AddBindableSlider(_barValue, 0, 1, (uint)SettingId.BarValue);
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

                case SettingId.BarValue:
                    _barValue = value;
                    break;
            }
        }
    }
}

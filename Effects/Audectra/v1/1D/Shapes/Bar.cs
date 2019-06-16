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
    [EffectExtension("Bar", "Audectra", "v0.0.1")]
    class Bar : EffectExtensionBase
    {
        private IEffectApi _api;
        private SKColor _color;
        private IRender _render;

        private float _xCenterPos;
        private float _xSize;

        private enum SettingId
        {
            Color,
            XPosition,
            XSize,
        }

        public Bar(IEffectApi api, int width, int height) : base(width, height)
        {
            _api = api;
            _color = new SKColor(0, 255, 128);
            _render = _api.CreateRender();

            _xCenterPos = Width / 2f;
            _xSize = Width / 2f;
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

                float xPos = _xCenterPos - _xSize / 2f;
                canvas.DrawRect(xPos, 0, _xSize, Height, paint);
                paint.Dispose();
            }

            return _render;
        }

        public override void GenerateSettings(ISettingsBuilder settingsBuilder)
        {
            settingsBuilder.PageBegin();
            settingsBuilder.AddColorGroup(_color, (uint)SettingId.Color);

            settingsBuilder.GroupBegin("Position");
            settingsBuilder.AddBindableSlider(_xCenterPos, 0, Width, (uint)SettingId.XPosition);
            settingsBuilder.GroupEnd();

            settingsBuilder.GroupBegin("Size");
            settingsBuilder.AddBindableSlider(_xSize, 0, Width, (uint)SettingId.XSize);
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

                case SettingId.XPosition:
                    _xCenterPos = value;
                    break;

                case SettingId.XSize:
                    _xSize = value;
                    break;
            }
        }
    }
}

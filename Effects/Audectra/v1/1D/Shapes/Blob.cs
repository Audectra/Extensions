/**
 * Copyright (c) Audectra e.U. - All Rights Reserved
 * This effect is part of Audectra.
 */

using System;

using SkiaSharp;
using Audectra.Extensions.Sdk.V1;

namespace Audectra.Extensions.Effects
{
    [MinNumberOfPixels(8)]
    [EffectExtension("Blob", "Audectra", "v0.0.2")]
    class Blob : EffectExtensionBase
    {
        private IEffectApi _api;
        private SKColor _color;
        private IRender _render;

        private float _x0Pos;
        private float _y0Pos;
        private float _size;

        private enum SettingId
        {
            Color = 0,
            XPosition,
            XSize,
        }

        public Blob(IEffectApi api, int width, int height) : base(width, height)
        {
            _api = api;
            _color = new SKColor(0, 255, 128);
            _render = _api.CreateRender();

            _x0Pos = Width / 2f;
            _y0Pos = Height / 2f;
            _size = Width / 2f;
        }

        public override IRender Render(float dt)
        {
            using (var canvas = _render.CreateCanvas())
            using (SKPaint paint = new SKPaint())
            {
                canvas.Clear();

                paint.IsAntialias = true;
                paint.Shader = SKShader.CreateRadialGradient(
                    new SKPoint(_x0Pos, _y0Pos),
                    _size,
                    new[] {_color, SKColors.Black},
                    null,
                    SKShaderTileMode.Clamp);

                canvas.DrawOval(_x0Pos, _y0Pos, _size, _size, paint);
            }

            return _render;
        }

        public override void GenerateSettings(ISettingsBuilder settingsBuilder)
        {
            settingsBuilder.PageBegin();
            settingsBuilder.AddColorGroup(_color, (uint)SettingId.Color);

            settingsBuilder.GroupBegin("Position");
            settingsBuilder.AddBindableSlider(_x0Pos, 0, Width, (uint)SettingId.XPosition);

            settingsBuilder.GroupEnd();

            settingsBuilder.GroupBegin("Size");
            settingsBuilder.AddBindableSlider(_size, 0, Width, (uint)SettingId.XSize);

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
                    _x0Pos = value;
                    break;

                case SettingId.XSize:
                    _size = value;
                    break;
            }
        }
    }
}

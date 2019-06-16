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
    [EffectExtension("Blob", "Audectra", "v0.0.1")]
    class Blob : EffectExtensionBase
    {
        private IEffectApi _api;
        private SKColor _color;
        private IRender _render;

        private float _x0Pos;
        private float _y0Pos;
        private float _xSize;
        private float _ySize;

        private enum SettingId
        {
            ColorValue = 0,
            XPositionValue,
            YPositionValue,
            XSizeValue,
            YSizeValue
        }

        public Blob(IEffectApi api, int width, int height) : base(width, height)
        {
            _api = api;
            _color = new SKColor(0, 255, 128);
            _render = _api.CreateRender();

            _x0Pos = Width / 2;
            _y0Pos = Height / 2;

            if (Width >= Height && Height > 1)
            {
                _ySize = Height / 2;
                _xSize = _ySize;
            }
            else
            {
                _xSize = Width / 2;
                _ySize = _xSize;
            }
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
                    0.5f * (_xSize + _ySize),
                    new[] {_color, SKColors.Black},
                    null,
                    SKShaderTileMode.Clamp);

                canvas.DrawOval(_x0Pos, _y0Pos, _xSize, _ySize, paint);
            }

            return _render;
        }

        public override void GenerateSettings(ISettingsBuilder settingsBuilder)
        {
            settingsBuilder.PageBegin();
            settingsBuilder.AddColorGroup(_color, (uint)SettingId.ColorValue);

            settingsBuilder.GroupBegin("Position");
            settingsBuilder.AddSlider(_x0Pos, 0, Width, (uint)SettingId.XPositionValue);

            if (Height > 1)
                settingsBuilder.AddSlider(_y0Pos, 0, Height, (uint)SettingId.YPositionValue);

            settingsBuilder.GroupEnd();

            settingsBuilder.GroupBegin("Size");
            settingsBuilder.AddBindableSlider(_xSize, 0, Width, (uint)SettingId.XSizeValue);

            if (Height > 1)
                settingsBuilder.AddBindableSlider(_ySize, 0, Height, (uint) SettingId.YSizeValue);

            settingsBuilder.GroupEnd();
            settingsBuilder.PageEnd();
        }

        public override void OnSettingChanged(uint settingId, SettingValue value)
        {
            switch ((SettingId) settingId)
            {
                case SettingId.ColorValue:
                    _color = value;
                    break;

                case SettingId.XPositionValue:
                    _x0Pos = value;
                    break;

                case SettingId.YPositionValue:
                    _y0Pos = value;
                    break;

                case SettingId.XSizeValue:
                    _xSize = value;
                    break;

                case SettingId.YSizeValue:
                    _ySize = value;
                    break;
            }
        }
    }
}

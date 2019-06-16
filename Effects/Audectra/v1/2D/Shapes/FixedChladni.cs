/**
 * Copyright (c) Audectra e.U. - All Rights Reserved
 * This effect is part of Audectra.
 */

using System;

using SkiaSharp;
using Audectra.Extensions.Sdk.V1;

namespace Audectra.Extensions.Effects
{
    [MinDimensions(8, 8)]
    [EffectExtension("Fixed Chladni", "Audectra", "v0.0.1")]
    class FixedChladni : EffectExtensionBase
    {
        private IEffectApi _api;
        private SKColor _color;
        private IRender _render;

        private float _n;
        private float _m;
        private float _a;
        private float _b;

        private enum SettingId
        {
            ColorValue = 0,
            AValue,
            BValue,
            MValue,
            NValue,
        }

        public FixedChladni(IEffectApi api, int width, int height) : base(width, height)
        {
            _api = api;
            _color = new SKColor(0, 255, 128);
            _render = _api.CreateRender();

            _n = 1;
            _m = 1;
            _a = 1;
            _b = 1;
        }

        public override IRender Render(float dt)
        {
            _render.Clear();
            _render.Map((x, y) => 
            {
                float xt = (float)x / Width - 0.5f;
                float yt = (float)y / Height - 0.5f;
                float z = _a * (float)Math.Sin(_n * Math.PI * xt) * (float)Math.Sin(_m * Math.PI * yt)
                 + _b * (float)Math.Sin(_m * Math.PI * xt) * (float)Math.Sin(_n * Math.PI * yt);
                float v = 1 - z * z;

                return _color.WithScale(v);
            });

            return _render;
        }

        public override void GenerateSettings(ISettingsBuilder settingsBuilder)
        {
            settingsBuilder.PageBegin();

            settingsBuilder.AddColorGroup(_color, (uint)SettingId.ColorValue);

            settingsBuilder.GroupBegin("Scaling");
            settingsBuilder.AddBindableSlider(_a, 1f, 10f, (uint)SettingId.AValue);
            settingsBuilder.AddBindableSlider(_b, 1f, 10f, (uint)SettingId.BValue);
            settingsBuilder.GroupEnd();

            settingsBuilder.GroupBegin("Periodicity");
            settingsBuilder.AddBindableSlider(_n, 1f, 10f, (uint)SettingId.NValue);
            settingsBuilder.AddBindableSlider(_m, 1f, 10f, (uint)SettingId.MValue);
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

                case SettingId.MValue:
                    _m = value;
                    break;

                case SettingId.NValue:
                    _n = value;
                    break;

                case SettingId.AValue:
                    _a = value;
                    break;

                case SettingId.BValue:
                    _b = value;
                    break;
            }
        }
    }
}

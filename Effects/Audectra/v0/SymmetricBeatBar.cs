/**
 * Copyright (c) Audectra e.U. - All Rights Reserved
 * This effect is part of Audectra.
 */

using System;

using SkiaSharp;
using Audectra.Extensions.Sdk.V1;

namespace Audectra.Extensions.Effects
{
    [MinWidth(4)]
    [LandscapeAspectRatio()]
    [EffectExtension("Symmetric Beat Bars", "Audectra", "1.3.0")]
    class SymmetricBeatBars : EffectExtensionBase
    {
        private IEffectApi _api;
        private SKColor _color;
        private IRender _render;
        private IAudioFeatureCache _featureCache;

        private float _remainingBeatTime;
        private float _beatPeriod;

        private enum SettingId
        {
            Color,
        }

        public SymmetricBeatBars(IEffectApi effectApi, int width, int height) : base(width, height)
        {
            _api = effectApi;
            _color = new SKColor(0, 128, 128);
            _render = _api.CreateRender();
            _featureCache = _api.CreateAudioFeatureCache();

            _remainingBeatTime = 0;
        }

        public override IRender Render(float dt)
        {
            float barValue = 0;

            if (_featureCache.IsBeat())
            {
                _beatPeriod = _featureCache.GetBeatPeriod();
                _remainingBeatTime = _beatPeriod;
            }
            else if (_remainingBeatTime > 0)
            {
                _remainingBeatTime = Math.Max(_remainingBeatTime - dt, 0);
                barValue = (1f - _remainingBeatTime / _beatPeriod);
            }

            int barSize = (int) (Width / 2 * barValue);

            using (var canvas = _api.CreateCanvas(_render))
            {
                canvas.Clear();

                var paint = new SKPaint
                {
                    IsAntialias = true,
                    Color = _color,
                    Style = SKPaintStyle.Fill
                };

                canvas.DrawRect(Width / 2, 0, barSize, Height, paint);
                canvas.DrawRect(Width / 2 - barSize, 0, barSize, Height, paint);
                paint.Dispose();
            }

            return _render;
        }

        public override void GenerateSettings(ISettingsBuilder settingsBuilder)
        {
            settingsBuilder.PageBegin();
            settingsBuilder.AddColorGroup(_color, (uint)SettingId.Color);
            settingsBuilder.PageEnd();
        }

        public override void OnSettingChanged(uint settingId, SettingValue value)
        {
            switch ((SettingId) settingId)
            {
                case SettingId.Color:
                    _color = value;
                    break;
            }
        }
    }
}

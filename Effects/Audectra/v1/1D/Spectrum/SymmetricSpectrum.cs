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
    [EffectExtension("Symmetric Spectrum", "Audectra", "v0.0.1")]
    class SymmetricSpectrum : EffectExtensionBase
    {
        private IEffectApi _api;
        private SKColor _color;
        private IRender _render;
        private IAudioFeatureCache _featureCache;

        private enum SettingId
        {
            Color,
        }

        public SymmetricSpectrum(IEffectApi api, int width, int height) : base(width, height)
        {
            _api = api;
            _color = new SKColor(0, 255, 128);
            _render = _api.CreateRender();
            _featureCache = _api.CreateAudioFeatureCache();
        }

        public override IRender Render(float dt)
        {
            var spectrum = _featureCache.GetMagnitudeSpectrum(Width / 2, 100, 4000);

            for (int x = 0; x < Width / 2; x++)
            {
                var color = _color.WithScale(spectrum[x]);

                for (int y = 0; y < Height; y++)
                {
                    _render[Width / 2 + x, y] = color;
                    _render[Width / 2 - x, y] = color;
                }
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

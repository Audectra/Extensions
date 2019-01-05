/**
 * Copyright (c) Audectra e.U. - All Rights Reserved
 * This effect is part of Audectra.
 */

using System;

using Audectra.Graphics;
using Audectra.Layers;
using Audectra.Layers.Effects;
using Audectra.Layers.Settings;


namespace Audectra.Extensions.Effects
{
    class SymmetricBeatBars : EffectBase, IExtension
    {
        private IEffectHelper _helper;
        private RgbColor _color;
        private IRgbRender _render;

        private float _remainingBeatTime;
        private float _beatPeriod;

        private enum SettingId
        {
            Color,
        }

        public SymmetricBeatBars() { }

        public SymmetricBeatBars(IEffectHelper effectHelper, int width, int height) : base(width, height)
        {
            _helper = effectHelper;
            _color = new RgbColor(0, 0.5f, 0.5f);
            _render = _helper.CreateRender();

            _remainingBeatTime = 0;
        }

        public override IRgbRender Render(float dt)
        {
            float barValue = 0;

            if (_helper.IsBeat())
            {
                _beatPeriod = _helper.GetBeatPeriod();
                _remainingBeatTime = _beatPeriod;
            }
            else if (_remainingBeatTime > 0)
            {
                _remainingBeatTime = Math.Max(_remainingBeatTime - dt, 0);
                barValue = (1f - _remainingBeatTime / _beatPeriod);
            }

            int barSize = (int) (Width / 2 * barValue);

            _render.Clear();
            _helper.FillBar(_render, Width / 2, barSize, _color);
            _helper.FillBar(_render, Width / 2 - barSize, barSize, _color);

            return _render;
        }

        public override void GenerateSettings(ILayerSettingsBuilder settingsBuilder)
        {
            settingsBuilder.PageBegin();
            settingsBuilder.AddColorGroup(this, _color, (uint)SettingId.Color);
            settingsBuilder.PageEnd();
        }

        public override void OnSettingChanged(uint settingId, object value)
        {
            switch ((SettingId) settingId)
            {
                case SettingId.Color:
                    _color = _helper.ValueToColor(value);
                    break;
            }
        }

        public string GetName()
        {
            return "Symmetric Beat Bars";
        }

        public string GetVersion()
        {
            return "v1.1.0";
        }

        public string GetAuthor()
        {
            return "Audectra";
        }
    }
}

/**
 * Copyright (c) Audectra e.U. - All Rights Reserved
 * This effect is part of Audectra.
 */

using System;

using Audectra.Gui;
using Audectra.Graphics;
using Audectra.Graphics.Effects;

namespace Audectra.Extensions.Effects
{
    class ValueBar : EffectBase, IExtension
    {
        private IEffectHelper _helper;
        private RgbColor _color;
        private IRgbRender _render;

        private float _barValue;

        private enum ValueId
        {
            Color,
            BarValue,
        }

        public ValueBar() { }

        public ValueBar(IEffectHelper effectHelper, int height, int width) : base(height, width)
        {
            _helper = effectHelper;
            _color = new RgbColor(0, 0.5f, 0.5f);
            _render = _helper.CreateRender();

            _barValue = 0;
        }

        public override IRgbRender Render(float dt)
        {
            _render.Clear();
            _helper.FillBar(_render, 0, (int)(Width * _barValue), _color);

            return _render;
        }

        public override void GenerateSettings(ILayerSettingsPanel settingsPanel)
        {
            settingsPanel.AddColorGroup(this, _color, (uint)ValueId.Color);

            settingsPanel.GroupBegin("Value");
            settingsPanel.AddBindableTrackbar(this, _barValue, 0, 1, (uint)ValueId.BarValue);
            settingsPanel.GroupEnd();
        }

        public override void ValueChanged(uint valueId, object value)
        {
            switch ((ValueId) valueId)
            {
                case ValueId.Color:
                    _color = _helper.ValueToColor(value);
                    break;

                case ValueId.BarValue:
                    _barValue = _helper.ValueToSingle(value);
                    break;
            }
        }

        public string GetName()
        {
            return "Value Bar";
        }

        public string GetVersion()
        {
            return "v1.0.0";
        }

        public string GetAuthor()
        {
            return "Audectra";
        }
    }
}
